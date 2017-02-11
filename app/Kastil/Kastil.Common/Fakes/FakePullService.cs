using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kastil.Common.Services;
using Kastil.Common.Models;
using Kastil.Common.Utils;

namespace Kastil.Common.Fakes
{
    public class FakePullService : IPullService
    {
        public Task<SyncResult<T>> Pull<T>(string queryString=null, bool persist = true) where T : BaseModel
        {
            return Task.FromResult(new SyncResult<T>());
        }
    }
}