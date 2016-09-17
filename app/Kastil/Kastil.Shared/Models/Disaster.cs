using System;

namespace Kastil.Shared.Models
{
    public class Disaster : BaseModel
    {
        public string Name { get; set; }
        public Location Location { get; set; }
        public DateTimeOffset When { get; set; }
    }
}