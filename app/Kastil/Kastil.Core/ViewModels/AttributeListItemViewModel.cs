using Kastil.Common.ViewModels;
using Kastil.Common.Models;

namespace Kastil.Core.ViewModels
{
    public class AttributeListItemViewModel : BaseViewModel
    {
        public Attribute Attribute { get; }

        public AttributeListItemViewModel(Attribute attribute)
        {
            Attribute = attribute;
        }

        public string AttributeName => Attribute.Key;

        public string AttributeValue
        {
            get { return Attribute.Value; }
            set
            {
                Attribute.Value = value;
                RaisePropertyChanged();
            }
        }
    }
}
