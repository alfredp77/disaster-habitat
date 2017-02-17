using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Kastil.Common.Models;
using Kastil.Common.Services;
using Kastil.Common.Utils;
using Attribute = Kastil.Common.Models.Attribute;

namespace Kastil.Core.Services
{
    public class SyncService : BaseService, ISyncService
    {
        private ITap2HelpService Tap2HelpService => Resolve<ITap2HelpService>();
        private IPullService PullService => Resolve<IPullService>();
        private IPushService PushService => Resolve<IPushService>();
        private IPersistenceContextFactory ContextFactory => Resolve<IPersistenceContextFactory>();
        private IBackendlessQueryProvider QueryProvider => Resolve<IBackendlessQueryProvider>();

        public async Task<SyncResult> Sync(User user)
        {
			var currentTime = DateTimeOffset.UtcNow;

            await PullDisasters();
            await PullAttributes();
            await PushAssessments(user);
            await PushShelters(user);

			RecordLastSync(currentTime);
            return new SyncResult();
        }

		private void RecordLastSync(DateTimeOffset currentTime)
		{
			var syncInfo = new SyncInfo { ObjectId = "x", LastSync = currentTime };
			var context = ContextFactory.CreateFor<SyncInfo>();
			context.Save(syncInfo);
		}

        public async Task PullDisasters()
        {
            var localDisasterIds = (await Tap2HelpService.GetDisasters()).Select(d => d.ObjectId);
            await PullService.Pull<Disaster>();
            var currentDisasterIds = (await Tap2HelpService.GetDisasters()).Select(d => d.ObjectId);
            var removedDisasterIds = localDisasterIds.Except(currentDisasterIds);
            foreach (var removedDisasterId in removedDisasterIds) {
                await Tap2HelpService.DeleteAssessments(removedDisasterId);
                await Tap2HelpService.DeleteShelters(removedDisasterId);
            }
        }

        private async Task PullAttributes()
        {
            await PullService.Pull<Attribute>();
        }

        private async Task PushShelters(User user)
        {
            //await PushService.Push<Shelter>(userToken, "Disaster");
        }

        private async Task PushAssessments(User user)
        {            
            var savedAssessments = await PushService.Push<Assessment>(user.Token);
            var assessmentIds = new HashSet<string>(savedAssessments.SuccessfulItems.Select(savedAssessments.GetLocalId));
            var attributePushResult = await PushService.Push<AssessmentAttribute>(user.Token, a => assessmentIds.Contains(a.ItemId));

            // if there's any failure, we should not continue doing the next steps
            if (savedAssessments.FailedItems.Any() || attributePushResult.FailedItems.Any())
            {
                // log error
                return;
            }

            // pull assessments
            var query = QueryProvider.Where().OwnedBy(user.ObjectId).IsActive();
            await PullService.Pull<Assessment>(query);

            // pull attributes
            await PullService.Pull<AssessmentAttribute>(query);

            // remove all assessments that do not belong to this user
            var assessmentContext = ContextFactory.CreateFor<Assessment>();
            var attributeContext = ContextFactory.CreateFor<AssessmentAttribute>();
            var existingAssessments = await Tap2HelpService.GetAssessments();
            foreach (var existingAssessment in existingAssessments.Where(a => !a.IsNew() && !user.ObjectId.Equals(a.OwnerId, StringComparison.CurrentCultureIgnoreCase)))
            {
                var attributes = await Tap2HelpService.GetAssessmentAttributes(existingAssessment.ObjectId);
                foreach (var assessmentAttribute in attributes)
                {
                    attributeContext.Purge(assessmentAttribute.ObjectId);
                }
                assessmentContext.Purge(existingAssessment.ObjectId);
            }
        }
    }
}