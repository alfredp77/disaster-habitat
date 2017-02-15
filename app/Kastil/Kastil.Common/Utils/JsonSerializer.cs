using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Kastil.Common.Utils
{
    public interface IJsonSerializer
    {
        T Deserialize<T>(string json);
        string Serialize(object o);

        IEnumerable<KeyValuePair<string, string>> ParseArray(string json, string arrayPropertyName,
            string idPropertyName);

        IEnumerable<T> ParseAsObjectArray<T>(string json, string arrayPropertyName,
            string idPropertyName, Action<T> patch=null);

       T Clone<T>(T o);

        Dictionary<string, string> AsDictionary(string json);
    }

    public class JsonSerializer : IJsonSerializer
    {
        private readonly JsonSerializerSettings _readerSettings;
        private readonly JsonSerializerSettings _writerSettings;

        public JsonSerializer(params JsonConverter[] readers)
        {
            _readerSettings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                Converters = readers.ToList()
            };
            _writerSettings = new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore };
        }

        public T Deserialize<T>(string json)
        {
            return JsonConvert.DeserializeObject<T>(json, _readerSettings);
        }

        public string Serialize(object o)
        {
            return JsonConvert.SerializeObject(o, _writerSettings);
        }

        public T Clone<T>(T o)
        {
            var json = Serialize(o);
            return Deserialize<T>(json);
        }        

        public IEnumerable<KeyValuePair<string, string>> ParseArray(string json, string arrayPropertyName, string idPropertyName)
        {
            if (string.IsNullOrEmpty(json))
                yield break;

            var token = JToken.Parse(json);
            var enumerable = Enumerable.Empty<JToken>();

            if (token is JObject && !string.IsNullOrEmpty(arrayPropertyName))
            {
                var obj = (JObject)token;
                var dataProp = obj.Properties().FirstOrDefault(p => p.Name == arrayPropertyName);
                if (dataProp == null)
                    enumerable = new List<JToken> { obj };
                else
                    enumerable = dataProp.Value.Children();

            }
            else if (token is JArray)
            {
                enumerable = (JArray)token;
            }

            foreach (var x in enumerable)
            {
                var id = x.Children<JProperty>().FirstOrDefault(c => c.Name == idPropertyName)?.Value.ToString() ?? $"NOID-{Guid.NewGuid()}";
                yield return new KeyValuePair<string, string>(id, x.ToString());
            }
        }


        public IEnumerable<T> ParseAsObjectArray<T>(string json, string arrayPropertyName, string idPropertyName, Action<T> patch=null)
        {
            patch = patch ?? (_ => { });
            return ParseArray(json, arrayPropertyName, idPropertyName)
                .Select(keyValuePair =>
                {
                    var result = Deserialize<T>(keyValuePair.Value);
                    patch(result);
                    return result;
                });
        }

        public Dictionary<string, string> AsDictionary(string json)
        {
            var obj = JObject.Parse(json);
            return obj.Properties().ToDictionary(p => p.Name, p => p.Value.ToString());
        } 
    }
}
