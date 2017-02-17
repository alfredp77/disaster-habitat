using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kastil.Common.Models;
using Kastil.Common.Utils;
using NUnit.Framework;

namespace Kastil.Common.Tests.Utils
{
    [TestFixture]
    public class SyncResultTests
    {
        private UpdateResult<Assessment> _updateResult;
        private Assessment _assessment1, _assessment2, _assessment3;

        [SetUp]
        public void Setup()
        {
            _updateResult = new UpdateResult<Assessment>();
            _assessment1 = new Assessment { ObjectId = Guid.NewGuid().ToString() };
            _assessment2 = new Assessment { ObjectId = Guid.NewGuid().ToString() };
            _assessment3 = new Assessment { ObjectId = Guid.NewGuid().ToString() };
        }

        [Test]
        public void Should_Register_Successful_Items_Correctly()
        {
            var localId1 = Guid.NewGuid().ToString();
            _updateResult.Success(_assessment1, localId1);
            var localId2 = Guid.NewGuid().ToString();
            _updateResult.Success(_assessment2, localId2);

            Assert.That(_updateResult.GetLocalId(_assessment1), Is.EqualTo(localId1));
            Assert.That(_updateResult.GetLocalId(_assessment2), Is.EqualTo(localId2));
            Assert.That(_updateResult.SuccessfulItems, Is.EquivalentTo(new[] {_assessment1, _assessment2}));
        }

        [Test]
        public void Should_Register_Failed_Items_Correctly()
        {
            var errorMessage1 = Guid.NewGuid().ToString();
            _updateResult.Failed(_assessment1, errorMessage1);
            var errorMessage2 = Guid.NewGuid().ToString();
            _updateResult.Failed(_assessment2, errorMessage2);

            Assert.That(_updateResult.GetErrorMessage(_assessment1), Is.EqualTo(errorMessage1));
            Assert.That(_updateResult.GetErrorMessage(_assessment2), Is.EqualTo(errorMessage2));
            Assert.That(_updateResult.FailedItems, Is.EquivalentTo(new[] { _assessment1, _assessment2 }));
        }

        [Test]
        public void Should_Be_Able_To_Register_Both_Successful_And_Failed_Items_Correctly()
        {
            var errorMessage1 = Guid.NewGuid().ToString();
            _updateResult.Failed(_assessment1, errorMessage1);
            var errorMessage2 = Guid.NewGuid().ToString();
            _updateResult.Failed(_assessment2, errorMessage2);
            var localId3 = Guid.NewGuid().ToString();
            _updateResult.Success(_assessment3, localId3);

            Assert.That(_updateResult.GetErrorMessage(_assessment1), Is.EqualTo(errorMessage1));
            Assert.That(_updateResult.GetErrorMessage(_assessment2), Is.EqualTo(errorMessage2));
            Assert.That(_updateResult.FailedItems, Is.EquivalentTo(new[] { _assessment1, _assessment2 }));
            Assert.That(_updateResult.GetLocalId(_assessment3), Is.EqualTo(localId3));
            Assert.That(_updateResult.SuccessfulItems, Is.EquivalentTo(new[] { _assessment3 }));
        }
    }
}
