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
        public ObservableRangeCollection<DisasterListItemViewModel> Items { get; } = new ObservableRangeCollection<DisasterListItemViewModel>();

        public DisastersViewModel()
        {
            Title = "Disasters";
			AllowSettingCommand = true;
        }


        public async Task Initialize()
        {
            Items.Clear();
            await Load();
        }

        private async Task Load()
        {
            var dialog = Resolve<IUserDialogs>();
            dialog.ShowLoading(Messages.General.Loading);

            var disasterService = Resolve<ITap2HelpService>();
            try
            {
                var disasters = await disasterService.GetDisasters();
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
			ShowViewModel<AssesmentListViewModel> (new { disasterId = disaster.Id });
		}

		private void DoShowShelters (Disaster disaster)
		{
			ShowViewModel<SheltersViewModel> (new { disasterId = disaster.Id });
		}

        protected override async Task DoSettingCommand()
        {
            ShowViewModel<SyncViewModel>();
        }
    }
}