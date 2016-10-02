using Acr.UserDialogs;
using Kastil.Core.Events;
using Kastil.Core.Services;
using Kastil.Core.Utils;
using Kastil.Shared.Models;
using MvvmCross.Core.ViewModels;
using MvvmCross.Platform;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Kastil.Core.ViewModels
{
    public class ShelterListViewModel : BaseViewModel
    {
        public ObservableRangeCollection<ShelterListItemViewModel> Items { get; } = new ObservableRangeCollection<ShelterListItemViewModel>();

        public string DisasterId { get; set; }
        public string AssessmentId { get; set; }
        public bool Selected { get; set; }

        public ShelterListViewModel()
        {
            Title = "Shelters";
            AllowAddCommand = true;
        }

        public void Init(string disasterId)
        {
            DisasterId = disasterId;
        }

        public Task Initialize()
        {
            Subscribe<EditingDoneEvent>(async e => await OnEditingDone(e));
            return Load();
        }

        private async Task OnEditingDone(EditingDoneEvent evt)
        {
            if (evt.Sender is ShelterViewModel)
                await DoRefreshCommand();
        }

        private async Task Load()
        {
            var dialog = Resolve<IUserDialogs>();
            dialog.ShowLoading(Messages.General.Loading);

            var disasterService = Resolve<ITap2HelpService>();
            try
            {
                var shelters = await disasterService.GetShelters(DisasterId, "");
                if (shelters != null)
                    Items.AddRange(shelters.Select(s => new ShelterListItemViewModel(s)));
            }
            catch (Exception ex)
            {
                dialog.HideLoading();
                Mvx.Trace("Unable to load Shelters, exception: {0}", ex);
                await dialog.AlertAsync("Unable to load Shelters. Please try again");
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

        private IShelterEditContext _context => Resolve<IShelterEditContext>();
        public MvxAsyncCommand LinkCommand => new MvxAsyncCommand(DoLinkCommand);
        private async Task DoLinkCommand()
        {
            var dialog = Resolve<IUserDialogs>();
            dialog.ShowLoading(Messages.General.Saving);

            try
            {
                SetShelterProperties();
                await _context.CommitChanges();
                Publish(new EditingDoneEvent(this, EditAction.Edit));
                dialog.ShowSuccess(Messages.General.ShelterSaved);
                Close();
            }
            catch (Exception ex)
            {
                dialog.HideLoading();
                Mvx.Trace("Unable to link Shelters to Disaster, exception: {0}", ex);
                await dialog.AlertAsync("Unable to link Shelters to Disaster. Please try again");
                Close();
            }
            finally
            {
                dialog.HideLoading();
            }
        }

        private void SetShelterProperties()
        {
            //TODO: ForEach Selected Shelter... Associate DisasterId with this Shelter
        }

        MvxCommand<ShelterListItemViewModel> _shelterSelectedCommand;
        public MvxCommand<ShelterListItemViewModel> ShelterSelectedCommand
        {
            get
            {
                _shelterSelectedCommand = _shelterSelectedCommand ?? new MvxCommand<ShelterListItemViewModel>(OnShelterSelectedCommand);
                return _shelterSelectedCommand;
            }
        }
        
        private void OnShelterSelectedCommand(ShelterListItemViewModel itemVm)
        {
            var context = Resolve<IShelterEditContext>();
            context.Initialize(itemVm.Value);
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