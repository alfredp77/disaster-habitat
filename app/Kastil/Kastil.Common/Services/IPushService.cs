using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Kastil.Common.Models;
using Kastil.Common.Utils;

namespace Kastil.Common.Services
{
    public interface IPushService
    {
        Task<UpdateResult<T>> Push<T>(string userToken, Predicate<T> criteria=null) where T : BaseModel;
    }        
}