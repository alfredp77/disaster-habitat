using System.Linq;
using System.Threading.Tasks;
using Kastil.Common.Models;
using Kastil.Common.Services;

namespace Kastil.Core.Services
{
    public class AttributeSyncService : ISyncService
    {
        private readonly IPullService _pullService;

        public AttributeSyncService(IPullService pullService)
        {
            _pullService = pullService;
        }

        public async Task<SyncResult> Sync(User user)
        {
            var result = await _pullService.Pull<Attribute>();
            return result.FailedItems.Any() ? SyncResult.Failed("Unable to pull latest Attributes.") : SyncResult.Success();
        }
    }
}