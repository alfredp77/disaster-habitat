using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kastil.Common.Services;
using Kastil.Common.Models;

namespace Kastil.Common.Fakes
{
    public class FakePushService : IPushService
    {
        public Task<IEnumerable<PushResult<T>>> Push<T>(string userToken, Predicate<T> criteria = null) where T : BaseModel
        {
            return Task.FromResult(Enumerable.Empty<PushResult<T>>());
        }
    }
}