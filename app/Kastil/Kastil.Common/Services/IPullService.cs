using System.Threading.Tasks;
using Kastil.Common.Models;

namespace Kastil.Common.Services
{
    public interface IPullService
    {
        Task Pull<T>(bool clear=false) where T : BaseModel;
    }
}