using Kastil.Shared.Models;
using MvvmCross.Core.ViewModels;
using System.Windows.Input;
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
}
