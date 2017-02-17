using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kastil.Common.Models;
using Kastil.Common.Services;

namespace Kastil.Core.Services
{
    public class SyncService : ISyncService
    {
        private readonly IPersistenceContextFactory _persistenceContextFactory;
        private readonly List<ISyncService> _syncServices;

        public SyncService(IPersistenceContextFactory persistenceContextFactory, IEnumerable<ISyncService> syncServices)
        {
            _persistenceContextFactory = persistenceContextFactory;
            _syncServices = syncServices.ToList();
        }

        public Func<DateTimeOffset> GetCurrentTimeFunc { get; set; } = () => DateTimeOffset.UtcNow;

        public async Task<SyncResult> Sync(User user)
        {
            foreach (var syncService in _syncServices)
            {
                var result = await syncService.Sync(user);
                if (result.HasErrors)
                    return result;
            }
            
			RecordLastSync();
            return SyncResult.Success();
        }

		private void RecordLastSync()
		{
            var currentTime = GetCurrentTimeFunc();
            var syncInfo = new SyncInfo { ObjectId = "x", LastSync = currentTime };
			var context = _persistenceContextFactory.CreateFor<SyncInfo>();
			context.Save(syncInfo);
		}        
    }
}