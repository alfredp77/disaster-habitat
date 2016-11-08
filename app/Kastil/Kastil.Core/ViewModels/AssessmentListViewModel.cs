using Kastil.Core.Services;
using System.Collections.Generic;
using System.Threading.Tasks;
using Kastil.Common.Models;
using Kastil.Common.Services;

namespace Kastil.Core.ViewModels
{
    public class AssessmentListViewModel : ItemListViewModel
    {
        private readonly AssessmentEditContext _context;

        public AssessmentListViewModel(AssessmentEditContext context)
        {
            _context = context;
            Title = "Assessments";
            AllowAddCommand = true;
        }

        protected override void DoItemSelectedCommand(AttributedListItemViewModel itemVm)
        {
            var assessment = (Assessment) itemVm.Value;
            _context.Initialize(assessment, DisasterId);
            ShowViewModel<AssessmentViewModel>();
        }

        protected override async Task<IEnumerable<Item>> GetItems()
        {
            var service = Resolve<ITap2HelpService>();
            return await service.GetAssessments(DisasterId);
        }

        protected override string ItemType => typeof (Assessment).Name;

        protected override Task DoAddCommand()
        {
            return Task.Run(() =>
            {
                _context.Initialize(disasterId: DisasterId);
                ShowViewModel<AssessmentViewModel>();
            });
        }
    }
}