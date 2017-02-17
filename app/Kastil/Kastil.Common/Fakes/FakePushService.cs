using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kastil.Common.Services;
using Kastil.Common.Models;
using Kastil.Common.Utils;

namespace Kastil.Common.Fakes
{
    public class FakePushService : IPushService
    {
        public Task<UpdateResult<T>> Push<T>(string userToken, Predicate<T> criteria = null) where T : BaseModel
        {
            return Task.FromResult(new UpdateResult<T>());
        }
    }
}