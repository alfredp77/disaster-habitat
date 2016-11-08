using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Acr.UserDialogs;
using Kastil.Common.Utils;
using Kastil.Common.ViewModels;
using Kastil.Core.Events;
using Kastil.Core.Services;
using MvvmCross.Core.ViewModels;

namespace Kastil.Core.ViewModels
{
    public abstract class EditAttributedItemViewModel : BaseViewModel
    {
        private IItemEditContext _context;
        private string _name;
        private string _attributeValue;
        private string _attributeText;
        private bool _editMode;
        private List<SpinnerItem> _items;
        private string _buttonText = "Add";

        protected EditAttributedItemViewModel(IItemEditContext context)
        {
            _context = context;
        }

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
            _context.DeleteAttribute(SelectedItem.Caption);
            Publish(new EditingDoneEvent(this, EditAction.Delete));
            Close();
        }

        private void DoAddAttrCommand()
        {
            _context.AddOrUpdateAttribute(SelectedItem.Attribute, AttributeValue);
            Publish(new EditingDoneEvent(this, EditAction.Edit));
            Close();
        }

        public override async Task Initialize()
        {
            var dialogs = Resolve<IUserDialogs>();
            dialogs.ShowLoading(Messages.General.Loading);
            try
            {
                Name = _context.ItemName;
                Items = _context.Attributes.Select(attribute => new SpinnerItem(attribute)).ToList();

                if (_context.SelectedAttribute != null)
                {
                    EditMode = true;
                    ButtonText = "Update";
                    SelectedItem = new SpinnerItem(_context.SelectedAttribute);
                    AttributeText = _context.SelectedAttribute.Key;
                    AttributeValue = _context.SelectedAttribute.Value;
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