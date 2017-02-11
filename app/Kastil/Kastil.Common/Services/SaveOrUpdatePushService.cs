using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kastil.Common.Models;
using Kastil.Common.Utils;

namespace Kastil.Common.Services
{
    public class SaveOrUpdatePushService : BaseService, IPushService
    {
        private IRestServiceCaller Caller => Resolve<IRestServiceCaller>();
        private IPersistenceContextFactory PersistenceContextFactory => Resolve<IPersistenceContextFactory>();
        private IJsonSerializer Serializer => Resolve<IJsonSerializer>();
        private Connection Connection => Resolve<Connection>();
        private IBackendlessResponseParser ResponseParser => Resolve<IBackendlessResponseParser>();

        public async Task<SyncResult<T>>  Push<T>(string userToken, Predicate<T> criteria=null) where T : BaseModel
        {            
            var headers = new Dictionary<string, string>(Connection.Headers) { { "user-token", userToken } };
            criteria = criteria ?? (a => true);

            var context = PersistenceContextFactory.CreateFor<T>();
            var items = await Asyncer.Async(() => context.LoadAll());
            var result = new SyncResult<T>();
            foreach (var item in items.Where(i => criteria(i)))
            {                
                var json = await PushItem(item, headers);
                var parsed = ResponseParser.Parse<T>(json);
                if (parsed.IsSuccessful)
                {
                    context.Save(parsed.Content);
                    context.Purge(item.ObjectId);
                    result.Success(parsed.Content, item.ObjectId);
                }
                else
                {
                    result.Failed(item, parsed.ToString());
                }
            }
            return result;
        }

        private async Task<string> PushItem<T>(T item, Dictionary<string, string> headers) where T : BaseModel
        {            
            var payload = Serializer.Serialize(PrepareItem(item));
            Task<string> pushTask;
            if (item.IsNew()) 
            {
                pushTask = Caller.Post(Connection.GenerateTableUrl<T>(), headers, payload);
            } 
            else 
            {
                pushTask = Caller.Put(Connection.GenerateTableUrl<T>(item.ObjectId), headers, payload);
            }

            return await pushTask;
        }

        private T PrepareItem<T>(T item) where T : BaseModel
        {
            var cloned = Serializer.Clone(item);
            cloned.ObjectId = null;
            cloned.OwnerId = null;
            return cloned;
        }
    }
}