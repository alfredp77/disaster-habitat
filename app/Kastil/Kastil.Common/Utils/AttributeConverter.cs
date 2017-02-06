using Kastil.Common.Models;

namespace Kastil.Common.Utils
{
    public static class AttributeConverter
    {
        public static T CreateValuedAttribute<T>(this Attribute attribute) where T : ValuedAttribute, new()
        {
            return new T
            {
                Key = attribute.Key
            };
        }

        public static Attribute AsBaseAttribute(this ValuedAttribute valuedAttribute)
        {
            return new Attribute {Key = valuedAttribute.Key};
        }
    }
}