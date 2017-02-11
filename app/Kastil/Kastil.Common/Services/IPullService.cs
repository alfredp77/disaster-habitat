using System.Collections.Generic;
using System.Threading.Tasks;
using Kastil.Common.Models;
using Kastil.Common.Utils;

namespace Kastil.Common.Services
{
    public interface IPullService
    {
        Task<SyncResult<T>> Pull<T>(string queryString="", bool persist=true) where T : BaseModel;
    }
}