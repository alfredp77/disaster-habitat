using Kastil.Common.ViewModels;
using Kastil.Common.Models;

namespace Kastil.Core.ViewModels
{
    public class ValuedAttributeListItemViewModel : BaseViewModel
    {
        public ValuedAttribute Item { get; }

        public ValuedAttributeListItemViewModel(ValuedAttribute item)
        {
            Item = item;
        }

        public string Key => Item.Key;

        public string Value
        {
            get { return Item.Value; }
            set
            {
                Item.Value = value;
                RaisePropertyChanged();
            }
        }
    }
}
