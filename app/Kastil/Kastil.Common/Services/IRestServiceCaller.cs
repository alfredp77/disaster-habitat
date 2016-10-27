using System.Collections.Generic;
using System.Threading.Tasks;

namespace Kastil.Common.Services
{
    public interface IRestServiceCaller
    {
        Task<string> Get(string url, Dictionary<string, string> headers);
    }
}