using System.Threading.Tasks;
using Kastil.Common.Services;
using Kastil.Common.Utils;
using Kastil.Shared.Models;

namespace Kastil.Common.Fakes
{
    public class FakePushService : IPushService
    {
        public Task Push<T>() where T : BaseModel
        {
            return Asyncer.DoNothing();
        }
    }
}