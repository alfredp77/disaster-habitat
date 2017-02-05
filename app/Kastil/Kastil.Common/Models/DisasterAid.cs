using System;

namespace Kastil.Common.Models
{
    public class DisasterAid : BaseModel
    {
        public string DisasterId { get; set; }
        public string DollarValue { get; set; }
        public string Description { get; set; }
    }
}
