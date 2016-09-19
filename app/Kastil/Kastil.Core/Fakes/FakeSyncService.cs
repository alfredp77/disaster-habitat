using System;
using System.Threading.Tasks;
using Kastil.Core.Services;

namespace Kastil.Core.Fakes
{
    public class FakeSyncService : ISyncService
    {
        public Task Sync()
        {
            return Task.Delay(TimeSpan.FromSeconds(3));
        }
    }
}