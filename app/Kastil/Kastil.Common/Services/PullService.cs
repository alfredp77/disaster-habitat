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
        private IBackendlessResponseParser ResponseParser => Resolve<IBackendlessResponseParser>();

        public async Task<SyncResult<T>> Pull<T>(string queryString="", bool persist = true) where T : BaseModel
        {
            var url = $"{Connection.GenerateTableUrl<T>()}{queryString}";
            var json = await Caller.Get(url, Connection.Headers);

            var parsed = ResponseParser.Parse<T>(json);
            var result = new SyncResult<T>();
            if (parsed.IsSuccessful)
            {
                var docs = Serializer.ParseArray(json, "data", "objectId").ToList();

                if (persist)
                {
                    await Persist<T>(docs);
                }

                foreach (var doc in docs)
                {
                    var deserialize = Serializer.Deserialize<T>(doc.Value);
                    result.Success(deserialize, deserialize.ObjectId);
                }
            }
            else
            {
                result.Failed(null, parsed.ToString());
            }

            return result;
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