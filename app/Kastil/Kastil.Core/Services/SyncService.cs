using System;
using System.Collections.Generic;
using System.Linq;
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
        private AssessmentPushService AssessmentPushService => Resolve<AssessmentPushService>();
        private ShelterPushService ShelterPushService => Resolve<ShelterPushService>();
        private IPersistenceContextFactory ContextFactory => Resolve<IPersistenceContextFactory>();

        public async Task Sync(string userToken)
        {
			var currentTime = DateTimeOffset.UtcNow;

            await PullDisasters();
            await PullAttributes();
            await PushAssessments(userToken);
            await PushShelters(userToken);

			RecordLastSync(currentTime);
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
            await PullService.Pull<Common.Models.Attribute>("Attributes");
        }

        private async Task PushShelters(string userToken)
        {
            //await PushService.Push<Shelter>(userToken, "Disaster");
        }

        private async Task PushAssessments(string userToken)
        {
            await AssessmentPushService.Push(userToken);
        }
    }
}