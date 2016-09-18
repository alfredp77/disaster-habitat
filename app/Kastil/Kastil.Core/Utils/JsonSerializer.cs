using System.Collections;
using Newtonsoft.Json;

namespace Kastil.Core.Utils
{
    public interface IJsonSerializer
    {
        T Deserialize<T>(string json);
        string Serialize(object o);        
    }

    public class JsonSerializer : IJsonSerializer
    {
        public T Deserialize<T>(string json)
        {
            return JsonConvert.DeserializeObject<T>(json);
        }

        public string Serialize(object o)
        {
            return JsonConvert.SerializeObject(o);
        }

        public T Clone<T>(T o)
        {
            var json = Serialize(o);
            return Deserialize<T>(json);
        }
    }
}
