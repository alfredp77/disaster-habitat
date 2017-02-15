using System;
using System.Collections.Generic;
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
    public class AttributedItemSyncServiceTests
    {
        private AttributedItemSyncService<Assessment, AssessmentAttribute> _syncService;
        private IPushService _pushService;
        private IPullService _pullService;
        private IBackendlessQueryProvider _queryProvider;
        private IBackendlessQuery _query;
        private IPersistenceContextFactory _persistenceContextFactory;
        private User _user;

        [SetUp]
        public void Setup()
        {
            _pushService = Substitute.For<IPushService>();
            _pullService = Substitute.For<IPullService>();
            _persistenceContextFactory = Substitute.For<IPersistenceContextFactory>();

            _queryProvider = Substitute.For<IBackendlessQueryProvider>();
            _query = Substitute.For<IBackendlessQuery>();
            _queryProvider.Where().Returns(_query);
            _pullService.Pull<Assessment>().ReturnsForAnyArgs(Task.FromResult(new SyncResult<Assessment>()));
            _pullService.Pull<AssessmentAttribute>().ReturnsForAnyArgs(Task.FromResult(new SyncResult<AssessmentAttribute>()));

            _user = new User {Token = "blah", ObjectId = "xyz"};
            _query.OwnedBy(_user.ObjectId).IsActive().Returns(_query);

            _syncService = new AttributedItemSyncService<Assessment, AssessmentAttribute>(_pushService, _pullService, 
                            _queryProvider, _persistenceContextFactory);
        }

        private static T CreateItem<T>(string ownerId = null) where T : BaseModel, new()
        {
            return new T { ObjectId = Guid.NewGuid().ToString(), OwnerId = ownerId };
        }

        private static SyncResult<T> CreateSyncResult<T>(params string[] localIds) where T : BaseModel, new()
        {
            var syncResult = new SyncResult<T>();
            foreach (var localId in localIds)
            {
                var item = CreateItem<T>();
                syncResult.Success(item, localId);
            }
            return syncResult;
        }

        [Test]
        public async Task Should_Push_Items_And_Attributes_Of_Successfully_Pushed_Items()
        {
            var syncResult = CreateSyncResult<Assessment>("1", "2");
            syncResult.Failed(CreateItem<Assessment>(), "error!");
            _pushService.Push<Assessment>(_user.Token).Returns(Task.FromResult(syncResult));
            var attributeSyncResult = CreateSyncResult<AssessmentAttribute>();
            Predicate<AssessmentAttribute> predicate = null;
            _pushService.Push(_user.Token, Arg.Do<Predicate<AssessmentAttribute>>(p => predicate = p))
                .Returns(attributeSyncResult);

            await _syncService.Sync(_user);

            Assert.That(predicate(new AssessmentAttribute { ItemId = "1"}));
            Assert.That(predicate(new AssessmentAttribute { ItemId = "2" }));
            Assert.That(predicate(new AssessmentAttribute { ItemId = "3" }), Is.False);
        }

        private Tuple<SyncResult<Assessment>, SyncResult<AssessmentAttribute>>  SetupEmptyPush()
        {
            var syncResult = CreateSyncResult<Assessment>();
            var attributeSyncResult = CreateSyncResult<AssessmentAttribute>();
            _pushService.Push<Assessment>(_user.Token).Returns(Task.FromResult(syncResult));
            _pushService.Push(_user.Token, Arg.Any<Predicate<AssessmentAttribute>>())
                .Returns(Task.FromResult(attributeSyncResult));
            return new Tuple<SyncResult<Assessment>, SyncResult<AssessmentAttribute>>(syncResult, attributeSyncResult);
        }

        [Test]
        public async Task Should_Pull_Assessments_When_Push_Is_Successful()
        {
            SetupEmptyPush();

            await _syncService.Sync(_user);

            await _pullService.Received().Pull<Assessment>(_query);
        }

        [Test]
        public async Task Should_NOT_Pull_Assesments_When_Push_Has_Some_Failures_On_Assessment()
        {
            var syncResults = SetupEmptyPush();
            var assessmentSyncResult = syncResults.Item1;
            assessmentSyncResult.Failed(CreateItem<Assessment>(), "xxx");
            
            await _syncService.Sync(_user);

            await _pullService.DidNotReceiveWithAnyArgs().Pull<Assessment>();
        }

        [Test]
        public async Task Should_Pull_Attributes_When_Assessments_Are_Pulled_Successfully()
        {
            SetupEmptyPush();

            await _syncService.Sync(_user);

            await _pullService.Received().Pull<AssessmentAttribute>(_query);
        }

        [Test]
        public async Task Should_NOT_Pull_Assesments_When_Push_Has_Some_Failures_On_Attributes()
        {
            var syncResults = SetupEmptyPush();
            var attributeSyncResult = syncResults.Item2;
            attributeSyncResult.Failed(CreateItem<AssessmentAttribute>(), "xxx");

            await _syncService.Sync(_user);

            await _pullService.DidNotReceiveWithAnyArgs().Pull<Assessment>();
        }

        [Test]
        public async Task Should_NOT_Pull_Attributes_When_Assessments_Are_Not_Pulled_Successfully()
        {
            SetupEmptyPush();
            var assessmentPullResult = new SyncResult<Assessment>();
            assessmentPullResult.Failed(CreateItem<Assessment>(), "error!!");
            _pullService.Pull<Assessment>(_query).Returns(Task.FromResult(assessmentPullResult));

            await _syncService.Sync(_user);

            await _pullService.DidNotReceiveWithAnyArgs().Pull<AssessmentAttribute>();
        }

        [Test]
        public async Task Should_Purge_Other_Users_Items()
        {
            SetupEmptyPush();
            var assessmentContext = _persistenceContextFactory.CreateFor<Assessment>();
            var attr1 = CreateItem<AssessmentAttribute>();
            var attr2 = CreateItem<AssessmentAttribute>();
            var assessmentAttributeContext = _persistenceContextFactory.CreateFor<AssessmentAttribute>();
            var assessments = await SetupPurgeTest(assessmentContext, attr1, attr2, assessmentAttributeContext);

            assessmentAttributeContext.Received().Purge(attr1.ObjectId);
            assessmentContext.Received().Purge(assessments[0].ObjectId);

            assessmentAttributeContext.DidNotReceive().Purge(attr2.ObjectId);
            assessmentContext.DidNotReceive().Purge(assessments[1].ObjectId);
        }

        private async Task<List<Assessment>> SetupPurgeTest(IPersistenceContext<Assessment> assessmentContext, AssessmentAttribute attr1,
            AssessmentAttribute attr2, IPersistenceContext<AssessmentAttribute> assessmentAttributeContext)
        {
            var assessments = new List<Assessment> {CreateItem<Assessment>("456"), CreateItem<Assessment>(_user.ObjectId)};
            assessmentContext.LoadAll().Returns(assessments);

            attr1.ItemId = assessments[0].ObjectId;
            attr2.ItemId = assessments[1].ObjectId;
            var attributes = new List<AssessmentAttribute>
            {
                attr1,
                attr2
            };
            assessmentAttributeContext = _persistenceContextFactory.CreateFor<AssessmentAttribute>();
            assessmentAttributeContext.LoadAll().Returns(attributes);

            await _syncService.Sync(_user);
            return assessments;
        }

        [Test]
        public async Task Should_Not_Purge_Anything_When_Attribute_Was_Not_Pulled_Successfully()
        {
            SetupEmptyPush();
            var attributePullResult = new SyncResult<Assessment>();
            attributePullResult.Failed(CreateItem<Assessment>(), "error!!");
            _pullService.Pull<Assessment>(_query).Returns(Task.FromResult(attributePullResult));

            var assessmentContext = _persistenceContextFactory.CreateFor<Assessment>();
            var attr1 = CreateItem<AssessmentAttribute>();
            var attr2 = CreateItem<AssessmentAttribute>();
            var assessmentAttributeContext = _persistenceContextFactory.CreateFor<AssessmentAttribute>();
            await SetupPurgeTest(assessmentContext, attr1, attr2, assessmentAttributeContext);

            await _syncService.Sync(_user);

            assessmentAttributeContext.DidNotReceiveWithAnyArgs().Purge("");
            assessmentContext.DidNotReceiveWithAnyArgs().Purge("");
        }

    }
}
