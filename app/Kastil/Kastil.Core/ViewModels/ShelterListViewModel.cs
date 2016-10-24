using Acr.UserDialogs;
using Kastil.Core.Services;
using Kastil.Core.Utils;
using MvvmCross.Core.ViewModels;
using MvvmCross.Platform;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Kastil.Core.ViewModels
{
    public class ShelterListViewModel : ItemListViewModel<ShelterListItemViewModel>
    {
        public ShelterListViewModel()
        {
            Title = "Shelters";
            AllowAddCommand = true;
            Items = new ObservableRangeCollection<ShelterListItemViewModel>();
        }

        protected override async Task Load()
        {
            var dialog = Resolve<IUserDialogs>();
            dialog.ShowLoading(Messages.General.Loading);

            var disasterService = Resolve<ITap2HelpService>();
            try
            {
                var shelters = await disasterService.GetShelters(DisasterId);
                if (shelters != null)
                {
                    Items.AddRange(shelters.Select(s => new ShelterListItemViewModel(s)));
                }
            }
            catch (Exception ex)
            {
                dialog.HideLoading();
                Mvx.Trace("Unable to load Shelter list, exception: {0}", ex);
                await dialog.AlertAsync("Unable to load Shelter list. Please try again");
            }
            finally
            {
                dialog.HideLoading();
            }
        }

        MvxCommand<ShelterListItemViewModel> _shelterSelectedCommand;
        public MvxCommand<ShelterListItemViewModel> ShelterSelectedCommand
        {
            get
            {
                _shelterSelectedCommand = _shelterSelectedCommand ?? new MvxCommand<ShelterListItemViewModel>(DoShelterSelectedCommand);
                return _shelterSelectedCommand;
            }
        }

        private void DoShelterSelectedCommand(ShelterListItemViewModel itemVm)
        {
            var context = Resolve<IShelterEditContext>();
            context.Initialize(itemVm.Value, DisasterId);
            ShowViewModel<ShelterViewModel>();
        }

        protected override Task DoAddCommand()
        {
            return Task.Run(() =>
            {
                var context = Resolve<IShelterEditContext>();
                context.Initialize(disasterId: DisasterId);
                ShowViewModel<ShelterViewModel>();
            });
        }
    }
}
