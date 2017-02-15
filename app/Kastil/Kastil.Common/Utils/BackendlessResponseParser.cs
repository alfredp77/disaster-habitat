using System.Linq;
using Kastil.Common.Models;

namespace Kastil.Common.Utils
{
    public class BackendlessResponseParser : IBackendlessResponseParser
    {
        private readonly IJsonSerializer _serializer;

        public BackendlessResponseParser(IJsonSerializer serializer)
        {
            _serializer = serializer;
        }

        public BackendlessResponse<T> Parse<T>(string response) where T : BaseModel
        {
            var backendlessResponse = ParseErrorResponse<T>(response);
            if (backendlessResponse != null)
                return backendlessResponse;

            var content = _serializer.Deserialize<T>(response);
            if (!string.IsNullOrEmpty(content.ObjectId))
            {
                return BackendlessResponse<T>.Success(content);
            }

            return BackendlessResponse<T>.Failed();
        }

        private BackendlessResponse<T> ParseErrorResponse<T>(string response) where T : BaseModel
        {
            if (string.IsNullOrEmpty(response))
            {
                return BackendlessResponse<T>.Failed();
            }

            var kvp = _serializer.AsDictionary(response);
            string code, message;
            kvp.TryGetValue("code", out code);
            kvp.TryGetValue("message", out message);
            if (!string.IsNullOrEmpty(code) || !string.IsNullOrEmpty(message))
                return BackendlessResponse<T>.Failed(code, message);
            return null;
        }

        public BackendlessResponse<T> ParseArray<T>(string response) where T : BaseModel
        {
            var backendlessResponse = ParseErrorResponse<T>(response);
            if (backendlessResponse != null)
                return backendlessResponse;

            var docs = _serializer.ParseAsObjectArray<T>(response, "data", "objectId");
            return BackendlessResponse<T>.Success(docs.ToArray());
        }
    }
}