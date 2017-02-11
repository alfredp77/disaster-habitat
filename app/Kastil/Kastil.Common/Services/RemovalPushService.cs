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
        public const string DeletionTime = "deletionTime";
        private IRestServiceCaller Caller => Resolve<IRestServiceCaller>();
        private IPersistenceContextFactory PersistenceContextFactory => Resolve<IPersistenceContextFactory>();
        private IJsonSerializer Serializer => Resolve<IJsonSerializer>();
        private Connection Connection => Resolve<Connection>();
        private IBackendlessResponseParser ResponseParser => Resolve<IBackendlessResponseParser>();

        public async Task<SyncResult<T>> Push<T>(string userToken, Predicate<T> criteria = null) where T : BaseModel
        {            
            var headers = new Dictionary<string, string>(Connection.Headers) { { "user-token", userToken } };
            criteria = criteria ?? (a => true);

            var context = PersistenceContextFactory.CreateFor<T>();
            var items = await Asyncer.Async(() => context.LoadDeletedObjects());
            var result = new SyncResult<T>();
            foreach (var item in items.Where(i => criteria(i)))
            {
                var url = Connection.GenerateTableUrl<T>(item.ObjectId);
                var json = await Caller.Delete(url, headers);
                var asDictionary = Serializer.AsDictionary(json);
                if (asDictionary.ContainsKey(DeletionTime))
                {
                    context.Purge(item.ObjectId);
                    result.Success(item, item.ObjectId);
                }
                else
                {
                    var parsed = ResponseParser.Parse<T>(json);
                    result.Failed(item, parsed.ToString());
                } 

            }
            return result;
        }
    }
}