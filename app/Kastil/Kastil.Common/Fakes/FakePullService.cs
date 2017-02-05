using System.Threading.Tasks;
using Kastil.Common.Services;
using Kastil.Common.Utils;
using Kastil.Common.Models;

namespace Kastil.Common.Fakes
{
    public class FakePullService : IPullService
    {
        public Task Pull<T>(string tableName=null) where T : BaseModel
        {
            return Asyncer.DoNothing();
        }
    }
}