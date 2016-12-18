using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Kastil.Common.Models
{
	public abstract class Attributed : BaseModel
    {
        public string DisasterId { get; set; }
        public string Name { get; set; }
        public string Location { get; set; }

        [JsonIgnore]
        public List<Attribute> Attributes { get; set; } = new List<Attribute>();
	}

    
}
