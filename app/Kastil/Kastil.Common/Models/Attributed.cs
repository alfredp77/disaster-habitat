namespace Kastil.Common.Models
{
	public abstract class Attributed : BaseModel
    {
        public string DisasterId { get; set; }
        public string Name { get; set; }
        public string Location { get; set; }
    }
}
