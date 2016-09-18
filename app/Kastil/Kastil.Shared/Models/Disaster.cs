using System;
using Newtonsoft.Json;

namespace Kastil.Shared.Models
{
    public class Disaster : BaseModel
    {
        public string Name { get; set; }
        public string Location { get; set; }
        private long When { get; set; }
        [JsonIgnore]
        public DateTimeOffset DateWhen
        {
            get
            {
                if (When == 0)
                    return DateTimeOffset.UtcNow;
                return new DateTimeOffset(When, TimeSpan.Zero);
            }
            set { When = value.UtcTicks; }
        }
    }
}