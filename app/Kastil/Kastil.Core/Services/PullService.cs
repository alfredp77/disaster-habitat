using System.Threading.Tasks;
using Kastil.Core.Utils;
using Kastil.Shared.Models;

namespace Kastil.Core.Services
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
            var docs = Serializer.ParseArray(json, "data", "id");

            var context = PersistenceContextFactory.CreateFor<T>();
            if (clear)
                await Asyncer.Async(context.DeleteAll);
            await Asyncer.Async(() => context.PersistAllJson(docs));
        }        
    }
}