using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kastil.Common.Models;
using Kastil.Common.Utils;

namespace Kastil.Common.Services
{
    public class PushService : BaseService, IPushService
    {
        private IRestServiceCaller Caller => Resolve<IRestServiceCaller>();
        private IPersistenceContextFactory PersistenceContextFactory => Resolve<IPersistenceContextFactory>();
        private IJsonSerializer Serializer => Resolve<IJsonSerializer>();
        private Connection Connection => Resolve<Connection>();

        public async Task<IEnumerable<T>>  Push<T>(string userToken, Predicate<T> criteria=null) where T : BaseModel
        {
            var url = Connection.GenerateTableUrl<Disaster>();
            var headers = new Dictionary<string, string>(Connection.Headers) { { "user-token", userToken } };
            criteria = criteria ?? (a => true);

            var context = PersistenceContextFactory.CreateFor<T>();
            var items = await Asyncer.Async(() => context.LoadAll());
            var savedItems = new List<T>();
            foreach (var item in items.Where(i => criteria(i)))
            {
                var savedItem = await PushItem(item, url, headers);
                if (savedItem == null || savedItem.IsNew())
                {
                    // somethin is wrong, log error and continue with the rest
                    continue;
                }

                context.Save(savedItem);
                savedItems.Add(savedItem);
            }
            return savedItems;
        }

        private async Task<T> PushItem<T>(T item, string url, Dictionary<string, string> headers) where T : BaseModel
        {
            item.RevokeNewId();
            var payload = Serializer.Serialize(item);
            var pushTask = item.IsNew() ? Caller.Post(url, headers, payload) : Caller.Put($"{url}/{item.ObjectId}", headers, payload);

            var result = await pushTask;
            return Serializer.Deserialize<T>(result);
        }
    }
}