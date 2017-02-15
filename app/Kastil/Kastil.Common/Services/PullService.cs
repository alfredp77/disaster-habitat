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
        private Connection Connection => Resolve<Connection>();
        private IBackendlessResponseParser ResponseParser => Resolve<IBackendlessResponseParser>();

        public async Task<SyncResult<T>> Pull<T>(IQuery query=null, bool persist = true) where T : BaseModel
        {
            var url = $"{Connection.GenerateTableUrl<T>()}{query}";
            var json = await Caller.Get(url, Connection.Headers);

            var parsed = ResponseParser.ParseArray<T>(json);
            if (!parsed.IsSuccessful)
                return Failure(parsed);

            if (persist)
                Persist(parsed);

            return Success(parsed);
        }        

        private static SyncResult<T> Failure<T>(BackendlessResponse<T> parsed) where T : BaseModel
        {
            var result = new SyncResult<T>();
            result.Failed(null, parsed.ToString());
            return result;
        }

        private static SyncResult<T> Success<T>(BackendlessResponse<T> parsed) where T : BaseModel
        {
            var result = new SyncResult<T>();
            foreach (var item in parsed.Content)
            {
                result.Success(item, item.ObjectId);
            }

            return result;
        }

        private void Persist<T>(BackendlessResponse<T> parsed) where T : BaseModel
        {
            var context = PersistenceContextFactory.CreateFor<T>();
            var all = context.LoadAll()
                .Where(i => !i.IsNew())
                .ToDictionary(i => i.ObjectId);

            foreach (var item in parsed.Content)
            {
                context.Save(item);
                all.Remove(item.ObjectId);
            }

            foreach (var item in all.Values)
            {
                context.Purge(item.ObjectId);
            }
        }
    }
}