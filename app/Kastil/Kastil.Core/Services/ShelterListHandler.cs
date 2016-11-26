using System.Collections.Generic;
using System.Threading.Tasks;
using Kastil.Common.Models;
using Kastil.Common.Services;

namespace Kastil.Core.Services
{
    public class ShelterListHandler : BaseService, IAttributedListHandler
    {
        public IAttributedItemHandler CreateItemHandler(Attributed item, string disasterId)
        {
            var shelter = (Shelter)item;
            return new ShelterItemHandler(shelter, disasterId);
        }

        public async Task<IEnumerable<Attributed>> Load()
        {
            var service = Resolve<ITap2HelpService>();
            return await service.GetShelters();
        }

        public string ItemType => typeof(Shelter).Name;
    }
}