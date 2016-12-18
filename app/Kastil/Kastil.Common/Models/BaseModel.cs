
using Newtonsoft.Json;

namespace Kastil.Common.Models
{
    public abstract class BaseModel
    {
        public const string BACKENDLESSCLASSNAME_ATTRIBUTE = "___class";
        public string ObjectId { get; set; }

        [JsonProperty(BACKENDLESSCLASSNAME_ATTRIBUTE)]
        public virtual string ClassName => GetType().Name;
    }

}