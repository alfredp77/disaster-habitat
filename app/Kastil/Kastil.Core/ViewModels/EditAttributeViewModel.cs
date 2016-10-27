using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Acr.UserDialogs;
using Kastil.Common.Services;
using Kastil.Common.ViewModels;
using Kastil.Core.Events;
using Kastil.Core.Services;
using MvvmCross.Core.ViewModels;
using Attribute = Kastil.Shared.Models.Attribute;
using Kastil.Shared.Models;

namespace Kastil.Core.ViewModels
{
    public class EditAttributeViewModel : BaseViewModel 
    {
        private string _name;
        private string _attributeValue;

        public class SpinnerItem
        {
            public SpinnerItem(Attribute attribute)
            {
                Attribute = attribute;
            }
            public Attribute Attribute { get; }
            public string Caption => Attribute.Key;

            public override string ToString()
            {
                return Caption;
            }

            public override bool Equals(object obj)
            {
                var item = obj as SpinnerItem;
                if (item == null)
                    return false;
                return item.Caption == Caption;
            }

            public override int GetHashCode()
            {
                return Caption?.GetHashCode() ?? 0;
            }
        }

        private bool _editMode;
        public bool EditMode
        {
            get { return _editMode; }
            private set 
            {
                if (_editMode != value)
                {
                    _editMode = value;
                    RaisePropertyChanged();
                }
            }
        }

        private List<SpinnerItem> _items;
        public List<SpinnerItem> Items
        {
            get { return _items; }
            set { _items = value; RaisePropertyChanged(() => Items); }
        }

        private SpinnerItem _selectedItem;
        public SpinnerItem SelectedItem
        {
            get { return _selectedItem; }
            set { _selectedItem = value; RaisePropertyChanged(() => SelectedItem); }
        }

        public string Name
        {
            get { return _name; }
            set { _name = value; RaisePropertyChanged(); }
        }

        public string AttributeValue
        {
            get { return _attributeValue; }
            set { _attributeValue = value; RaisePropertyChanged();}
        }

		private string _buttonText = "Add";
        public string ButtonText 
		{
			get { return _buttonText; }
			set { _buttonText = value; RaisePropertyChanged (); }
		}

        public override async Task Initialize()
        {
			var dialogs = Resolve<IUserDialogs> ();
			dialogs.ShowLoading (Messages.General.Loading);
			try 
			{
				var context = Resolve<IAssessmentEditContext> ();

				var item = context.Assessment;
				Name = item.Name;
				var service = Resolve<ITap2HelpService> ();
				Items = (await service.GetAssessmentAttributes ()).Select (attribute => new SpinnerItem (attribute)).ToList ();

				if (context.SelectedAttribute != null) {
					EditMode = true;
					ButtonText = "Update";
					SelectedItem = new SpinnerItem (context.SelectedAttribute);
					AttributeValue = context.SelectedAttribute.Value;
				}
			} 
			finally 
			{
				dialogs.HideLoading();
			}
        }


        public ICommand DeleteClickCommand => new MvxCommand(DoDeleteAttrCommand);

        public ICommand AddClickCommand => new MvxCommand(DoAddAttrCommand);

        public ICommand CancelClickCommand => new MvxCommand(DoCancelAttrCommand);

        private void DoDeleteAttrCommand()
        {
            var context = Resolve<IAssessmentEditContext>();
            context.DeleteAttribute(SelectedItem.Caption);
            Publish(new EditingDoneEvent(this, EditAction.Delete));
            Close();
        }


        private void DoCancelAttrCommand()
        {
            Close();
        }

        private void DoAddAttrCommand()
        {
            var context = Resolve<IAssessmentEditContext>();
            context.AddOrUpdateAttribute(SelectedItem.Attribute, AttributeValue);
            Publish(new EditingDoneEvent(this, EditAction.Edit));
            Close();
        }
    }
}
