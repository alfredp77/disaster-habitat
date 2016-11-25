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

        public Task Push<T>(string tableName=null) where T : BaseModel
        {
			var context = PersistenceContextFactory.CreateFor<T>();
			var allDocs = context.LoadAll();

			var url = Connection.GenerateGetUrl<T>();

			//var json = await Caller.Get(url, Connection.Headers);
            return null;
        }
    }
}