using System;
using System.Threading.Tasks;
using Kastil.Common.Models;
using Kastil.Common.Services;

namespace Kastil.Core.Services
{
    public class ShelterEditContext : ItemEditContext<Shelter>
    {        
        protected override Shelter CreateItem(string disasterId)
        {
            return new Shelter { Id = Guid.NewGuid().ToString(), DisasterId = disasterId };
        }

        public override async Task CommitChanges()
        {
            var service = Resolve<ITap2HelpService>();
            await service.Save(Item);
        }
    }
}