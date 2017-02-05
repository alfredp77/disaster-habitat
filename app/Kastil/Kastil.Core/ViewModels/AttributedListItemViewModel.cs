using Kastil.Common.Models;
using Kastil.Common.ViewModels;

namespace Kastil.Core.ViewModels
{
    public class AttributedListItemViewModel : BaseViewModel
    {
        public AttributedListItemViewModel(Attributed value)
        {
            Value = value;
        }

        public Attributed Value { get; }

        public string DisasterId => Value.DisasterId;
        public string ShelterId => Value.ObjectId;
        public string Text => Value.Name;
        public string LocationName => Value.Location;
    }
}
