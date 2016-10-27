using Kastil.Common.ViewModels;
using Kastil.Shared.Models;

namespace Kastil.Core.ViewModels
{
    public class AttributeViewModel : BaseViewModel
    {
        public Attribute Attribute { get; }

        public AttributeViewModel(Attribute attribute)
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
