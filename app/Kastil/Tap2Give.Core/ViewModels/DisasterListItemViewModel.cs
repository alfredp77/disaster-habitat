﻿using System;
using Kastil.Common.Models;

namespace Tap2Give.Core.ViewModels
{
    public class DisasterListItemViewModel
    {
        public DisasterListItemViewModel(Disaster value)
        {
            Value = value;
        }

        public Disaster Value { get; }

        public string Name => Value.Name;
        public string Location => Value.Location;
        public string Description => Value.Description;
		public string ImageUrl => Value.ImageUrl;
        public DateTimeOffset? When => Value.DateWhen;
    }
}
