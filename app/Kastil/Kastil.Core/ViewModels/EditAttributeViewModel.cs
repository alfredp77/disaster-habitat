using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Kastil.Core.Services;
using Kastil.Core.Utils;
using Kastil.Shared.Models;
using MvvmCross.Core.ViewModels;
using Attribute = Kastil.Shared.Models.Attribute;

namespace Kastil.Core.ViewModels
{
    public class EditAttributeViewModel : BaseViewModel
    {
        private string _disasterId;
        private string _assesmentId;
        private string _name;
        private string _attributeValue;
        private string _originalValue;
        private Assesment _assesment;

        public class SpinnerItem
        {
            public SpinnerItem(string caption)
            {
                Caption = caption;
            }

            public string Caption { get; }

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

        public bool EditMode { get; private set; }
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

        public string ButtonText => EditMode ? "Update" : "Add";

        public async void Init(string disasterId , string assesmentId, string attributeName, string attributeValue)
        {
            _disasterId = disasterId;
            _assesmentId = assesmentId;
            var service = Resolve<ITap2HelpService>();
            _assesment = await service.GetAssesment(_disasterId, _assesmentId);
            Items = (from attribute in await service.GetAssesmentAttributes() select new SpinnerItem(attribute.Key)).ToList();
            Name = _assesment.Name;
            if (attributeName == null || attributeValue == null)
            {
                EditMode = false;
                return;
            }

            EditMode = true;
            SelectedItem = new SpinnerItem(attributeName);
            AttributeValue = attributeValue;
            _originalValue = attributeValue;

        }

        public ICommand DeleteClickCommand => new MvxCommand<EditAttributeViewModel>(DoDeleteAttrCommand);

        public ICommand AddClickCommand => new MvxCommand<EditAttributeViewModel>(DoAddAttrCommand);

        public ICommand CancelClickCommand => new MvxCommand<EditAttributeViewModel>(DoCancelAttrCommand);

        private void DoDeleteAttrCommand(EditAttributeViewModel obj)
        {
            var attributes = _assesment.Attributes;
            attributes.Remove(attributes.Single(attr => attr.Key == SelectedItem.Caption && attr.Value == _originalValue));

            SaveAssesment();
            Close();
        }


        private void DoCancelAttrCommand(EditAttributeViewModel obj)
        {
            Close();
        }

        private void DoAddAttrCommand(EditAttributeViewModel obj)
        {
            
            var attributes = _assesment.Attributes;
            if (EditMode)
            {
                UpdateAttr(attributes);
            }
            else
            {
                AddAttr(attributes);
            }

            SaveAssesment();
            Close();
        }

        private async void SaveAssesment()
        {
            var service = Resolve<ITap2HelpService>();
            await service.Save(_assesment);
        }

        private void UpdateAttr(List<Attribute> attributes)
        {
            var attribute = attributes.Find(att => att.Key == SelectedItem.Caption && att.Value == _originalValue);
            attribute.Value = AttributeValue;
        }

        private void AddAttr(List<Attribute> attributes)
        {
            var attribute = new Attribute
            {
                Key = SelectedItem.Caption,
                Value = AttributeValue
            };
            attributes.Add(attribute);
        }
    }
}
