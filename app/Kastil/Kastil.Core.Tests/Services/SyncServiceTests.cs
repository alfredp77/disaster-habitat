using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kastil.Common.Services;
using Kastil.Core.Services;
using Kastil.Common.Models;
using Kastil.TestUtils;
using NSubstitute;
using NUnit.Framework;

namespace Kastil.Core.Tests.Services
{
    [TestFixture]
    public class SyncServiceTests
    {
        private SyncService _syncService;
        private ISyncService _service1, _service2;
        private IPersistenceContextFactory _persistenceContextFactory;
        private User _user;

        [SetUp]
        public void Setup()
        {
            _user = new User {ObjectId = Guid.NewGuid().ToString()};
            _service1 = Substitute.For<ISyncService>();
            _service2 = Substitute.For<ISyncService>();
            _persistenceContextFactory = Substitute.For<IPersistenceContextFactory>();
            
            _syncService = new SyncService(_persistenceContextFactory, new [] {_service1, _service2});
        }

        [Test]
        public async Task Should_Return_Failure_When_Any_Of_The_Services_Fails()
        {
            _service1.Sync(_user).Returns(Task.FromResult(SyncResult.Success()));
            _service2.Sync(_user).Returns(Task.FromResult(SyncResult.Failed("test error")));

            var syncResult = await _syncService.Sync(_user);

            Assert.That(syncResult.HasErrors);
            Assert.That(syncResult.ErrorMessage, Is.EqualTo("test error"));
        }

        [Test]
        public async Task Should_Not_Execute_Subsequent_SyncServices_On_Failure()
        {
            _service1.Sync(_user).Returns(Task.FromResult(SyncResult.Failed("test error")));
            _service2.Sync(_user).Returns(Task.FromResult(SyncResult.Success()));

            var syncResult = await _syncService.Sync(_user);

            await _service2.DidNotReceive().Sync(_user);
            Assert.That(syncResult.HasErrors);
            Assert.That(syncResult.ErrorMessage, Is.EqualTo("test error"));
        }

        [Test]
        public async Task Should_Return_Success_When_No_Errors()
        {
            _service1.Sync(_user).Returns(Task.FromResult(SyncResult.Success()));
            _service2.Sync(_user).Returns(Task.FromResult(SyncResult.Success()));

            var syncResult = await _syncService.Sync(_user);

            Assert.That(syncResult.HasErrors, Is.False);
        }

        [Test]
        public async Task Should_Record_Last_Sync_When_Successful()
        {
            _service1.Sync(_user).Returns(Task.FromResult(SyncResult.Success()));
            _service2.Sync(_user).Returns(Task.FromResult(SyncResult.Success()));
            var currentTime = DateTimeOffset.UtcNow;
            _syncService.GetCurrentTimeFunc = () => currentTime;

            await _syncService.Sync(_user);

            _persistenceContextFactory.CreateFor<SyncInfo>().Received().Save(Arg.Is<SyncInfo>(s => s.LastSync == currentTime && s.ObjectId == "x"));
        }

        [Test]
        public async Task Should_Not_Record_Last_Sync_When_Failed()
        {
            _service1.Sync(_user).Returns(Task.FromResult(SyncResult.Failed("test error!")));
            _service2.Sync(_user).Returns(Task.FromResult(SyncResult.Success()));

            await _syncService.Sync(_user);

            _persistenceContextFactory.DidNotReceive().CreateFor<SyncInfo>();
        }

    }
}