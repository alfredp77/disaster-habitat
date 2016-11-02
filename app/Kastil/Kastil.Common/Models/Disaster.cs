using System;
using Kastil.Common.Utils;
using Newtonsoft.Json;

namespace Kastil.Common.Models
{
    public class Disaster : BaseModel
    {
        public string Name { get; set; }
        public string Location { get; set; }
        private long When { get; set; }
        public string Description { get; set; }
        public string GiveUrl { get; set; }
        public string ImageUrl { get; set; }
        //public string LocationName { get { return Location != null ? Location.Name + ", " + Location.Country : "Unknown location"; } }

        [JsonIgnore]
        public DateTimeOffset? DateWhen
        {
            get { return When.AsDateTimeOffset(); }
			set { When = value.AsUtcTicks(); }
        }
    }
}