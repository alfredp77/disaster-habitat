using Kastil.Shared.Models;

namespace Kastil.Core.ViewModels
{
    public class AttributeViewModel
    {
        private readonly Attribute _attribute;

        public AttributeViewModel(Attribute attribute)
        {
            _attribute = attribute;
        }

        public string AttributeName => _attribute.Key;
        public string AttributeValue => _attribute.Value;
    }
}
