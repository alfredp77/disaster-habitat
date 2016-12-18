using System.Threading.Tasks;
using Kastil.Common.Models;

namespace Kastil.Common.Services
{
    public interface IPullService
    {
        Task Pull<T>(string tableName=null) where T : BaseModel;
    }
}