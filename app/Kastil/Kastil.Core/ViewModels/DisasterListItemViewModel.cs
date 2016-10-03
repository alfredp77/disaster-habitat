using System;
using System.Threading.Tasks;
using Acr.UserDialogs;
using Kastil.Shared.Models;
using MvvmCross.Core.ViewModels;

namespace Kastil.Core.ViewModels
{
    public class DisasterListItemViewModel : BaseViewModel
    {
        public DisasterListItemViewModel(Disaster value)
        {
            Value = value;
        }

        public Disaster Value { get; }

        public string Text => Value.Name;
        public DateTimeOffset? When => Value.DateWhen;

    }
}