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
        
        public async Task Pull<T>(bool clear=false) where T : BaseModel
        {
            var url = Connection.GenerateGetUrl<T>();
            var json = await Caller.Get(url, Connection.Headers);
            var docs = Serializer.ParseArray(json, "data", "objectId");

            var context = PersistenceContextFactory.CreateFor<T>();
            if (clear)
                await Asyncer.Async(context.DeleteAll);
            await Asyncer.Async(() => context.PersistAllJson(docs));
        }        
    }
}