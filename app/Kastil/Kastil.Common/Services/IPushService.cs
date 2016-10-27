using System.Threading.Tasks;
using Kastil.Shared.Models;

namespace Kastil.Common.Services
{
    public interface IPushService
    {
        Task Push<T>() where T : BaseModel;
    }
}