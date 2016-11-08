using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Acr.UserDialogs;
using Kastil.Common.Models;
using Kastil.Common.Services;
using Kastil.Common.Utils;
using Kastil.Common.ViewModels;
using Kastil.Core.Events;
using Kastil.Core.Services;
using MvvmCross.Core.ViewModels;
using MvvmCross.Platform;

namespace Kastil.Core.ViewModels
{
    public abstract class ItemListViewModel : BaseViewModel 
    {
		public ObservableRangeCollection<AttributedListItemViewModel> Items { get; } = new ObservableRangeCollection<AttributedListItemViewModel>();
        public string DisasterId { get; private set; }
        
        public void Init(string disasterId)
        {
            DisasterId = disasterId;
        }

        public override Task Initialize()
        {
            Subscribe<EditingDoneEvent>(async e => await OnEditingDone(e));
            return Load();
        }

        protected async Task Load()
        {
            var dialog = Resolve<IUserDialogs>();
            dialog.ShowLoading(Messages.General.Loading);            
            try
            {
                var rawItems = await GetItems();
                if (rawItems != null)
                {
                    Items.AddRange(rawItems.Select(s => new AttributedListItemViewModel(s)));
                }
            }
            catch (Exception ex)
            {
                dialog.HideLoading();
                Mvx.Trace($"Unable to load {ItemType} list, exception: {ex}");
                await dialog.AlertAsync($"Unable to load {ItemType} list. Please try again");
            }
            finally
            {
                dialog.HideLoading();
            }
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

        MvxCommand<AttributedListItemViewModel> _itemSelectedCommand;
        public MvxCommand<AttributedListItemViewModel> ItemSelectedCommand
        {
            get
            {
                _itemSelectedCommand = _itemSelectedCommand ?? new MvxCommand<AttributedListItemViewModel>(DoItemSelectedCommand);
                return _itemSelectedCommand;
            }
        }

        protected abstract void DoItemSelectedCommand(AttributedListItemViewModel itemVm);
        
        protected abstract Task<IEnumerable<Item>> GetItems();
        protected abstract string ItemType { get; }

    }
}
