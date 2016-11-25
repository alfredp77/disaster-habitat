using System;
using System.Threading.Tasks;
using Kastil.Common.Models;
using Kastil.Common.Services;

namespace Kastil.Core.Services
{
    public class AssessmentEditContext : ItemEditContext<Assessment>
    {        
        protected override Assessment CreateItem(string disasterId)
        {
            return new Assessment { ObjectId = Guid.NewGuid().ToString(), DisasterId = disasterId };
        }

        public override async Task CommitChanges()
        {
            var service = Resolve<ITap2HelpService>();
            await service.Save(Item);
        }
    }
}