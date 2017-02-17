using System;
using System.Threading.Tasks;
using Kastil.Common.Models;
using Kastil.Common.Services;
using Kastil.Core.Services;

namespace Kastil.Core.Fakes
{
    public class FakeSyncService : ISyncService
    {
        public async Task<SyncResult> Sync(User user)
        {
            await Task.Delay(TimeSpan.FromSeconds(3));
            return SyncResult.Success();
        }
    }
}