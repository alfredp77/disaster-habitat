using System;
using Kastil.Common.ViewModels;
using Kastil.Shared.Models;

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