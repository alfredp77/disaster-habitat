using System.Threading.Tasks;
using Kastil.Common.Models;

namespace Kastil.Common.Services
{
    public interface IPushService
    {
        Task Push<T>(string userToken, string tableName=null) where T : BaseModel;
        Task Push(string userToken, object o, string tableName);
    }

    public interface IPushService2
    {
        Task Push(string userToken);
    }
}