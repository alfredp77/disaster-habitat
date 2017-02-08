using System.Threading.Tasks;
using Kastil.Common.Models;

namespace Kastil.Core.Services
{
    public interface ISyncService
    {
        Task Sync(User user);
    }
}