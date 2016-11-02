using System.Collections.Generic;
using Kastil.Common.Models;

namespace Kastil.Common
{
    public partial class Connection
    {
        public string AppId { get; set; }
        public string SecretKey { get; set; }

        private static string APPID_HEADER = "application-id";
        private static string SECRET_KEY_HEADER = "secret-key";
        private static string BASE_URL = "https://api.backendless.com/v1";

        private Dictionary<string, string> _headers; 
        public Dictionary<string, string> Headers => _headers ?? (_headers = new Dictionary<string, string>
        {
            {APPID_HEADER, AppId},
            {SECRET_KEY_HEADER, SecretKey},
        });

        public static string GenerateGetUrl<T>() where T : BaseModel
        {
            return $"{BASE_URL}/data/{typeof(T).Name}";
        }
    }
}
