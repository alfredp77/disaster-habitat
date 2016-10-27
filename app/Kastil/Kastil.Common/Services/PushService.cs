using System.Threading.Tasks;
using Kastil.Shared.Models;

namespace Kastil.Common.Services
{
    public class PushService : BaseService, IPushService
    {
        public Task Push<T>() where T : BaseModel
        {
            throw new System.NotImplementedException();
        }
    }
}