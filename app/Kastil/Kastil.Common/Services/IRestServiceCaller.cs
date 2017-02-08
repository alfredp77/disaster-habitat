using System.Collections.Generic;
using System.Threading.Tasks;

namespace Kastil.Common.Services
{
    public interface IRestServiceCaller
    {
        Task<string> Get(string url, Dictionary<string, string> headers);
        Task<string> Post(string url, Dictionary<string, string> headers, string payload);
		Task<string> Put(string url, Dictionary<string, string> headers, string payload);
        Task<string> Delete(string url, Dictionary<string, string> headers);
    }
}