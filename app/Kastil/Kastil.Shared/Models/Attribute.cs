namespace Kastil.Shared.Models
{
    public class Attribute : BaseModel
    {
        public string Key { get; set; }
        public string Value { get; set; }
        public string Category { get; set; }
    }
}