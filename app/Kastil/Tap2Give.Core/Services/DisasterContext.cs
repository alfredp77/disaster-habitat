using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kastil.Common.Models;
using Kastil.Common.Services;

namespace Tap2Give.Core.Services
{
    public interface IDisasterContext
    {
        Disaster Disaster { get; }
        IEnumerable<DisasterAid> DisasterAids { get; }
        Task Initialize(Disaster disaster);
    }

    public class DisasterContext : BaseService, IDisasterContext
    {
        public Disaster Disaster { get; private set; }

        public async Task Initialize(Disaster disaster)
        {
            Disaster = disaster;

            var tap2GiveService = Resolve<ITap2GiveService>();
            DisasterAids = (await tap2GiveService.GetDisasterAids(disaster.ObjectId)).ToList();
        }

        public IEnumerable<DisasterAid> DisasterAids { get; private set; }
    }
}
