using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Kastil.Common.Models;

namespace Kastil.Common.Services
{
    public interface IPushService
    {
        Task<IEnumerable<T>> Push<T>(string userToken, Predicate<T> criteria=null) where T : BaseModel;
    }        
}