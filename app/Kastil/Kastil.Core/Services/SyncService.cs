using System.Linq;
using System.Threading.Tasks;
using Kastil.Common.Models;
using Kastil.Common.Services;

namespace Kastil.Core.Services
{
    public class SyncService : BaseService, ISyncService
    {
        private ITap2HelpService Tap2HelpService => Resolve<ITap2HelpService>();
        private IPullService PullService => Resolve<IPullService>();
        private IPushService PushService => Resolve<IPushService>();

        public async Task Sync(string userToken)
        {
            await PullDisasters();
            await PullAttributes();
            await PushAssessments(userToken);
            await PushShelters(userToken);
        }

        public async Task PullDisasters()
        {
            var localDisasterIds = (await Tap2HelpService.GetDisasters()).Select(d => d.ObjectId);
            await PullService.Pull<Disaster>(true);
            var currentDisasterIds = (await Tap2HelpService.GetDisasters()).Select(d => d.ObjectId);
            var removedDisasterIds = localDisasterIds.Except(currentDisasterIds);
            foreach (var removedDisasterId in removedDisasterIds) {
                await Tap2HelpService.DeleteAssessments(removedDisasterId);
                await Tap2HelpService.DeleteShelters(removedDisasterId);
            }
        }

        private async Task PullAttributes()
        {
            await PullService.Pull<Attribute>(true);
        }

        private async Task PushShelters(string userToken)
        {
            await PushService.Push<Shelter>(userToken);
        }

        private async Task PushAssessments(string userToken)
        {
            await PushService.Push<Assessment>(userToken, "Assesment");
        }
    }
}