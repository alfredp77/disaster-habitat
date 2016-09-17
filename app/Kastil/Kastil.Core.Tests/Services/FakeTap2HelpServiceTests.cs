using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kastil.Core.Services;
using NUnit.Core;
using NUnit.Framework;

namespace Kastil.Core.Tests.Services
{

    [TestFixture]
    public class FakeTap2HelpServiceTests
    {
        private FakeTap2HelpService _service;

        [SetUp]
        public void SetUp()
        {
            _service = new FakeTap2HelpService();
        }

        [Test]
        public async Task GetDisastersIsFine()
        {
            var disasters = await _service.GetDisasters();
            Assert.That(disasters.Any());
        }

        [Test]
        public async Task AttributesTest()
        {
            var assesments = await _service.GetAssesments();
            var first = assesments.First();
            Assert.That(first.Attributes.FirstOrDefault(), Is.Not.Null);
        }

        [Test]
        public async Task GetAllAssesmentIsWorking()
        {
            var assesments = await _service.GetAssesments();
            Assert.That(assesments.Any());
        }

        [Test]
        public async Task GetAllAssesmentsForEachDisaster()
        {
            var disasters = await _service.GetDisasters();
            foreach(var disaster in disasters)
            {
                var assesments = await _service.GetAssesments(disaster.Id);
                Assert.That(assesments.Any());
                Assert.True(assesments.All(a => a.DisasterId.Equals(disaster.Id)));
            }
        }


    }
}
