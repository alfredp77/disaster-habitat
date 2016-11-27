using System.Threading.Tasks;

namespace Kastil.Core.Services
{
    public interface ISyncService
    {
        Task Sync(string userToken);
    }
}