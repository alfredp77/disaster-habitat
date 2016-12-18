using Kastil.Common.Utils;
using MvvmCross.Platform;
using Newtonsoft.Json;

namespace Kastil.Common.Models
{
	public class Attribute : BaseModel
	{
	    public const string BACKENDLESSNAME = "Attributes";

		public string Key { get; set; }

        public string Value { get; set; }

		[JsonIgnore]
        public string Category { get; set; }

        [JsonProperty(BACKENDLESSCLASSNAME_ATTRIBUTE)]
        public override string ClassName => BACKENDLESSNAME;
    }

    public class ShelterAttribute : Attribute
    {
        public new const string BACKENDLESSNAME = "ShelterAttribute";

        [JsonProperty(BACKENDLESSCLASSNAME_ATTRIBUTE)]
		public override string ClassName => BACKENDLESSNAME;
    }

    public class AssessmentAttribute : Attribute
    {
        public new const string BACKENDLESSNAME = "AssesmentAttribute";

        [JsonProperty(BACKENDLESSCLASSNAME_ATTRIBUTE)]
        public override string ClassName => BACKENDLESSNAME;
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