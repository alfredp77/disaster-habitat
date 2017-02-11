using System;
using MvvmCross.Core.ViewModels;

namespace Kastil.Common.Events
{
    public class PopToViewModelPresentationHint : MvxPresentationHint
    {
        public Type ViewModelType { get; set; }

        public PopToViewModelPresentationHint(Type viewModelType)
        {
            ViewModelType = viewModelType;
        }
    }
}