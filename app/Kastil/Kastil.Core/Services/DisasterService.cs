using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kastil.Shared.Models;

namespace Kastil.Core.Services
{

    public class ServiceCallResult
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public Exception Exception { get; set; }
    }

   
}
