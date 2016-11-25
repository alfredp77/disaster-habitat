using Newtonsoft.Json;

namespace Kastil.Common.Models
{
    public class User : BaseModel
    {
        public string StaffCode { get; set; }
        
		public string Email { get; set; }
		public string Name { get; set; }

		[JsonProperty("user-token")]
		public string Token { get; set; }
    }
}
