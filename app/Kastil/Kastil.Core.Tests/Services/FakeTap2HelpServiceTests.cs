using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Kastil.Core.Fakes;
using Kastil.Core.Services;
using Kastil.Core.Utils;
using Kastil.Shared.Models;
using Newtonsoft.Json.Linq;
using NUnit.Core;
using NUnit.Framework;

namespace Kastil.Core.Tests.Services
{

    [TestFixture]
    public class FakeTap2HelpServiceTests
    {
        private FakeTap2HelpService _service;

        [SetUp]
        public void SetUp()
        {
            _service = new FakeTap2HelpService();
        }

        [Test]
        public async Task GetDisastersIsFine()
        {
            var disasters = await _service.GetDisasters();
            Assert.That(disasters.Any());
        }

        [Test]
        public async Task AttributesTest()
        {
            var assesments = await _service.GetAssesments();
            var first = assesments.First();
            Assert.That(first.Attributes.FirstOrDefault(), Is.Not.Null);
        }

        [Test]
        public async Task GetAllAssesmentIsWorking()
        {
            var assesments = await _service.GetAssesments();
            Assert.That(assesments.Any());
        }

        [Test]
        public async Task GetAllAssesmentsForEachDisaster()
        {
            var disasters = await _service.GetDisasters();
            foreach(var disaster in disasters)
            {
                var assesments = await _service.GetAssesments(disaster.Id);
                Assert.That(assesments.Any());
                Assert.True(assesments.All(a => a.DisasterId.Equals(disaster.Id)));
            }
        }


    }


    [TestFixture]
    public class TryCallingBackendlessTests
    {
        private static string APPID_HEADER = "application-id";
        private static string SECRET_KEY_HEADER = "secret-key";
        private static string APPID_VALUE = "4ED00D7B-240E-B654-FF92-8E5E8C6F0100";
        private static string SECRET_KEY_VALUE = "9FA43F77-53DA-2D88-FF36-44DFC400C600";
        private static string BASE_URL = "https://api.backendless.com/v1/";

        private static Dictionary<string, string> _headers = new Dictionary<string, string>
        {
            {APPID_HEADER,APPID_VALUE },
            {SECRET_KEY_HEADER,SECRET_KEY_VALUE },
        };

        [Test]
        public async Task CallTest()
        {
            var caller = new RestServiceCaller();
            var result = await caller.Get("https://api.backendless.com/v1/data/Disaster", _headers);
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

    public class RestServiceCaller
    {
        //Map persistableMap = new PersistableMap()
        public async Task<string> Get(string url, Dictionary<string, string> headers)
        {
            var request = (HttpWebRequest)WebRequest.Create(url);
            //request.ContentType = chunk.ContentType;
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
