using System;
using System.Linq;
using System.Threading.Tasks;
using Kastil.Common.Models;
using Kastil.Common.Utils;
using Attribute = Kastil.Common.Models.Attribute;

namespace Kastil.Common.Services
{
	public class SyncService : BaseService, ISyncService
	{
		private ITap2HelpService Tap2HelpService => Resolve<ITap2HelpService>();
		private IPullService PullService => Resolve<IPullService>();
		private IPushService PushService => Resolve<IPushService>();

		public async Task Sync(string staffCode)
		{
			await PullDisasters();
			await PullShelters();
		}

		public async Task PullDisasters()
		{
			var localDisasterIds = (await Tap2HelpService.GetDisasters()).Select(d => d.ObjectId);
			await PullService.Pull<Disaster>(true);
			var currentDisasterIds = (await Tap2HelpService.GetDisasters()).Select(d => d.ObjectId);
			var removedDisasterIds = localDisasterIds.Except(currentDisasterIds);
			foreach (var removedDisasterId in removedDisasterIds) {
				await Tap2HelpService.DeleteAssessments(removedDisasterId);
			}
		}

		public async Task PullShelters()
		{
			await PullService.Pull<Shelter>(true);
		}

		public async Task PullAttributes()
		{
			await PullService.Pull<Attribute>(true);
		}

		public async Task PushAssessments()
		{
			await PushService.Push<Assessment>("Assesment");
		}
	}

    public interface ILoginService
    {
        Task<User> Login(string login, string password);
    }

	public class LoginService : BaseService, ILoginService
	{
		private IRestServiceCaller Caller { get; }
        private Connection Connection { get;  }
	    private IJsonSerializer Serializer {get;}

	    public LoginService(Connection connection, IRestServiceCaller caller, IJsonSerializer serializer)
	    {
	        Connection = connection;
	        Caller = caller;
	        Serializer = serializer;
	    }

        public async Task<User> Login(string login, string password)
		{
		    var url = Connection.LoginUrl;
            var result = await Caller.Post(url, Connection.Headers, Serializer.Serialize(new {login, password}));
            return Serializer.Deserialize<User>(result);
		}
	}
}