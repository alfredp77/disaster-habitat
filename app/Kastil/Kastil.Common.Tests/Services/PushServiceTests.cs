using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kastil.Common.Models;
using Kastil.Common.Services;
using Kastil.Common.Utils;
using NSubstitute;
using NUnit.Framework;

namespace Kastil.Common.Tests.Services
{
    [TestFixture]
    public class PushServiceTests
    {
        [Test]
        public async Task Should_Merge_Result_From_Both_Services()
        {
            var token = "abc";
            Predicate<Assessment> predicate = a => true;
            Assessment savedAssessment;
            var saveOrUpdate = SetupSaveOrUpdate(token, predicate, out savedAssessment);

            Assessment removedAssessment;
            var removal = SetupRemoval(token, predicate, out removedAssessment);

            var pushService = new PushService(saveOrUpdate, removal);

            var result = await pushService.Push(token, predicate);

            var success = result.SuccessfulItems.ToList();
            Assert.That(success.Contains(savedAssessment));
            Assert.That(success.Contains(removedAssessment));
        }

        private static IPushService SetupRemoval(string token, Predicate<Assessment> predicate, out Assessment removedAssessment)
        {
            var removal = Substitute.For<IPushService>();
            var removalResult = new SyncResult<Assessment>();
            removedAssessment = new Assessment {ObjectId = Guid.NewGuid().ToString()};
            removalResult.Success(removedAssessment, removedAssessment.ObjectId);
            removal.Push(token, predicate).Returns(Task.FromResult(removalResult));
            return removal;
        }

        private static IPushService SetupSaveOrUpdate(string token, Predicate<Assessment> predicate, out Assessment savedAssessment)
        {
            var saveOrUpdate = Substitute.For<IPushService>();
            var saveOrUpdateResult = new SyncResult<Assessment>();
            savedAssessment = new Assessment {ObjectId = Guid.NewGuid().ToString()};
            saveOrUpdateResult.Success(savedAssessment, "abc");
            saveOrUpdate.Push(token, predicate).Returns(Task.FromResult(saveOrUpdateResult));
            return saveOrUpdate;
        }
    }
}
