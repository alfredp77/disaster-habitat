using System.Collections.Generic;
using System.Windows.Input;
using Kastil.Common.Services;
using Kastil.Common.Utils;
using Kastil.Common.ViewModels;
using MvvmCross.Core.ViewModels;

namespace Kastil.Core.ViewModels
{
    public class EditItemAttributeViewModel : BaseViewModel 
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

        protected virtual void DoAddAttrCommand() { }

        protected virtual void DoDeleteAttrCommand() { }

        private void DoCancelAttrCommand()
        {
            Close();
        }
    }
}
