using Kastil.Common.Models;

namespace Kastil.Common.Utils
{
    public class BackendlessResponseParser
    {
        private readonly IJsonSerializer _serializer;

        public BackendlessResponseParser(IJsonSerializer serializer)
        {
            _serializer = serializer;
        }

        public BackendlessResponse<T> Parse<T>(string response) where T : BaseModel
        {
            if (string.IsNullOrEmpty(response))
            {
                return BackendlessResponse<T>.Failed();
            }

            var content = _serializer.Deserialize<T>(response);
            if (!string.IsNullOrEmpty(content.ObjectId))
            {
                return BackendlessResponse<T>.Success(content);
            }

            var kvp = _serializer.AsDictionary(response);
            string code, message;
            kvp.TryGetValue("code", out code);
            kvp.TryGetValue("message", out message);
            return BackendlessResponse<T>.Failed(code, message);
        }
    }
}