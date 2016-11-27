using System.Threading.Tasks;
using Kastil.Common;
using Kastil.Common.Models;
using Kastil.Common.Services;
using Kastil.Common.Utils;

namespace Kastil.Core.Services
{
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