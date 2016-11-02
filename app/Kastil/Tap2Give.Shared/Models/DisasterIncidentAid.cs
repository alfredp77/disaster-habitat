using System;

namespace Tap2Give.Shared.Models
{
    public class DisasterIncidentAid : BaseModel
    {
        public string DisasterId { get; set; }
        public string Category { get; set; }
        public string DollarValue { get; set; }
        public string DisplayText { get; set; }
    }
}
