using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kastil.Common.Services;
using Kastil.Core.Services;
using Kastil.Shared.Models;
using Moq;
using NUnit.Framework;

namespace Kastil.Core.Tests.Services
{
    [TestFixture]
    public class SyncServiceTests : BaseTest
    {
        private SyncService _syncService;
        private Mock<IPushService> _pushService;
        private Mock<IPullService> _pullService;
        private Mock<ITap2HelpService> _tap2HelpService;

        public override void CreateTestableObject()
        {
            _syncService = new SyncService();
            _pushService = CreateMock<IPushService>();
            _pullService = CreateMock<IPullService>();
            _tap2HelpService = CreateMock<ITap2HelpService>();

            Ioc.RegisterSingleton(_pushService.Object);
            Ioc.RegisterSingleton(_pullService.Object);
            Ioc.RegisterSingleton(_tap2HelpService.Object);
        }

        [Test]
        public async Task Should_Clean_Assessments_From_Removed_Disasters()
        {
            var disaster1 = new Disaster { Id = "x" };
            var disaster2 = new Disaster { Id = "y" };
            var disaster3 = new Disaster { Id = "z" };
            var disaster4 = new Disaster { Id = "a" };
            var localDisasters = new List<Disaster> {disaster1, disaster2 ,disaster3};
            var incomingDisasters = new List<Disaster> {disaster1, disaster4};
            _tap2HelpService.SetupSequence(s => s.GetDisasters())
                .ReturnsAsync(localDisasters.AsEnumerable())
                .ReturnsAsync(incomingDisasters.AsEnumerable());

            await _syncService.PullDisasters();

            _pullService.Verify(p => p.Pull<Disaster>(true), Times.Once);
            _tap2HelpService.Verify(s => s.DeleteAssessments(disaster2.Id), Times.Once);
            _tap2HelpService.Verify(s => s.DeleteAssessments(disaster3.Id), Times.Once);
        }
    }
}