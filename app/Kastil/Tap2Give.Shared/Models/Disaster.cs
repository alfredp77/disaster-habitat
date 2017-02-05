using Newtonsoft.Json;
using System;
using Tap2Give.Core.Utils;

namespace Tap2Give.Shared.Models
{
    public class Disaster : BaseModel
    {
        public string Name { get; set; }
        public string Location { get; set; }
        private long When { get; set; }
        public string Description { get; set; }
        public string GiveUrl { get; set; }
        public string ImageUrl { get; set; }

        [JsonIgnore]
        public DateTimeOffset? DateWhen
        {
            get { return When.AsDateTimeOffset(); }
            set { When = value.AsUtcTicks(); }
        }
    }
}
