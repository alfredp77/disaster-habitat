using System.Threading.Tasks;
using Kastil.Common.Services;
using Kastil.Common.Utils;
using Kastil.Shared.Models;

namespace Kastil.Common.Fakes
{
    public class FakePullService : IPullService
    {
        public Task Pull<T>(bool clear = false) where T : BaseModel
        {
            return Asyncer.DoNothing();
        }
    }
}