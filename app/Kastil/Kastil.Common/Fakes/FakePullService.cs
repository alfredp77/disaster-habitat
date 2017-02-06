using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kastil.Common.Services;
using Kastil.Common.Models;

namespace Kastil.Common.Fakes
{
    public class FakePullService : IPullService
    {
        public Task<IEnumerable<T>> Pull<T>(string queryString=null, bool persist = true) where T : BaseModel
        {
            return Task.FromResult(Enumerable.Empty<T>());
        }
    }
}