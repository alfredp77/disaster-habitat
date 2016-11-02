using System;

namespace Kastil.Common.Models
{
    public class DisasterIncidentAid : BaseModel
    {
        public string DisasterId { get; set; }
        public string Category { get; set; }
        public string DollarValue { get; set; }
        public string DisplayText { get; set; }
    }
}
