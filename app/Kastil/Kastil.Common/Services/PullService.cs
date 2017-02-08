using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kastil.Common.Utils;
using Kastil.Common.Models;

namespace Kastil.Common.Services
{
    public class PullService : BaseService, IPullService
    {
        private IRestServiceCaller Caller => Resolve<IRestServiceCaller>();
        private IPersistenceContextFactory PersistenceContextFactory => Resolve<IPersistenceContextFactory>();
        private IJsonSerializer Serializer => Resolve<IJsonSerializer>();
        private Connection Connection => Resolve<Connection>();
        
        public async Task<IEnumerable<T>> Pull<T>(string queryString="", bool persist = true) where T : BaseModel
        {
            var url = $"{Connection.GenerateTableUrl<T>()}{queryString}";
            var json = await Caller.Get(url, Connection.Headers);
            var docs = Serializer.ParseArray(json, "data", "objectId").ToList();

            if (persist)
            {
                await Persist<T>(docs);
            }

            return docs.Select(d => Serializer.Deserialize<T>(d.Value));
        }

        private async Task Persist<T>(IEnumerable<KeyValuePair<string, string>> docs) where T : BaseModel
        {
            var context = PersistenceContextFactory.CreateFor<T>();

            var all = context.LoadAll();
            foreach (var item in all.Where(i => !i.IsNew()))
            {
                context.Purge(item.ObjectId);
            }
            await Asyncer.Async(() => context.PersistAllJson(docs));
        }
    }
}