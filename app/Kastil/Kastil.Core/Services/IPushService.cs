using System.Threading.Tasks;
using Kastil.Shared.Models;

namespace Kastil.Core.Services
{
    public interface IPushService
    {
        Task Push<T>() where T : BaseModel;
    }
}