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

namespace Tap2Give.Core.ViewModels
{
    public class DisasterIncidentsViewModel : BaseViewModel
    {
        public ObservableRangeCollection<DisasterIncidentViewModel> DisasterIncidents { get; set; }

        public override Task Initialize()
        {
            Title = Messages.MainTitle;
            return LoadIncidents();
        }

        private async Task LoadIncidents()
        {
            var dialog = Mvx.Resolve<IUserDialogs>();
            dialog.ShowLoading(Messages.Loading);

            var disasterService = Mvx.Resolve<ITap2HelpService>();
            try
            {
                var disasters = await disasterService.GetDisasters();
                if (disasters != null)
                {
                    DisasterIncidents.AddRange(disasters.Select(d => new DisasterIncidentViewModel(d)));
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

        public MvxCommand<DisasterIncidentViewModel> IncidentSelectedCommand 
        {
            get
            {
                return new MvxCommand<DisasterIncidentViewModel>(d => ShowViewModel<DisasterIncidentAidViewModel>(d.Value));
            }
        }

        private void DoShowIncidentAid(Disaster disaster)
        {
            ShowViewModel<DisasterIncidentAidViewModel>(disaster);
        }
    }
}
