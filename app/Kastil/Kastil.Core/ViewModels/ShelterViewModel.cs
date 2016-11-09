using System;
using Kastil.Common.Models;
using Kastil.Core.Services;

namespace Kastil.Core.ViewModels
{
    public class ShelterViewModel : AttributedViewModel
    {
        public ShelterViewModel(ShelterEditContext context) : base(context)
        {
        }

        public override string NamePlaceholderText => "Enter shelter name";
        public override string LocationPlaceholderText => "Where is this shelter located?";
        public override string ItemType => typeof (Shelter).Name;
        protected override void NavigateToEditScreen()
        {
			ShowViewModel<EditShelterAttributeViewModel>();
        }
    }
}