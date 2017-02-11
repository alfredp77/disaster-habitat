using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Kastil.Common.Services;

namespace Kastil.PlatformSpecific.Shared
{    
    public class RestServiceCaller : IRestServiceCaller
    {        
        public async Task<string> Get(string url, Dictionary<string, string> headers)
        {
            return await SendRequest(url, headers, "GET");
        }

        public async Task<string> Post(string url, Dictionary<string, string> headers, string payload)
        {
            return await SendRequest(url, headers, "POST", payload);
        }

		public async Task<string> Put(string url, Dictionary<string, string> headers, string payload)
		{
			return await SendRequest(url, headers, "PUT", payload);
		}

        public async Task<string> Delete(string url, Dictionary<string, string> headers)
        {
            return await SendRequest(url, headers, "DELETE");
        }

        private async Task<string> SendRequest(string url, Dictionary<string, string> headers, string method,
            string payload=null)
        {
            var request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = method;
            foreach (var kvp in headers)
            {
                request.Headers[kvp.Key] = kvp.Value;
            }
            

            if (payload != null)
            {
				request.ContentType = "application/json";
                using (var writer = new StreamWriter(request.GetRequestStream()))
                {
                    await writer.WriteAsync(payload);
                }
            }

            try
            {
                using (var response = await request.GetResponseAsync())
                {
                    var httpResponse = (HttpWebResponse)response;
                    return ReadResponse(httpResponse);
                }
            }
            catch (WebException ex)
            {
                return ReadResponse(ex.Response);
            }
        }

        private static string ReadResponse(WebResponse httpResponse)
        {
            try
            {
                using (var reader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    return reader.ReadToEnd();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return null;
            }
            
        }
    }
}
