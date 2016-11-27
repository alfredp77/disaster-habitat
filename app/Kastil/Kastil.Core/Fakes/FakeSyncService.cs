using System;
using System.Threading.Tasks;
using Kastil.Common.Services;
using Kastil.Core.Services;

namespace Kastil.Core.Fakes
{
    public class FakeSyncService : ISyncService
    {
        public Task Sync(string userToken)
        {
            return Task.Delay(TimeSpan.FromSeconds(3));
        }
    }
}