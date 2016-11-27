using System.Collections.Generic;
using Newtonsoft.Json;

namespace Kastil.Common.Models
{
    public abstract class Attributed : BaseModel
    {
        [JsonProperty("ownerId")]
        public string DisasterId { get; set; }
        public string Name { get; set; }
        public string LocationName { get; set; }
        public List<Attribute> Attributes { get; set; } = new List<Attribute>();
    }
}
