using System;
using System.Linq;
using System.Threading.Tasks;
using Acr.UserDialogs;
using Kastil.Common.Models;
using Kastil.Common.Services;
using Kastil.Common.Utils;
using Kastil.Common.ViewModels;
using MvvmCross.Core.ViewModels;
using MvvmCross.Platform;
using Tap2Give.Core.Services;

namespace Tap2Give.Core.ViewModels
{
    public class DisastersViewModel : BaseViewModel
    {
		public ObservableRangeCollection<DisasterListItemViewModel> DisasterIncidents { get; } = new ObservableRangeCollection<DisasterListItemViewModel>();

        public override Task Initialize()
        {
            Title = Messages.MainTitle;
            AllowCancelCommand = false;
            return LoadIncidents();
        }

        private async Task LoadIncidents()
        {
            var dialog = Mvx.Resolve<IUserDialogs>();
            dialog.ShowLoading(Messages.Loading);

            var disasterService = Mvx.Resolve<ITap2GiveService>();
            try
            {
                var disasters = await disasterService.GetDisasters(true);
                if (disasters != null)
                {
					DisasterIncidents.AddRange(disasters.Where(d => !string.IsNullOrEmpty(d.GiveUrl))
					                           .Select(d => new DisasterListItemViewModel(d)));
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

        public MvxCommand<DisasterListItemViewModel> ShowDetailsCommand 
        {
            get
            {
                return new MvxCommand<DisasterListItemViewModel>(DoShowDetails);
            }
        }

		private void DoShowDetails(DisasterListItemViewModel selectedItem)
        {
            var disasterContext = Resolve<IDisasterContext>();
            disasterContext.Initialize(selectedItem.Value);
            ShowViewModel<DisasterDetailsViewModel>();
        }
    }
}
