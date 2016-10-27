using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Kastil.Common.Fakes;
using Kastil.Core.Services;
using Kastil.Shared.Models;
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
            var assessments = await _service.GetAssessments();
            var first = assessments.First();
            Assert.That(first.Attributes.FirstOrDefault(), Is.Not.Null);
        }

        [Test]
        public async Task GetAllAssessmentIsWorking()
        {
            var assessments = await _service.GetAssessments();
            Assert.That(assessments.Any());
        }

        [Test]
        public async Task GetAllAssessmentsForEachDisaster()
        {
            var disasters = await _service.GetDisasters();
            foreach (var disaster in disasters)
            {
                var assessments = await _service.GetAssessments(disaster.Id);
                Assert.That(assessments.Any());
                Assert.True(assessments.All(a => a.DisasterId.Equals(disaster.Id)));
            }
        }
    }
}