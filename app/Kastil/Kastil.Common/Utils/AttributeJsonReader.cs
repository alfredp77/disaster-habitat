using System;
using Kastil.Common.Models;
using MvvmCross.Platform;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Attribute = Kastil.Common.Models.Attribute;

namespace Kastil.Common.Utils
{
    public class AttributeJsonReader : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return typeof(Attribute).IsAssignableFrom(objectType);
        }

        public override object ReadJson(JsonReader reader, Type objectType,
            object existingValue, Newtonsoft.Json.JsonSerializer serializer)
        {
            var jObject = JObject.Load(reader);
            var className = (string)jObject[BaseModel.BACKENDLESSCLASSNAME_ATTRIBUTE];

            Attribute target;
            if (objectType == typeof (AssessmentAttribute))
            {
                target = new AssessmentAttribute();
            }
            else if (objectType == typeof (ShelterAttribute) || ShelterAttribute.BACKENDLESSNAME.Equals(className, StringComparison.CurrentCultureIgnoreCase))
            {
                target = new ShelterAttribute();
            }
			else if (AssessmentAttribute.BACKENDLESSNAME.Equals(className, StringComparison.CurrentCultureIgnoreCase))
            {
                target = new AssessmentAttribute();
            }
			else if (ShelterAttribute.BACKENDLESSNAME.Equals(className, StringComparison.CurrentCultureIgnoreCase))
			{
			    target = new ShelterAttribute();
			}
			else
			{
			    target = new Attribute();
			}
            serializer.Populate(jObject.CreateReader(), target);
            return target;
        }

        public override void WriteJson(JsonWriter writer, object value,
            Newtonsoft.Json.JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}