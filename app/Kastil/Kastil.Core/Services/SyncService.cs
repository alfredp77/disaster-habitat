using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kastil.Core.Utils;
using Kastil.Shared.Models;
using Newtonsoft.Json.Linq;

namespace Kastil.Core.Services
{
    public class SyncService : BaseService, ISyncService
    {
        private IRestServiceCaller Caller => Resolve<IRestServiceCaller>();
        private IPersistenceContextFactory PersistenceContextFactory => Resolve<IPersistenceContextFactory>();
        private IJsonSerializer Serializer => Resolve<IJsonSerializer>();
        private Connection Connection => Resolve<Connection>();
        
        public async Task Sync<T>(bool clear=false) where T : BaseModel
        {
            var url = Connection.GenerateGetUrl<T>();
            var json = await Caller.Get(url, Connection.Headers);
            var docs = Serializer.ParseArray(json, "data", "id");

            var context = PersistenceContextFactory.CreateFor<T>();
            if (clear)
                await Asyncer.Async(context.DeleteAll);
            await Asyncer.Async(() => context.PersistAllJson(docs));
        }
    }
}