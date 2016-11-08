using Kastil.Common.Models;
using Kastil.Common.ViewModels;

namespace Kastil.Core.ViewModels
{
    public class ShelterListItemViewModel: BaseViewModel
    {
        public ShelterListItemViewModel(Shelter value)
        {
            Value = value;
        }

        public Shelter Value { get; set; }

        public string DisasterId => Value.DisasterId;
        public string ShelterId => Value.Id;
        public string Text => Value.Name;
        public string LocationName => Value.LocationName;
    }

    public class AttributedListItemViewModel : BaseViewModel
    {
        public AttributedListItemViewModel(Item value)
        {
            Value = value;
        }

        public Item Value { get; }

        public string DisasterId => Value.DisasterId;
        public string ShelterId => Value.Id;
        public string Text => Value.Name;
        public string LocationName => Value.LocationName;
    }
}
