using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
    public class RemovalPushServiceTests : BaseTest
    {
        private RemovalPushService _service;
        private IJsonSerializer _jsonSerializer;
        private IPersistenceContextFactory _persistenceContextFactory;
        private IPersistenceContext<Assessment> _persistenceContext;
        private IBackendlessResponseParser _responseParser;
        private IRestServiceCaller _restServiceCaller;
        private Connection _connection = new Connection();
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

            _service = new RemovalPushService();
        }

        private static Assessment CreateAssessment()
        {
            return new Assessment {ObjectId = Guid.NewGuid().ToString()};
        }

        private void SetSuccessfulDeletion(Assessment assessment)
        {
            var url = Connection.GenerateTableUrl<Assessment>(assessment.ObjectId);
            var json = assessment.ObjectId;
            _restServiceCaller.Delete(url, Arg.Any<Dictionary<string, string>>())
                .Returns(Task.FromResult(json));
            _jsonSerializer.AsDictionary(json)
                .Returns(new Dictionary<string, string> {{RemovalPushService.DeletionTime, "12345"}});
        }

        private BackendlessResponse<Assessment> SetFailedDeletion(Assessment assessment, string errorMessage)
        {
            var url = Connection.GenerateTableUrl<Assessment>(assessment.ObjectId);
            var json = assessment.ObjectId;
            _restServiceCaller.Delete(url, Arg.Any<Dictionary<string, string>>())
                .Returns(Task.FromResult(json));
            _jsonSerializer.AsDictionary(json)
                .Returns(new Dictionary<string, string>());
            var backendlessResponse = BackendlessResponse<Assessment>.Failed("123", errorMessage);
            _responseParser.Parse<Assessment>(json).Returns(backendlessResponse);
            return backendlessResponse;
        }

        [Test]
        public async Task Should_Try_To_Remove_All_Deleted_Items()
        {
            var assessment1 = CreateAssessment();
            var assessment2 = CreateAssessment();
            _persistenceContext.LoadDeletedObjects().Returns(new[] {assessment1, assessment2});
            SetSuccessfulDeletion(assessment1);
            SetSuccessfulDeletion(assessment2);

            var result = await _service.Push<Assessment>(UserToken);

            var deletedAssessments = result.SuccessfulItems.ToList();
            Assert.That(deletedAssessments.Count, Is.EqualTo(2));
            Assert.That(deletedAssessments.Contains(assessment1));
            Assert.That(deletedAssessments.Contains(assessment2));
            Assert.That(result.FailedItems, Is.Empty);
        }

        [Test]
        public async Task Should_Only_Remove_Items_Which_Fulfills_The_Predicate()
        {
            var assessment1 = CreateAssessment();
            var assessment2 = CreateAssessment();
            _persistenceContext.LoadDeletedObjects().Returns(new[] { assessment1, assessment2 });
            SetSuccessfulDeletion(assessment1);
            SetSuccessfulDeletion(assessment2);

            var result = await _service.Push<Assessment>(UserToken, a => a.ObjectId == assessment2.ObjectId);

            var deletedAssessments = result.SuccessfulItems.ToList();
            Assert.That(deletedAssessments.Count, Is.EqualTo(1));
            Assert.That(!deletedAssessments.Contains(assessment1));
            Assert.That(deletedAssessments.Contains(assessment2));
            Assert.That(result.FailedItems, Is.Empty);
        }

        [Test]
        public async Task Should_Purge_Successfully_Deleted_Items()
        {
            var assessment1 = CreateAssessment();
            _persistenceContext.LoadDeletedObjects().Returns(new[] { assessment1 });
            SetSuccessfulDeletion(assessment1);

            await _service.Push<Assessment>(UserToken, a => a.ObjectId == assessment1.ObjectId);

            _persistenceContext.Received().Purge(assessment1.ObjectId);
        }

        [Test]
        public async Task Should_Report_Failures()
        {
            var assessment1 = CreateAssessment();
            _persistenceContext.LoadDeletedObjects().Returns(new[] { assessment1 });
            var failedResponse = SetFailedDeletion(assessment1, "Error deleting object");

            var result = await _service.Push<Assessment>(UserToken);

            Assert.That(result.GetErrorMessage(assessment1), Is.EqualTo(failedResponse.ToString()));
            var items = result.FailedItems.ToList();
            Assert.That(items.Count, Is.EqualTo(1));
            Assert.That(items.Contains(assessment1));
            Assert.That(result.SuccessfulItems, Is.Empty);
            _persistenceContext.DidNotReceive().Purge(assessment1.ObjectId);
        }
    }
}

