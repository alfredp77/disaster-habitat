using System.Linq;
using System.Threading.Tasks;
using Acr.UserDialogs;
using Kastil.Common.Services;
using Kastil.Common.Utils;
using Kastil.Core.Events;
using Kastil.Core.Services;

namespace Kastil.Core.ViewModels
{
    public class EditShelterAttributeViewModel : EditItemAttributeViewModel
    {
        private IShelterEditContext _shelterEditContext;
        public IShelterEditContext ShelterEditContext
        {
            get { return _shelterEditContext; }
            set { _shelterEditContext = value; RaisePropertyChanged(); }
        }

        protected override void DoDeleteAttrCommand()
        {
            ShelterEditContext.DeleteAttribute(SelectedItem.Caption);
            Publish(new EditingDoneEvent(this, EditAction.Delete));
            Close();
        }

        protected override void DoAddAttrCommand()
        {
            ShelterEditContext.AddOrUpdateAttribute(SelectedItem.Attribute, AttributeValue);
            Publish(new EditingDoneEvent(this, EditAction.Edit));
            Close();
        }

        public override async Task Initialize()
        {
            var dialogs = Resolve<IUserDialogs>();
            dialogs.ShowLoading(Messages.General.Loading);
            try
            {
                ShelterEditContext = Resolve<IShelterEditContext>();
                var item = ShelterEditContext.Item;
                Name = item.Name;
                var service = Resolve<ITap2HelpService>();
                Items = (await service.GetAttributes(item)).Select(attribute => new SpinnerItem(attribute)).ToList();

                if (ShelterEditContext.SelectedAttribute != null)
                {
                    EditMode = true;
                    ButtonText = "Update";
                    SelectedItem = new SpinnerItem(ShelterEditContext.SelectedAttribute);
                    AttributeText = ShelterEditContext.SelectedAttribute.Key;
                    AttributeValue = ShelterEditContext.SelectedAttribute.Value;
                }
            }
            finally
            {
                dialogs.HideLoading();
            }
        }
    }
}
