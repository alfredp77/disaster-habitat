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

        public async Task<IEnumerable<PushResult<T>>>  Push<T>(string userToken, Predicate<T> criteria=null) where T : BaseModel
        {            
            var headers = new Dictionary<string, string>(Connection.Headers) { { "user-token", userToken } };
            criteria = criteria ?? (a => true);

            var context = PersistenceContextFactory.CreateFor<T>();
            var items = await Asyncer.Async(() => context.LoadAll());
            var savedItems = new List<PushResult<T>>();
            foreach (var item in items.Where(i => criteria(i)))
            {                
                var savedItem = await PushItem(item, headers);
                if (savedItem == null || savedItem.IsNew())
                {
                    // somethin is wrong, log error and continue with the rest
                    continue;
                }

                context.Save(savedItem);
                context.Purge(item.ObjectId);
                savedItems.Add(new PushResult<T>(savedItem, item.ObjectId));
            }
            return savedItems;
        }

        private async Task<T> PushItem<T>(T item, Dictionary<string, string> headers) where T : BaseModel
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

            var result = await pushTask;
            return Serializer.Deserialize<T>(result);
        }

        private T PrepareItem<T>(T item) where T : BaseModel
        {
            var cloned = Serializer.Clone(item);
            cloned.ObjectId = null;
            cloned.OwnerId = null;
            return cloned;
        }
    }

    public class PushResult<T> where T : BaseModel
    {
        public T Pushed { get; }
        public string LocalId { get; }

        public PushResult(T pushed, string localId)
        {
            Pushed = pushed;
            LocalId = localId;
        }
    }
}