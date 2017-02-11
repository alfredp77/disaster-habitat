using Kastil.Common.Models;

namespace Kastil.Common.Utils
{
    public interface IBackendlessResponseParser
    {
        BackendlessResponse<T> Parse<T>(string response) where T : BaseModel;
    }
}