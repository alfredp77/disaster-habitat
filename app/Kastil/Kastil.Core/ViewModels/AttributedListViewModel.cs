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
    public class AttributedListViewModel : BaseViewModel 
    {
		public ObservableRangeCollection<AttributedListItemViewModel> Items { get; } = new ObservableRangeCollection<AttributedListItemViewModel>();

        public override Task Initialize()
        {
            Subscribe<EditingDoneEvent>(async e => await OnEditingDone(e));
            return Load();
        }

        protected async Task Load()
        {
            var dialog = Resolve<IUserDialogs>();
            dialog.ShowLoading(Messages.General.Loading);

            var context = Resolve<AttributedListContext>();
            try
            {
                var rawItems = await context.Load();
                if (rawItems != null)
                {
                    Items.AddRange(rawItems.Select(s => new AttributedListItemViewModel(s)));
                }
            }
            catch (Exception ex)
            {
                dialog.HideLoading();
                Mvx.Trace($"Unable to load {context.ItemType} list, exception: {ex}");
                await dialog.AlertAsync($"Unable to load {context.ItemType} list. Please try again");
            }
            finally
            {
                dialog.HideLoading();
            }
        }

        private async Task OnEditingDone(EditingDoneEvent evt)
        {
            if (evt.Sender is AttributedViewModel)
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

        MvxAsyncCommand<AttributedListItemViewModel> _itemSelectedCommand;
        public MvxAsyncCommand<AttributedListItemViewModel> ItemSelectedCommand
        {
            get
            {
                _itemSelectedCommand = _itemSelectedCommand ?? new MvxAsyncCommand<AttributedListItemViewModel>(DoItemSelectedCommand);
                return _itemSelectedCommand;
            }
        }

        private async Task DoItemSelectedCommand(AttributedListItemViewModel itemVm)
        {
            var context = Resolve<AttributedListContext>();
            var handler = context.CreateItemHandler(itemVm.Value);

            var editContext = Resolve<AttributedEditContext>();
			await editContext.Initialize(handler);
            ShowViewModel<AttributedViewModel>();
        }
    }
}
