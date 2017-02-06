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
            var docs = Serializer.ParseArray(json, "data", "objectId");

            if (persist)
            {
                var context = PersistenceContextFactory.CreateFor<T>();
                await Asyncer.Async(context.PurgeAll);
                await Asyncer.Async(() => context.PersistAllJson(docs));
            }

            return docs.Select(d => Serializer.Deserialize<T>(d.Value));
        }        
    }
}