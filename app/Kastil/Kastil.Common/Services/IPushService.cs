using System.Threading.Tasks;
using Kastil.Common.Models;

namespace Kastil.Common.Services
{
    public interface IPushService
    {
        Task Push<T>(string tableName=null) where T : BaseModel;
    }
}