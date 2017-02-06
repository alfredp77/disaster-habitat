using System.Collections.Generic;
using System.Threading.Tasks;
using Kastil.Common.Models;

namespace Kastil.Common.Services
{
    public interface IPullService
    {
        Task<IEnumerable<T>> Pull<T>(string queryString="", bool persist=true) where T : BaseModel;
    }
}