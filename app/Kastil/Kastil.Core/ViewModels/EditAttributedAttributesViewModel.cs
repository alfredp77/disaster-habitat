using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Acr.UserDialogs;
using Kastil.Common.Events;
using Kastil.Common.Utils;
using Kastil.Common.ViewModels;
using Kastil.Core.Services;
using MvvmCross.Core.ViewModels;

namespace Kastil.Core.ViewModels
{
    public class EditAttributedAttributesViewModel : BaseViewModel
    {
        private string _name;
        private string _attributeValue;
        private string _attributeText;
        private bool _editMode;
        private List<SpinnerItem> _items;
        private string _buttonText = "Add";

        public bool EditMode
        {
            get { return _editMode; }
            protected set
            {
                if (_editMode != value)
                {
                    _editMode = value;
                    RaisePropertyChanged();
                }
            }
        }

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

        public string AttributeText
        {
            get { return _attributeText; }
            set { _attributeText = value; RaisePropertyChanged(); }
        }

        public string AttributeValue
        {
            get { return _attributeValue; }
            set { _attributeValue = value; RaisePropertyChanged(); }
        }

        public string ButtonText
        {
            get { return _buttonText; }
            set { _buttonText = value; RaisePropertyChanged(); }
        }

        public ICommand DeleteClickCommand => new MvxCommand(DoDeleteAttrCommand);
        public ICommand AddClickCommand => new MvxCommand(DoAddAttrCommand);
        public ICommand CancelClickCommand => new MvxCommand(DoCancelAttrCommand);

        private void DoDeleteAttrCommand()
        {
            var context = Resolve<AttributedEditContext>();
            context.DeleteAttribute(SelectedItem.Caption);
            Publish(new EditingDoneEvent(this, EditAction.Delete));
            Close();
        }

        private void DoAddAttrCommand()
        {
            var context = Resolve<AttributedEditContext>();
            context.AddOrUpdateAttribute(SelectedItem.Attribute, AttributeValue);
            Publish(new EditingDoneEvent(this, EditAction.Edit));
            Close();
        }

        public override async Task Initialize()
        {
            var dialogs = Resolve<IUserDialogs>();
            dialogs.ShowLoading(Messages.General.Loading);

            var context = Resolve<AttributedEditContext>();
            try
            {
                Name = context.ItemName;
                Items = context.AvailableAttributes.Select(attribute => new SpinnerItem(attribute)).ToList();

                if (context.SelectedAttribute != null)
                {
                    var selected = Items.SingleOrDefault(s => s.Attribute.Key == context.SelectedAttribute.Key);
                    if (selected == null)
                    {
                        selected = new SpinnerItem(context.SelectedAttribute.AsBaseAttribute());
                        Items.Add(selected);
                    }
                    SelectedItem = selected;

                    EditMode = true;
                    ButtonText = "Update";
                    AttributeText = context.SelectedAttribute.Key;
                    AttributeValue = context.SelectedAttribute.Value;
                }
            }
            finally
            {
                dialogs.HideLoading();
            }
        }
        private void DoCancelAttrCommand()
        {
            Close();
        }
    }
}