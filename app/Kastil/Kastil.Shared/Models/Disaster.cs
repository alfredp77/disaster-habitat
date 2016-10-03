using System;
using Kastil.Core.Utils;
using Newtonsoft.Json;

namespace Kastil.Shared.Models
{
    public class Disaster : BaseModel
    {
        public string Name { get; set; }
        public Location Location { get; set; }
        private long When { get; set; }
        [JsonIgnore]
        public DateTimeOffset? DateWhen
        {
            get { return When.AsDateTimeOffset(); }
			set { When = value.AsUtcTicks(); }
        }
    }
}