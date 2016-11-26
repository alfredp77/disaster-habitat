using System;
using System.Threading.Tasks;
using Kastil.Common.Models;
using Kastil.Common.Services;
using Kastil.Common.Utils;

namespace Kastil.Core.Services
{
    public class ShelterItemHandler : BaseService, IAttributedItemHandler
    {
        private readonly Shelter _shelter;
        public ShelterItemHandler(Shelter shelter, string disasterId)
        {
            if (shelter == null)
            {
                _shelter = new Shelter { ObjectId = Guid.NewGuid().ToString(), DisasterId = disasterId };
            }
            else
            {
                _shelter = Resolve<IJsonSerializer>().Clone(shelter);
            }
        }

        public Attributed CurrentItem => _shelter;
        public async Task CommitChanges()
        {
            var service = Resolve<ITap2HelpService>();
            await service.Save(_shelter);
        }

        public string NamePlaceholderText => "Enter shelter name";
        public string LocationPlaceholderText => "Where is this shelter located?";
        public string ItemType => typeof(Shelter).Name;
    }
}