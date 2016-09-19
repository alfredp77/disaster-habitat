using System.Threading.Tasks;
using Kastil.Shared.Models;

namespace Kastil.Core.Services
{
    public interface IPullService
    {
        Task Pull<T>(bool clear=false) where T : BaseModel;
    }
}