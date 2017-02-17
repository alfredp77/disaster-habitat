using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kastil.Common.Models;
using Kastil.Common.Services;

namespace Kastil.Core.Services
{
    public class DisasterSyncService : ISyncService
    {
        private readonly IPullService _pullService;
        private readonly IPersistenceContextFactory _persistenceContextFactory;

        public DisasterSyncService(IPullService pullService, IPersistenceContextFactory persistenceContextFactory)
        {
            _pullService = pullService;
            _persistenceContextFactory = persistenceContextFactory;
        }

        public async Task<SyncResult> Sync(User user)
        {
            var pullResult = await _pullService.Pull<Disaster>();
            var pulledItems = new HashSet<string>(pullResult.SuccessfulItems.Select(i => i.ObjectId));

            if (!pullResult.FailedItems.Any() && pulledItems.Any())
            {
                PurgeRelated<Assessment>(pulledItems);
                PurgeRelated<Shelter>(pulledItems);
            }
            return pullResult.FailedItems.Any() ? SyncResult.Failed("Unable to pull latest Disasters.") 
                : SyncResult.Success();

        }

        private void PurgeRelated<T>(HashSet<string> disasterIds) where T : Attributed
        {
            var context = _persistenceContextFactory.CreateFor<T>();
            var items = context.LoadAll()
                .Where(k => !disasterIds.Contains(k.DisasterId))
                .ToList();
            foreach (var item in items)
            {
                context.Purge(item.ObjectId);
            }
        }
    }
}