using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kastil.Common;
using Kastil.Common.Services;
using Kastil.Common.Utils;
using Kastil.TestUtils;
using Moq;
using NUnit.Framework;

namespace Kastil.Core.Tests.Services
{
    [TestFixture]
    public class PullServiceTests : BaseTest
    {
        private IPullService _service;
        private Connection _connection;
        private Mock<IRestServiceCaller> _restServiceCaller;
        private Mock<IPersistenceContextFactory> _persistenceContextFactory;
        private Mock<IPersistenceContext<TestModel>> _persistenceContext;
        private Mock<IBackendlessResponseParser> _responseParser;
        private Mock<IJsonSerializer> _serializer;

        private string _json = "the json returned by backendless";
        private List<KeyValuePair<string, string>> _kvps;
        public override void CreateTestableObject()
        {
            _service = new PullService();
            _connection = new Connection {AppId = "foo", SecretKey = "bar"};

            _restServiceCaller = CreateMock<IRestServiceCaller>();
            _responseParser = CreateMock<IBackendlessResponseParser>();
            _persistenceContextFactory = CreateMock<IPersistenceContextFactory>();
            _persistenceContext = CreateMock<IPersistenceContext<TestModel>>();
            _persistenceContextFactory.Setup(f => f.CreateFor<TestModel>()).Returns(_persistenceContext.Object);
            _serializer = CreateMock<IJsonSerializer>();

            _restServiceCaller.Setup(c => c.Get(Connection.GenerateTableUrl<TestModel>(""), _connection.Headers))
                .ReturnsAsync(_json);

            _kvps = new List<KeyValuePair<string, string>>();
            _serializer.Setup(s => s.ParseArray(_json, "data", "objectId"))
                .Returns(_kvps);

            Ioc.RegisterSingleton(_connection);
            Ioc.RegisterSingleton(_restServiceCaller.Object);
            Ioc.RegisterSingleton(_persistenceContextFactory.Object);
            Ioc.RegisterSingleton(_serializer.Object);
            Ioc.RegisterSingleton(_responseParser.Object);
        }

        private TestModel PrepareTestModel()
        {
            var tm = new TestModel();
            _kvps.Add(new KeyValuePair<string, string>("1", "blah"));
            _serializer.Setup(s => s.Deserialize<TestModel>("blah")).Returns(tm);
            return tm;
        }

        [Test]
        public async Task Should_Return_Data_Without_Persisting()
        {
            var tm = PrepareTestModel();

            var result = await _service.Pull<TestModel>(persist:false);

            var successful = result.SuccessfulItems.ToList();
            Assert.That(successful.Count, Is.EqualTo(1));
            Assert.That(successful[0], Is.EqualTo(tm));
            Assert.That(result.FailedItems, Is.Empty);
            _persistenceContext.Verify(c => c.PersistAllJson(_kvps), Times.Never);
            _persistenceContext.Verify(c => c.PurgeAll(), Times.Never);
        }

        [Test]
        public async Task Should_Persist_All_Json_And_Wipe_Existing_Data()
        {
            var tm = PrepareTestModel();
            var existing = new TestModel {ObjectId = "xxx"};
            _persistenceContext.Setup(c => c.LoadAll()).Returns(new[] {existing});

            var result = await _service.Pull<TestModel>();

            var successful = result.SuccessfulItems.ToList();
            Assert.That(successful.Count, Is.EqualTo(1));
            Assert.That(successful[0], Is.EqualTo(tm));
            Assert.That(result.FailedItems, Is.Empty);
            _persistenceContext.Verify(c => c.Purge(existing.ObjectId), Times.Once);
            _persistenceContext.Verify(c => c.PersistAllJson(_kvps), Times.Once);
        }

        [Test]
        public async Task Should_Not_Wipe_New_Data()
        {
            var tm = PrepareTestModel();
            var existing = new TestModel();
            existing.StampNewId();
            _persistenceContext.Setup(c => c.LoadAll()).Returns(new[] { existing });

            var result = (await _service.Pull<TestModel>());

            var successful = result.SuccessfulItems.ToList();
            Assert.That(successful.Count, Is.EqualTo(1));
            Assert.That(successful[0], Is.EqualTo(tm));
            Assert.That(result.FailedItems, Is.Empty);
            _persistenceContext.Verify(c => c.Purge(existing.ObjectId), Times.Never);
            _persistenceContext.Verify(c => c.PersistAllJson(_kvps), Times.Once);
        }
    }
}