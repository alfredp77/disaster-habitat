using System.Collections.Generic;
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

        public async Task Push<T>(string userToken, string tableName=null) where T : BaseModel
        {
			var context = PersistenceContextFactory.CreateFor<T>();
			var allDocs = context.LoadAll();

			var url = Connection.GenerateTableUrl<T>();
            var headers = new Dictionary<string, string>(Connection.Headers) {{"user-token", userToken}};

            foreach (var doc in allDocs)
            {
                var json = await Caller.Post(url, headers, Serializer.Serialize(doc));
                var savedDoc = Serializer.Deserialize<T>(json);
                if (!string.IsNullOrEmpty(savedDoc.ObjectId))
                {
                    context.Save(savedDoc);
                }
            }                        
        }
    }
}