using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kastil.Common;
using Kastil.Common.Services;
using Kastil.Common.Utils;
using Kastil.TestUtils;
using Moq;
using NSubstitute;
using NUnit.Framework;

namespace Kastil.Core.Tests.Services
{
    [TestFixture]
    public class PullServiceTests : BaseTest
    {
        private IPullService _service;
        private Connection _connection;
        private IRestServiceCaller _restServiceCaller;
        private IPersistenceContextFactory _persistenceContextFactory;
        private IPersistenceContext<TestModel> _persistenceContext;
        private IBackendlessResponseParser _responseParser;

        private const string Json = "the json returned by backendless";
        private List<KeyValuePair<string, string>> _kvps;
        public override void CreateTestableObject()
        {
            _service = new PullService();
            _connection = new Connection {AppId = "foo", SecretKey = "bar"};

            _restServiceCaller = Substitute.For<IRestServiceCaller>();
            _responseParser = Substitute.For<IBackendlessResponseParser>();
            _persistenceContextFactory = Substitute.For<IPersistenceContextFactory>();
            _persistenceContext = Substitute.For<IPersistenceContext<TestModel>>();
            _persistenceContextFactory.CreateFor<TestModel>().Returns(_persistenceContext);

            _restServiceCaller.Get(Connection.GenerateTableUrl<TestModel>(), _connection.Headers).Returns(Task.FromResult(Json));

            Ioc.RegisterSingleton(_connection);
            Ioc.RegisterSingleton(_restServiceCaller);
            Ioc.RegisterSingleton(_persistenceContextFactory);
            Ioc.RegisterSingleton(_responseParser);
        }

        private static TestModel CreateTestModel()
        {
            return new TestModel { ObjectId = Guid.NewGuid().ToString() };
        }

        private BackendlessResponse<TestModel> SetupSuccessfulPull(params TestModel[] contents)
        {            
            var response = BackendlessResponse<TestModel>.Success(contents);
            _responseParser.ParseArray<TestModel>(Json).Returns(response);
            return response;
        }

        [Test]
        public async Task Should_Persist_Successful_Items()
        {
            var tm1 = CreateTestModel();
            var tm2 = CreateTestModel();
            SetupSuccessfulPull(tm1, tm2);

            await _service.Pull<TestModel>();

            _persistenceContext.Received().Save(tm1);
            _persistenceContext.Received().Save(tm2);
        }        

        [Test]
        public async Task Should_Return_Successful_Items()
        {
            var tm1 = CreateTestModel();
            var tm2 = CreateTestModel();
            SetupSuccessfulPull(tm1, tm2);

            var result = await _service.Pull<TestModel>();

            var success = result.SuccessfulItems.ToList();
            Assert.That(success.Count, Is.EqualTo(2));
            Assert.That(success.Contains(tm1));
            Assert.That(success.Contains(tm2));
            Assert.That(result.FailedItems, Is.Empty);
        }

        [Test]
        public async Task Should_Clear_All_Non_New_Items_When_Pull_Is_Successful()
        {
            var existing = CreateTestModel();
            var newItem = new TestModel();
            newItem.StampNewId();
            _persistenceContext.LoadAll().Returns(new[] {existing, newItem});

            var pulled = CreateTestModel();
            SetupSuccessfulPull(pulled);

            await _service.Pull<TestModel>();

            _persistenceContext.Received().Purge(existing.ObjectId);
            _persistenceContext.DidNotReceive().Purge(newItem.ObjectId);
        }

        [Test]
        public async Task Should_Not_Persist_When_Specified()
        {
            var tm1 = CreateTestModel();
            var tm2 = CreateTestModel();
            SetupSuccessfulPull(tm1, tm2);

            var result = await _service.Pull<TestModel>(persist:false);

            _persistenceContext.DidNotReceiveWithAnyArgs().Save(null);
            _persistenceContext.DidNotReceiveWithAnyArgs().Purge("");
            var success = result.SuccessfulItems.ToList();
            Assert.That(success.Count, Is.EqualTo(2));
            Assert.That(success.Contains(tm1));
            Assert.That(success.Contains(tm2));
            Assert.That(result.FailedItems, Is.Empty);
        }

        [Test]
        public async Task Should_Not_Purge_Existing_Items()
        {
            var pulled = CreateTestModel();
            var existing = CreateTestModel();
            existing.ObjectId = pulled.ObjectId;
            var anotherOne = CreateTestModel();

            _persistenceContext.LoadAll().Returns(new[] { existing, anotherOne });

            SetupSuccessfulPull(pulled);

            await _service.Pull<TestModel>();

            _persistenceContext.Received().Purge(anotherOne.ObjectId);
            _persistenceContext.DidNotReceive().Purge(existing.ObjectId);           
        }

    }
}