using System.Linq;
using System.Threading.Tasks;
using Kastil.Shared.Models;

namespace Kastil.Common.Services
{
    public class SyncService : BaseService, ISyncService
    {
        private ITap2HelpService Tap2HelpService => Resolve<ITap2HelpService>();
        private IPullService PullService => Resolve<IPullService>();
        private IPushService PushService => Resolve<IPushService>();

        public async Task Sync(string staffCode)
        {
            await PullDisasters();
        }

        public async Task PullDisasters()
        {
            var localDisasterIds = (await Tap2HelpService.GetDisasters()).Select(d => d.Id);
            await PullService.Pull<Disaster>(true);
            var currentDisasterIds = (await Tap2HelpService.GetDisasters()).Select(d => d.Id);
            var removedDisasterIds = localDisasterIds.Except(currentDisasterIds);
            foreach (var removedDisasterId in removedDisasterIds)
            {
                await Tap2HelpService.DeleteAssessments(removedDisasterId);
            }
        }
    }    
}