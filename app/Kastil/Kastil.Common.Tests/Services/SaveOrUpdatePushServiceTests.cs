using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kastil.Common.Models;
using Kastil.Common.Services;
using Kastil.Common.Utils;
using Kastil.TestUtils;
using NSubstitute;
using NUnit.Framework;

namespace Kastil.Common.Tests.Services
{
    [TestFixture]
    public class SaveOrUpdatePushServiceTests : BaseTest
    {
        private SaveOrUpdatePushService _service;
        private IJsonSerializer _jsonSerializer;
        private IPersistenceContextFactory _persistenceContextFactory;
        private IPersistenceContext<Assessment> _persistenceContext;
        private IBackendlessResponseParser _responseParser;
        private IRestServiceCaller _restServiceCaller;
        private readonly Connection _connection = new Connection();
        private const string UserToken = "BLAH";

        public override void CreateTestableObject()
        {
            _jsonSerializer = Substitute.For<IJsonSerializer>();
            _persistenceContextFactory = Substitute.For<IPersistenceContextFactory>();
            _persistenceContext = Substitute.For<IPersistenceContext<Assessment>>();
            _responseParser = Substitute.For<IBackendlessResponseParser>();
            _restServiceCaller = Substitute.For<IRestServiceCaller>();

            _persistenceContextFactory.CreateFor<Assessment>().Returns(_persistenceContext);

            Ioc.RegisterSingleton(_jsonSerializer);
            Ioc.RegisterSingleton(_persistenceContextFactory);
            Ioc.RegisterSingleton(_responseParser);
            Ioc.RegisterSingleton(_restServiceCaller);
            Ioc.RegisterSingleton(_connection);

            _service = new SaveOrUpdatePushService();
        }

        private static Assessment CreateSavedAssessment()
        {
            return new Assessment { ObjectId = Guid.NewGuid().ToString() };
        }

        private static Assessment CreateLocalAssessment()
        {
            var localAssessment = new Assessment();
            localAssessment.StampNewId();
            return localAssessment;
        }

        private Assessment SetupSuccessfulPost(Assessment localAssessment)
        {
            var clonedLocalAssessment = new Assessment { ObjectId = localAssessment.ObjectId };
            var savedAssessment = CreateSavedAssessment();
            var url = Connection.GenerateTableUrl<Assessment>();
            _jsonSerializer.Clone(localAssessment).Returns(clonedLocalAssessment);
            var serializedObject = $"{savedAssessment.ObjectId}-serialized";
            _jsonSerializer.Serialize(clonedLocalAssessment).Returns(serializedObject);
            var jsonResponse = $"{savedAssessment.ObjectId}-response";
            _restServiceCaller.Post(url, Arg.Any<Dictionary<string, string>>(), serializedObject)
                .Returns(Task.FromResult(jsonResponse));
            _responseParser.Parse<Assessment>(jsonResponse).Returns(BackendlessResponse<Assessment>.Success(savedAssessment));
            return savedAssessment;
        }

        private BackendlessResponse<Assessment> SetupFailedPost(Assessment localAssessment, string errorMessage)
        {
            var clonedLocalAssessment = new Assessment { ObjectId = localAssessment.ObjectId };
            var savedAssessment = CreateSavedAssessment();
            var url = Connection.GenerateTableUrl<Assessment>();
            _jsonSerializer.Clone(localAssessment).Returns(clonedLocalAssessment);
            var serializedObject = $"{savedAssessment.ObjectId}-serialized";
            _jsonSerializer.Serialize(clonedLocalAssessment).Returns(serializedObject);
            var jsonResponse = $"{savedAssessment.ObjectId}-failedResponse";
            _restServiceCaller.Post(url, Arg.Any<Dictionary<string, string>>(), serializedObject)
                .Returns(Task.FromResult(jsonResponse));
            var backendlessResponse = BackendlessResponse<Assessment>.Failed("error", errorMessage);
            _responseParser.Parse<Assessment>(jsonResponse).Returns(backendlessResponse);
            return backendlessResponse;
        }

