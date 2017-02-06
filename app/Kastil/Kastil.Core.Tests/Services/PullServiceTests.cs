using System.Collections.Generic;
using System.Threading.Tasks;
using Kastil.Common;
using Kastil.Common.Services;
using Kastil.Common.Utils;
using Kastil.Core.Services;
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
        private Mock<IJsonSerializer> _serializer;

        private string _json = "the json returned by backendless";
        private List<KeyValuePair<string, string>> _kvps = new List<KeyValuePair<string, string>>();
        public override void CreateTestableObject()
        {
            _service = new PullService();
            _connection = new Connection {AppId = "foo", SecretKey = "bar"};

            _restServiceCaller = CreateMock<IRestServiceCaller>();
            _persistenceContextFactory = CreateMock<IPersistenceContextFactory>();
            _persistenceContext = CreateMock<IPersistenceContext<TestModel>>();
            _persistenceContextFactory.Setup(f => f.CreateFor<TestModel>()).Returns(_persistenceContext.Object);
            _serializer = CreateMock<IJsonSerializer>();

            _restServiceCaller.Setup(c => c.Get(Connection.GenerateTableUrl<TestModel>(), _connection.Headers))
                .ReturnsAsync(_json);
            _serializer.Setup(s => s.ParseArray(_json, "data", "id"))
                .Returns(_kvps);

            Ioc.RegisterSingleton(_connection);
            Ioc.RegisterSingleton(_restServiceCaller.Object);
            Ioc.RegisterSingleton(_persistenceContextFactory.Object);
            Ioc.RegisterSingleton(_serializer.Object);
        }

        [Test]
        public async Task Should_Persist_All_Json_Without_Wiping_Existing_Data()
        {            
            await _service.Pull<TestModel>();
            _persistenceContext.Verify(c => c.PersistAllJson(_kvps), Times.Once);
            _persistenceContext.Verify(c => c.DeleteAll(), Times.Never);
        }

        [Test]
        public async Task Should_Persist_All_Json_And_Wipe_Existing_Data()
        {
            await _service.Pull<TestModel>();
            _persistenceContext.Verify(c => c.PersistAllJson(_kvps), Times.Once);
            _persistenceContext.Verify(c => c.DeleteAll(), Times.Once);
        }
    }
}