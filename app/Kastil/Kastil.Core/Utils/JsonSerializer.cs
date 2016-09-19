using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Kastil.Core.Utils
{
    public interface IJsonSerializer
    {
        T Deserialize<T>(string json);
        string Serialize(object o);

        IEnumerable<KeyValuePair<string, string>> ParseArray(string json, string arrayPropertyName,
            string idPropertyName);

        T Clone<T>(T o);        
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

        public IEnumerable<KeyValuePair<string, string>> ParseArray(string json, string arrayPropertyName, string idPropertyName)
        {
            var obj = JObject.Parse(json);
            var dataProp = obj.Properties().FirstOrDefault(p => p.Name == arrayPropertyName);
            if (dataProp == null)
                yield break;

            foreach (var x in dataProp.Value.Children())
            {
                var id = x.Children<JProperty>().FirstOrDefault(c => c.Name == idPropertyName);
                if (id != null)
                {
                    yield return new KeyValuePair<string, string>(id.Value.ToString(), x.ToString());
                }
            }
        }
    }
}
