﻿using Acr.UserDialogs;
using Kastil.Core.Events;
using Kastil.Core.Services;
using Kastil.Core.Utils;
using Kastil.Shared.Models;
using MvvmCross.Core.ViewModels;
using MvvmCross.Platform;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kastil.Core.ViewModels
{
    public class ItemListViewModel<T> : BaseViewModel where T : BaseViewModel
    {
        public ObservableRangeCollection<T> Items { get; set; }
        public string DisasterId { get; set; }

        public void Init(string disasterId)
        {
            DisasterId = disasterId;
        }

        public override Task Initialize()
        {
            Subscribe<EditingDoneEvent>(async e => await OnEditingDone(e));
            return Load();
        }

        protected virtual async Task Load()
        {
            
        }

        private async Task OnEditingDone(EditingDoneEvent evt)
        {
            if (evt.Sender is ShelterViewModel || evt.Sender is AssessmentViewModel)
                await DoRefreshCommand();
        }

        MvxAsyncCommand _refreshCommand;
        public MvxAsyncCommand RefreshCommand
        {
            get
            {
                _refreshCommand = _refreshCommand ?? new MvxAsyncCommand(DoRefreshCommand);
                return _refreshCommand;
            }
        }

        protected async Task DoRefreshCommand()
        {
            IsLoading = true;
            try
            {
                Items.Clear();
                await Load();
            }
            finally
            {
                IsLoading = false;
            }
        }

        private bool _isLoading;
        public bool IsLoading
        {
            get { return _isLoading; }
            set { SetProperty(ref _isLoading, value); }
        }
    }
}
