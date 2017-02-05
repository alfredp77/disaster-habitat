using System;
using System.Linq;
using System.Threading.Tasks;
using Acr.UserDialogs;
using Kastil.Common.Services;
using Kastil.Common.Utils;
using Kastil.Common.ViewModels;
using Kastil.Common.Models;
using Kastil.Core.Services;
using MvvmCross.Core.ViewModels;
using MvvmCross.Platform;
using Kastil.Core.Events;

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

        public override Task Initialize()
        {
			Subscribe<SyncCompletedEvent>(async e => await OnSyncCompleted(e));
			return Reload();
        }

		private async Task OnSyncCompleted(SyncCompletedEvent evt)
		{
			if (evt.Successful) 
			{
				Items.Clear();
				await Load();
			}
		}

		private Task Reload()
		{
			Items.Clear();
			return Load();
		}
		private async Task Load()
        {
            var dialog = Resolve<IUserDialogs>();
            dialog.ShowLoading(Messages.General.Loading);
            try
            {
            	var disasterService = Resolve<ITap2HelpService>();

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
			get
            {
				_disasterSelectedCommand = _disasterSelectedCommand ?? new MvxCommand<DisasterListItemViewModel>(DoDisasterSelectedCommand);
				return _disasterSelectedCommand;
			}
		}

		private void DoDisasterSelectedCommand (DisasterListItemViewModel itemVm)
		{
			var actionSheetConfig = new ActionSheetConfig ();
			actionSheetConfig.Add (Messages.DisasterMenu.Assessment, () => DoShowAssessment(itemVm.Value));
			actionSheetConfig.Add (Messages.DisasterMenu.Shelters, () => DoShowShelters(itemVm.Value));

			var dialog = Resolve<IUserDialogs> ();
			dialog.ActionSheet (actionSheetConfig);
		}

		private void DoShowAssessment(Disaster disaster)
		{
		    var context = Resolve<AttributedListContext>();
		    var handler = Resolve<AssessmentListHandler>();
            context.Initialize(handler, disaster.ObjectId);
			ShowViewModel<AttributedListViewModel>();
		}

		private void DoShowShelters (Disaster disaster)
		{
            var context = Resolve<AttributedListContext>();
            var handler = Resolve<ShelterListHandler>();
            context.Initialize(handler, disaster.ObjectId);
            ShowViewModel<AttributedListViewModel>();
		}

        protected override async Task DoSettingCommand()
        {
            ShowViewModel<SyncViewModel>();
        }
    }
}