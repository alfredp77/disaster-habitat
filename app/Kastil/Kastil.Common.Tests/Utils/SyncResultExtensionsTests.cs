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
    public class SyncResultExtensionsTests
    {
        private Tuple<UpdateResult<Assessment>, Assessment> PrepareSuccessfulSyncResult()
        {
            var localId = Guid.NewGuid().ToString();
            var assessment = new Assessment { ObjectId = Guid.NewGuid().ToString() };
            var syncResult = new UpdateResult<Assessment>();
            syncResult.Success(assessment, localId);
            return new Tuple<UpdateResult<Assessment>, Assessment>(syncResult, assessment);
        }

        private Tuple<UpdateResult<Assessment>, Assessment> PrepareFailedSyncResult()
        {
            var localId = Guid.NewGuid().ToString();
            var assessment = new Assessment { ObjectId = Guid.NewGuid().ToString() };
            var syncResult = new UpdateResult<Assessment>();
            syncResult.Failed(assessment, localId);
            return new Tuple<UpdateResult<Assessment>, Assessment>(syncResult, assessment);
        }
        

        [Test]
        public void Should_Merge_Successful_Items()
        {
            var tuple1 = PrepareSuccessfulSyncResult();
            var tuple2 = PrepareSuccessfulSyncResult();

            var merged = tuple1.Item1.Merge(tuple2.Item1);
            var items = merged.SuccessfulItems.ToList();
            Assert.That(items.Count, Is.EqualTo(2));
            Assert.That(items.Contains(tuple1.Item2));
            Assert.That(items.Contains(tuple2.Item2));
        }

        [Test]
        public void Should_Merge_Failed_Items()
        {
            var tuple1 = PrepareFailedSyncResult();
            var tuple2 = PrepareFailedSyncResult();

            var merged = tuple1.Item1.Merge(tuple2.Item1);
            var items = merged.FailedItems.ToList();
            Assert.That(items.Count, Is.EqualTo(2));
            Assert.That(items.Contains(tuple1.Item2));
            Assert.That(items.Contains(tuple2.Item2));
        }


        [Test]
        public void Should_Merge_Both_Successful_And_Failed_Items()
        {
            var tuple1 = PrepareFailedSyncResult();
            var tuple2 = PrepareSuccessfulSyncResult();
            var tuple3 = PrepareSuccessfulSyncResult();

            var merged = tuple1.Item1.Merge(tuple2.Item1, tuple3.Item1);
            var successful = merged.SuccessfulItems.ToList();
            Assert.That(successful.Count, Is.EqualTo(2));
            Assert.That(successful.Contains(tuple2.Item2));
            Assert.That(successful.Contains(tuple3.Item2));

            var failed = merged.FailedItems.ToList();
            Assert.That(failed.Count, Is.EqualTo(1));
            Assert.That(failed.Contains(tuple1.Item2));
        }

    }
    
}
