using System;
using System.Threading.Tasks;
using Kastil.Common.Services;

namespace Kastil.Common.Fakes
{
    public class FakeSyncService : ISyncService
    {
        public Task Sync(string staffCode)
        {
            return Task.Delay(TimeSpan.FromSeconds(3));
        }
    }
}