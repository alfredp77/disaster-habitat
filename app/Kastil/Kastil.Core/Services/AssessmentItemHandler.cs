using System;
using System.Threading.Tasks;
using Kastil.Common.Models;
using Kastil.Common.Services;
using Kastil.Common.Utils;

namespace Kastil.Core.Services
{
    public class AssessmentItemHandler : BaseService, IAttributedItemHandler
    {
        private readonly Assessment _assessment;
        public AssessmentItemHandler(Assessment assessment, string disasterId)
        {
            if (assessment == null)
            {
                _assessment = new Assessment {ObjectId = Guid.NewGuid().ToString(), DisasterId = disasterId};
            }
            else
            {
                _assessment = Resolve<IJsonSerializer>().Clone(assessment);
            }
        }

        public Attributed CurrentItem => _assessment;
        public async Task CommitChanges()
        {
            var service = Resolve<ITap2HelpService>();
            await service.Save(_assessment);
        }

        public string NamePlaceholderText => "Enter assessment name";
        public string LocationPlaceholderText => "Where was this assessment made?";
        public string ItemType => typeof(Assessment).Name;
    }
}