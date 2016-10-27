using Kastil.Common.Utils;
using MvvmCross.Platform;

namespace Kastil.Shared.Models
{
    public class Attribute : BaseModel
    {
        public string Key { get; set; }
        public string Value { get; set; }
        public string Category { get; set; }
    }

    public class ShelterAttribute : Attribute
    {
    }

    public class AssessmentAttribute : Attribute
    {
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