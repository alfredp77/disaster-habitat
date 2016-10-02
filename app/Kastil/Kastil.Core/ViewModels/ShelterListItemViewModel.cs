﻿using Kastil.Shared.Models;

namespace Kastil.Core.ViewModels
{
    public class ShelterListItemViewModel: BaseViewModel
    {
        public ShelterListItemViewModel(Shelter value)
        {
            Value = value;
        }

        public Shelter Value { get; }

        public string DisasterId => Value.DisasterId;
        public string AssessmentId => Value.AssessmentId;
        public string ShelterId => Value.Id;
        public string Text => Value.Name;
        public string LocationName => Value.LocationName;

        private bool _isChecked;
        public bool IsChecked
        {
            get { return _isChecked; }
            set
            {
                _isChecked = value;
                RaisePropertyChanged(() => IsChecked);
            }
        }

    }
}
