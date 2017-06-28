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
		private static string APPLICATION_TYPE_HEADER = "application-type";
		private static string REST = "REST";
		private static string CONTENT_TYPE_HEADER = "Content-Type";
		private static string CONTENT_TYPE = "application/json";
#if DEBUG
		private static string BASE_URL = "https://api.backendless.com/v1";
		private static string BASE_NON_HTTPS_URL = "http://api.backendless.com/v1";
#else
		private static string BASE_URL = "https://api.backendless.com/vprod";
		private static string BASE_NON_HTTPS_URL = "http://api.backendless.com/vprod";
#endif
		private static string USER_TOKEN = "user-token";

        private Dictionary<string, string> _headers; 
        public Dictionary<string, string> Headers => _headers ?? (_headers = new Dictionary<string, string>
        {
            {APPID_HEADER, AppId},
            {SECRET_KEY_HEADER, SecretKey},
            {APPLICATION_TYPE_HEADER, REST}
        });

        public static string GenerateTableUrl<T>(string objectId="") where T : BaseModel
        {
            var objectIdParam = string.IsNullOrEmpty(objectId) ? "" : $"/{objectId}";
            return $"{BASE_NON_HTTPS_URL}/data/{typeof(T).Name}{objectIdParam}";
        }
        
		public static string LoginUrl => $"{BASE_URL}/users/login";
    }
}
