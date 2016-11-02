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
            var request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "GET";
            foreach (var kvp in headers)
            {
                request.Headers[kvp.Key] = kvp.Value;
            }

            try
            {
                using (var response = await request.GetResponseAsync())
                {
                    var httpResponse = (HttpWebResponse)response;
                    if (httpResponse.StatusCode == HttpStatusCode.OK)
                    {
                        using (var reader = new StreamReader(httpResponse.GetResponseStream()))
                        {
                            return reader.ReadToEnd();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            return "{}";
        }
    }
}
