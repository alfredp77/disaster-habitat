using System.Collections.Generic;

namespace Kastil.Common.Models
{
    public abstract class Attributed : BaseModel
    {
        public string DisasterId { get; set; }
        public string Name { get; set; }
        public string LocationName { get; set; }
        public List<Attribute> Attributes { get; set; } = new List<Attribute>();
    }
}
