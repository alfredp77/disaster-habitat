using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kastil.Common.Models;
using Kastil.Common.Services;
using Kastil.Common.Utils;
using MvvmCross.Plugins.File;

namespace Tap2Give.Core.Services
{
    public interface ITap2GiveService
    {
        Task<IEnumerable<Disaster>> GetDisasters(bool reload=false);
        Task<IEnumerable<DisasterAid>> GetDisasterAids(string disasterId);
    }

    public class Tap2GiveService : BaseService, ITap2GiveService
    {
        private readonly ITap2HelpService _service;
        private readonly IPullService _pullService;
        private readonly IPersistenceContextFactory _persistenceContextFactory;

        public Tap2GiveService(ITap2HelpService service, IPullService pullService, IPersistenceContextFactory persistenceContextFactory)
        {
            _service = service;
            _pullService = pullService;
            _persistenceContextFactory = persistenceContextFactory;
        }

        public async Task<IEnumerable<Disaster>> GetDisasters(bool reload=false)
        {
            var lastDownload = GetLastDownloadDate();
            if (reload || (lastDownload.HasValue && DateTimeOffset.UtcNow.Subtract(lastDownload.Value).TotalDays >= 1))
            {
                await _pullService.Pull<Disaster>();
                UpdateLastDownloadDate();
            }
            return await _service.GetDisasters();
        }

        public Task<IEnumerable<DisasterAid>> GetDisasterAids(string disasterId)
        {
            var context = _persistenceContextFactory.CreateFor<DisasterAid>();
            return Asyncer.Async(() => context.LoadAll().Where(d => d.DisasterId == disasterId));
        }

        private DateTimeOffset? GetLastDownloadDate()
        {
            var context = _persistenceContextFactory.CreateFor<SyncInfo>();
            var syncInfo = context.LoadAll().SingleOrDefault();
            return syncInfo?.LastSync;
        }

        private void UpdateLastDownloadDate()
        {
            var context = _persistenceContextFactory.CreateFor<SyncInfo>();
            var syncInfo = new SyncInfo {ObjectId = "x", LastSync = DateTimeOffset.UtcNow};
            context.Save(syncInfo);
        }
    }
}
