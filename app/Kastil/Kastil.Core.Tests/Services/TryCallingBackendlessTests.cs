using System;
using System.Linq;
using System.Threading.Tasks;
using Kastil.Common;
using Kastil.PlatformSpecific.Shared;
using Newtonsoft.Json.Linq;
using NUnit.Framework;

namespace Kastil.Core.Tests.Services
{
    [TestFixture]
    [Explicit("Run this manually to try out backendless service")]
    public class TryCallingBackendlessTests
    {
        [Test]
        public async Task CallTest()
        {
            var connection = new Connection();
            var caller = new RestServiceCaller();
            var result = await caller.Get("https://api.backendless.com/v1/data/Disaster", connection.Headers);
            var obj = JObject.Parse(result);

            var dataProp = obj.Properties().FirstOrDefault(p => p.Name == "data");
            if (dataProp != null)
            {
                foreach (var x in dataProp.Value.Children())
                {
                    var id = x.Children<JProperty>().FirstOrDefault(c => c.Name == "id");
                    if (id != null)
                    {
                        Console.WriteLine(id.Value + " " + x);
                    }
                }
            }
            else
            {
                Console.WriteLine("Didn't get anything back!");
            }
        }
    }
}