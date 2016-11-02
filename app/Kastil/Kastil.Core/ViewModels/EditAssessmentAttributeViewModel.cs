using System.Linq;
using System.Threading.Tasks;
using Acr.UserDialogs;
using Kastil.Common.Services;
using Kastil.Common.Utils;
using Kastil.Core.Events;
using Kastil.Core.Services;

namespace Kastil.Core.ViewModels
{
    public class EditAssessmentAttributeViewModel : EditItemAttributeViewModel
    {
        private IAssessmentEditContext _assessmentEditContext;
        public IAssessmentEditContext AssessmentEditContext
        {
            get { return _assessmentEditContext; }
            set { _assessmentEditContext = value; RaisePropertyChanged(); }
        }

        protected override void DoDeleteAttrCommand()
        {
            AssessmentEditContext.DeleteAttribute(SelectedItem.Caption);
            Publish(new EditingDoneEvent(this, EditAction.Delete));
            Close();
        }

        protected override void DoAddAttrCommand()
        {
            AssessmentEditContext.AddOrUpdateAttribute(SelectedItem.Attribute, AttributeValue);
            Publish(new EditingDoneEvent(this, EditAction.Edit));
            Close();
        }

        public override async Task Initialize()
        {
            var dialogs = Resolve<IUserDialogs>();
            dialogs.ShowLoading(Messages.General.Loading);
            try
            {
                AssessmentEditContext = Resolve<IAssessmentEditContext>();
                var item = AssessmentEditContext.Item;
                Name = item.Name;
                var service = Resolve<ITap2HelpService>();
                Items = (await service.GetAttributes(item)).Select(attribute => new SpinnerItem(attribute)).ToList();

                if (AssessmentEditContext.SelectedAttribute != null)
                {
                    EditMode = true;
                    ButtonText = "Update";
                    SelectedItem = new SpinnerItem(AssessmentEditContext.SelectedAttribute);
                    AttributeText = AssessmentEditContext.SelectedAttribute.Key;
                    AttributeValue = AssessmentEditContext.SelectedAttribute.Value;
                }
            }
            finally
            {
                dialogs.HideLoading();
            }
        }
    }
}
