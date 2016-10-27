using System.Threading.Tasks;

namespace Kastil.Common.Services
{
    public interface ISyncService
    {
        Task Sync(string staffCode);
    }
}