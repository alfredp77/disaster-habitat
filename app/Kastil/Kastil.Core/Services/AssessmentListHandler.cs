using System.Collections.Generic;
using System.Threading.Tasks;
using Kastil.Common.Models;
using Kastil.Common.Services;

namespace Kastil.Core.Services
{
    public class AssessmentListHandler : BaseService, IAttributedListHandler
    {
        public IAttributedItemHandler CreateItemHandler(Attributed item, string disasterId)
        {
            var assessment = (Assessment) item;
            return new AssessmentItemHandler(assessment, disasterId);
        }

        public async Task<IEnumerable<Attributed>> Load(string disasterId)
        {
            var service = Resolve<ITap2HelpService>();
            return await service.GetAssessments(disasterId);
        }

        public string ItemType => typeof (Assessment).Name;
    }
}