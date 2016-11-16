using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kastil.Common.Models
{
    public class SyncInfo : BaseModel
    {
        public DateTimeOffset? LastSync { get; set; }
    }
}
