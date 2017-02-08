using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kastil.Common.Models;
using Kastil.Common.Utils;

namespace Kastil.Common.Services
{
    public class RemovalPushService : BaseService, IPushService
    {
        private IRestServiceCaller Caller => Resolve<IRestServiceCaller>();
        private IPersistenceContextFactory PersistenceContextFactory => Resolve<IPersistenceContextFactory>();
        private IJsonSerializer Serializer => Resolve<IJsonSerializer>();
        private Connection Connection => Resolve<Connection>();

        public async Task<IEnumerable<PushResult<T>>> Push<T>(string userToken, Predicate<T> criteria = null) where T : BaseModel
        {            
            var headers = new Dictionary<string, string>(Connection.Headers) { { "user-token", userToken } };
            criteria = criteria ?? (a => true);

            var context = PersistenceContextFactory.CreateFor<T>();
            var items = await Asyncer.Async(() => context.LoadDeletedObjects());
            var deletedItems = new List<PushResult<T>>();
            foreach (var item in items.Where(i => criteria(i)))
            {
                var url = Connection.GenerateTableUrl<T>(item.ObjectId);
                var result = await Caller.Delete(url, headers);
                var asDictionary = Serializer.AsDictionary(result);
                if (asDictionary.ContainsKey("deletionTime"))
                {
                    context.Purge(item.ObjectId);
                    deletedItems.Add(new PushResult<T>(item, item.ObjectId));
                }
                else
                {
                    // log error 
                } 

            }
            return deletedItems;
        }
    }
}