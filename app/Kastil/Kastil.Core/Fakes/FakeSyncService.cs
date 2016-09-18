using System.Threading.Tasks;
using Kastil.Core.Services;
using Kastil.Core.Utils;
using Kastil.Shared.Models;

namespace Kastil.Core.Fakes
{
    public class FakeSyncService : ISyncService
    {
        public Task Sync<T>(bool clear = false) where T : BaseModel
        {
            return Asyncer.Async(() => { });
        }
    }
}