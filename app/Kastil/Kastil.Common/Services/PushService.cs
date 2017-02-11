using System;
using System.Threading.Tasks;
using Kastil.Common.Models;
using Kastil.Common.Utils;

namespace Kastil.Common.Services
{
    public class PushService : IPushService
    {
        private readonly IPushService _saveOrUpdate;
        private readonly IPushService _removal;

        public PushService(IPushService saveOrUpdate, IPushService removal)
        {
            _saveOrUpdate = saveOrUpdate;
            _removal = removal;
        }      

        public async Task<SyncResult<T>> Push<T>(string userToken, Predicate<T> criteria = null) where T : BaseModel
        {
            var removed = await _removal.Push(userToken, criteria);
            var saved = await _saveOrUpdate.Push(userToken, criteria);
            return saved.Merge(removed);
        }
    }
}