        private Assessment SetupSuccessfulPut(Assessment localAssessment)
        {
            var clonedLocalAssessment = new Assessment { ObjectId = localAssessment.ObjectId };
            var savedAssessment = new Assessment { ObjectId = localAssessment.ObjectId };
            var url = Connection.GenerateTableUrl<Assessment>(localAssessment.ObjectId);
            _jsonSerializer.Clone(localAssessment).Returns(clonedLocalAssessment);
            var serializedObject = $"{savedAssessment.ObjectId}-serialized";
            _jsonSerializer.Serialize(clonedLocalAssessment).Returns(serializedObject);
            var jsonResponse = $"{savedAssessment.ObjectId}-response";
            _restServiceCaller.Put(url, Arg.Any<Dictionary<string, string>>(), serializedObject)
                .Returns(Task.FromResult(jsonResponse));
            _responseParser.Parse<Assessment>(jsonResponse).Returns(BackendlessResponse<Assessment>.Success(savedAssessment));
            return savedAssessment;
        }

        [Test]
        public async Task Should_Post_New_Items()
        {
            var localAssessment = CreateLocalAssessment();
            var savedAssessment = SetupSuccessfulPost(localAssessment);
            _persistenceContext.LoadAll().Returns(new[] { localAssessment });

            var result = await _service.Push<Assessment>(UserToken);

            var items = result.SuccessfulItems.ToList();
            Assert.That(items.Count, Is.EqualTo(1));
            Assert.That(items.Contains(savedAssessment));
            Assert.That(result.FailedItems, Is.Empty);
            _persistenceContext.Received().Save(savedAssessment);
            _persistenceContext.Received().Purge(localAssessment.ObjectId);
        }

        [Test]
        public async Task Should_Update_Existing_Items()
        {
            var localAssessment = CreateSavedAssessment();
            var savedAssessment = SetupSuccessfulPut(localAssessment);
            _persistenceContext.LoadAll().Returns(new[] { localAssessment });

            var result = await _service.Push<Assessment>(UserToken);

            var items = result.SuccessfulItems.ToList();
            Assert.That(items.Count, Is.EqualTo(1));
            Assert.That(items.Contains(savedAssessment));
            Assert.That(result.FailedItems, Is.Empty);
            _persistenceContext.Received().Save(savedAssessment);
            _persistenceContext.DidNotReceive().Purge(localAssessment.ObjectId);
        }

        [Test]
        public async Task Should_Only_Saved_Those_Matches_Supplied_Predicate()
        {
            var localAssessment1 = CreateLocalAssessment();
            var localAssessment2 = CreateLocalAssessment();
            var localAssessment3 = CreateLocalAssessment();
            var savedAssessment1 = SetupSuccessfulPost(localAssessment1);
            var savedAssessment2 = SetupSuccessfulPost(localAssessment2);
            var savedAssessment3 = SetupSuccessfulPost(localAssessment3);
            _persistenceContext.LoadAll().Returns(new[] { localAssessment1, localAssessment2, localAssessment3 });

            var result = await _service.Push<Assessment>(UserToken, a => a.ObjectId == localAssessment2.ObjectId || a.ObjectId == localAssessment3.ObjectId);

            var items = result.SuccessfulItems.ToList();
            Assert.That(items.Count, Is.EqualTo(2));
            Assert.That(items.Contains(savedAssessment2));
            Assert.That(items.Contains(savedAssessment3));
            _persistenceContext.DidNotReceive().Save(savedAssessment1);
            _persistenceContext.DidNotReceive().Purge(localAssessment1.ObjectId);
            _persistenceContext.Received().Save(savedAssessment2);
            _persistenceContext.Received().Purge(localAssessment2.ObjectId);
            _persistenceContext.Received().Save(savedAssessment3);
            _persistenceContext.Received().Purge(localAssessment3.ObjectId);
        }

        [Test]
        public async Task Should_Report_Failures()
        {
            var localAssessment1 = CreateLocalAssessment();
            var localAssessment2 = CreateLocalAssessment();
            var savedAssessment1 = SetupSuccessfulPost(localAssessment1);
            var failedResponse = SetupFailedPost(localAssessment2, "Test error");
            _persistenceContext.LoadAll().Returns(new[] { localAssessment1, localAssessment2 });

            var result = await _service.Push<Assessment>(UserToken);
            var successful = result.SuccessfulItems.ToList();
            Assert.That(successful.Count, Is.EqualTo(1));
            Assert.That(successful.Contains(savedAssessment1));

            var failed = result.FailedItems.ToList();
            Assert.That(failed.Count, Is.EqualTo(1));
            Assert.That(failed.Contains(localAssessment2));
            Assert.That(result.GetErrorMessage(localAssessment2), Is.EqualTo(failedResponse.ToString()));
        }

    }
}

