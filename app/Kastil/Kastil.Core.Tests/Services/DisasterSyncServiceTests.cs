using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kastil.Common.Models;
using Kastil.Common.Services;
using Kastil.Common.Utils;
using Kastil.Core.Services;
using NSubstitute;
using NUnit.Framework;

namespace Kastil.Core.Tests.Services
{
    [TestFixture]
    public class DisasterSyncServiceTests
    {
        private DisasterSyncService _service;
        private IPullService _pullService;
        private IPersistenceContextFactory _persistenceContextFactory;
        private IPersistenceContext<Assessment> _assessmentContext;
        private IPersistenceContext<Shelter> _shelterContext;
        private User _user;
        private List<Assessment> _assessments;
        private List<Shelter> _shelters;
        [SetUp]
        public void Setup()
        {
            _user = new User();
            _pullService = Substitute.For<IPullService>();
            _persistenceContextFactory = Substitute.For<IPersistenceContextFactory>();
            _assessmentContext = Substitute.For<IPersistenceContext<Assessment>>();
            _shelterContext = Substitute.For<IPersistenceContext<Shelter>>();

            _assessments = new List<Assessment>();
            _assessmentContext.LoadAll().Returns(_assessments);
            _shelters = new List<Shelter>();
            _shelterContext.LoadAll().Returns(_shelters);

            _persistenceContextFactory.CreateFor<Assessment>().Returns(_assessmentContext);
            _persistenceContextFactory.CreateFor<Shelter>().Returns(_shelterContext);

            _service = new DisasterSyncService(_pullService, _persistenceContextFactory);
        }

        [Test]
        public async Task Should_Pull_Disasters()
        {
            var result = new UpdateResult<Disaster>();
            _pullService.Pull<Disaster>().Returns(Task.FromResult(result));

            var syncResult = await _service.Sync(_user);

            Assert.That(syncResult.HasErrors, Is.False);
            await _pullService.Received().Pull<Disaster>();
        }
        private static T CreateItem<T>(string ownerId = null) where T : Attributed, new()
        {
            return new T { ObjectId = Guid.NewGuid().ToString(), OwnerId = ownerId };
        }

        [Test]
        public async Task Should_Purge_Unrelated_Assessments_When_Pull_Is_Successful()
        {
            await RunPurgeUnrelatedItemsTest(_assessmentContext);
        }

        [Test]
        public async Task Should_Purge_Unrelated_Shelters_When_Pull_Is_Successful()
        {
            await RunPurgeUnrelatedItemsTest(_shelterContext);
        }

        private async Task RunPurgeUnrelatedItemsTest<T>(IPersistenceContext<T> context) where T : Attributed, new()
        {
            var item1 = CreateItem<T>();
            item1.DisasterId = "abc";
            var item2 = CreateItem<T>();
            item2.DisasterId = "def";
            var items = new List<T> {item1, item2};
            context.LoadAll().Returns(items);

            var disaster = new Disaster {ObjectId = "abc"};
            var result = new UpdateResult<Disaster>();
            result.Success(disaster, "xxx");
            _pullService.Pull<Disaster>().Returns(Task.FromResult(result));

            var syncResult = await _service.Sync(_user);

            Assert.That(syncResult.HasErrors, Is.False);
            context.Received().Purge(item2.ObjectId);
            context.DidNotReceive().Purge(item1.ObjectId);
        }

        [Test]
        public async Task Should_Return_Failure_When_Pull_Is_Not_Successful()
        {
            var result = new UpdateResult<Disaster>();
            result.Success(new Disaster {ObjectId = "fff"}, "ccc");
            result.Failed(null, "test error");
            _assessmentContext.LoadAll().Returns(new[] {CreateItem<Assessment>()});
            _shelterContext.LoadAll().Returns(new[] { CreateItem<Shelter>() });
            _pullService.Pull<Disaster>().Returns(Task.FromResult(result));

            var syncResult = await _service.Sync(_user);

            Assert.That(syncResult.HasErrors);
            _assessmentContext.DidNotReceive().Purge(Arg.Any<string>());
            _shelterContext.DidNotReceive().Purge(Arg.Any<string>());
        }
    }
}
