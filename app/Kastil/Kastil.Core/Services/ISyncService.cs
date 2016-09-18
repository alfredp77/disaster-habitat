using System.Threading.Tasks;
using Kastil.Shared.Models;

namespace Kastil.Core.Services
{
    public interface ISyncService
    {
        Task Sync<T>(bool clear=false) where T : BaseModel;
    }
}