using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Acr.UserDialogs;
using Kastil.Core.Events;
using Kastil.Core.Services;
using Kastil.Core.Utils;
using MvvmCross.Core.ViewModels;
using MvvmCross.Platform;

namespace Kastil.Core.ViewModels
{
    public class ShelterViewModel : BaseViewModel
    {
        public string Name
        {
            get
            {
                return Context.Item.Name;
            }
            set
            {
                Context.Item.Name = value;
                SetTitle();
                RaisePropertyChanged();
            }
        }

        public string NamePlaceholderText
        {
            get { return Messages.Placeholders.ShelterName; }
        }

        public string Location
        {
            get
            {
                return Context.Item.LocationName;
            }
            set
            {
                Context.Item.LocationName = value;
                RaisePropertyChanged();
            }
        }

        public string LocationPlaceholderText
        {
            get
            {
                if (Context.IsNew)
                    return Messages.Placeholders.ShelterLocation;
                return Messages.General.Unknown;
            }
        }

        public bool AddMode => Context.IsNew;

        public ICommand AddAttributeCommand => new MvxCommand(DoAddAttrCommand);
        private void DoAddAttrCommand()
        {
            var context = Resolve<IShelterEditContext>();
            context.SelectedAttribute = null;
            ShowViewModel<EditAttributeViewModel>();
        }

        public MvxCommand<AttributeViewModel> AttributeSelectedCommand => new MvxCommand<AttributeViewModel>(DoAttributeSelectedCommand);
        private void DoAttributeSelectedCommand(AttributeViewModel obj)
        {
            var context = Resolve<IShelterEditContext>();
            context.SelectedAttribute = obj.Attribute;
            ShowViewModel<EditAttributeViewModel>();
        }

        public MvxAsyncCommand SaveCommand => new MvxAsyncCommand(DoSaveCommand);
        private async Task DoSaveCommand()
        {
            var dialog = Resolve<IUserDialogs>();
            dialog.ShowLoading(Messages.General.Saving);

            try
            {
                SetShelterProperties();
                await Context.CommitChanges();
                Publish(new EditingDoneEvent(this, EditAction.Edit));
                dialog.ShowSuccess(Messages.General.ShelterSaved);
                Close();
            }
            catch (Exception ex)
            {
                dialog.HideLoading();
                Mvx.Trace("Unable to save Shelter, exception: {0}", ex);
                await dialog.AlertAsync("Unable to save Shelter. Please try again");
                Close();
            }
            finally
            {
                dialog.HideLoading();
            }
        }

        private IShelterEditContext Context => Resolve<IShelterEditContext>();

        private void SetShelterProperties()
        {
            var shelter = Context.Item;
            shelter.Name = Name;
            shelter.LocationName = Location;
        }

        public MvxCommand CancelCommand => new MvxCommand(Close);

        public ObservableRangeCollection<AttributeViewModel> Attributes { get; } = new ObservableRangeCollection<AttributeViewModel>();

        public override Task Initialize()
        {
            Subscribe<EditingDoneEvent>(async e => await OnEditingDone(e));
            return Load();
        }

        private async Task OnEditingDone(EditingDoneEvent evt)
        {
            if (evt.Sender is EditAttributeViewModel)
                await Load();
        }

        private async Task Load()
        {
            var dialog = Resolve<IUserDialogs>();
            dialog.ShowLoading(Messages.General.Loading);
            try
            {
                var shelter = Context.Item;
                if (shelter != null)
                {
                    SetTitle();
                    Attributes.Clear();
                    Attributes.AddRange(shelter.Attributes.Select(a => new AttributeViewModel(a)));
                }
            }
            catch (Exception ex)
            {
                dialog.HideLoading();
                Mvx.Trace("Unable to load Shelter, exception: {0}", ex);
                await dialog.AlertAsync("Unable to load Shelter. Please try again");
                Close();
            }
            finally
            {
                dialog.HideLoading();
            }
        }

        private void SetTitle()
        {
            Title = string.IsNullOrEmpty(Name) ? Messages.General.DefaultNewShelterName : Name;
        }
    }
}
