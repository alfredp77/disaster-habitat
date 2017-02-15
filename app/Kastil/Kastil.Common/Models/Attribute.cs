using Kastil.Common.Utils;
using MvvmCross.Platform;
using Newtonsoft.Json;

namespace Kastil.Common.Models
{
	public class Attribute : BaseModel
	{
		public string Key { get; set; }

        public string Type { get; set; }
    }

    public abstract class ValuedAttribute : BaseModel
    {
        public string Key { get; set; }
        public string Value { get; set; }
        public virtual string ItemId { get; set; }
    }

    public class ShelterAttribute : ValuedAttribute
    {
        [JsonProperty("ShelterId")]
        public override string ItemId { get; set; }
    }

    public class AssessmentAttribute : ValuedAttribute
    {
        [JsonProperty("AssessmentId")]
        public override string ItemId { get; set; }
    }

    public static class AttributeExtensions
    {
        public static T Convert<T>(this Attribute super) where T : Attribute
        {
            var serializer = Mvx.Resolve<IJsonSerializer>();
            var json = serializer.Serialize(super);
            return serializer.Deserialize<T>(json);
        }
    }
}