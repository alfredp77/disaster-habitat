using Kastil.Common.Models;
using Kastil.Core.Services;

namespace Kastil.Core.ViewModels
{
    public class AssessmentViewModel : AttributedViewModel
    {
        public AssessmentViewModel(AssessmentEditContext context) : base(context)
        {
        }

        public override string NamePlaceholderText => "Enter assessment name";
        public override string LocationPlaceholderText => "Where was this assessment made?";
        public override string ItemType => typeof(Assessment).Name;

        protected override void NavigateToEditScreen()
        {
            ShowViewModel<EditAssessmentAttributeViewModel>();
        }
    }
}