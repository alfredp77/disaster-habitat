using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Acr.UserDialogs;
using Kastil.Core.Services;
using Kastil.Core.Utils;
using Kastil.Shared.Models;
using MvvmCross.Core.ViewModels;
using MvvmCross.Platform;

namespace Kastil.Core.ViewModels
{
    public class DisastersViewModel : BaseViewModel
    {
        public ObservableRangeCollection<DisasterListItemViewModel> Items { get; }

        public DisastersViewModel()
        {
            Items = new ObservableRangeCollection<DisasterListItemViewModel>();
        }


        public Task Initialize()
        {
            _nextPage = 0;
            Items.Clear();
            return Load();
        }

        private int _nextPage;
         
        private async Task Load()
        {
            var dialog = Resolve<IUserDialogs>();
            dialog.ShowLoading(Messages.General.Loading);

            var disasterService = Resolve<IDisasterService>();
            try
            {
                var disasters = await disasterService.Load(new DisasterFilter(), _nextPage);
                if (disasters != null)
                {
                    Items.AddRange(disasters.Select(d => new DisasterListItemViewModel(d)));
                }
            }
            catch (Exception ex)
            {
                dialog.HideLoading();
                Mvx.Trace("Unable to load Disasters list, exception: {0}", ex);
                await dialog.AlertAsync("Unable to load Disasters list. Please try again");
            }
            finally
            {
                dialog.HideLoading();
            }
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

        private async Task DoRefreshCommand()
        {
            IsLoading = true;
            try
            {
                await Initialize();
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

        MvxAsyncCommand _loadMoreCommand;
        public MvxAsyncCommand LoadMoreCommand
        {
            get
            {
                _loadMoreCommand = _loadMoreCommand ?? new MvxAsyncCommand(DoLoadMoreCommand);
                return _loadMoreCommand;
            }
        }

        private Task DoLoadMoreCommand()
        {
            _nextPage++;
            return Load();
        }

		MvxCommand<DisasterListItemViewModel> _disasterSelectedCommand;
		public MvxCommand<DisasterListItemViewModel> DisasterSelectedCommand {
			get {
				_disasterSelectedCommand = _disasterSelectedCommand ?? new MvxCommand<DisasterListItemViewModel>(DoDisasterSelectedCommand);
				return _disasterSelectedCommand;
			}
		}

		private void DoDisasterSelectedCommand (DisasterListItemViewModel itemVm)
		{
			var actionSheetConfig = new ActionSheetConfig ();
			actionSheetConfig.Add (Messages.DisasterMenu.Assesment, () => DoShowAssesment(itemVm.Value));
			actionSheetConfig.Add (Messages.DisasterMenu.Shelters, () => DoShowShelters(itemVm.Value));

			var dialog = Resolve<IUserDialogs> ();
			dialog.ActionSheet (actionSheetConfig);
		}

		private void DoShowAssesment(Disaster disaster)
		{
			ShowViewModel<AssesmentViewModel> (new { disasterId = disaster.Id });
		}

		private void DoShowShelters (Disaster disaster)
		{
			ShowViewModel<SheltersViewModel> (new { disasterId = disaster.Id });
		}

	}
}