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
                return _context.Item.Name;                
            }
            set
            {
                _context.Item.Name = value;
                SetTitle();
                RaisePropertyChanged();
            }
        }        

        public string LocationName
        {
            get
            {
                var location = _context.Item.LocationName;
				if (_context.IsNew)
					return location;
				if (string.IsNullOrEmpty (location))
					return $"({Messages.General.Unknown}";
				return location;

            }
            set
            {
                _context.Item.LocationName = value;
                RaisePropertyChanged();
            }
        }

        public bool AddMode => _context.IsNew;
        
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
                await _context.CommitChanges();
				Publish(new EditingDoneEvent (this, EditAction.Edit));
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

        private IShelterEditContext _context => Resolve<IShelterEditContext>();

        private void SetShelterProperties()
        {
            var shelter = _context.Item;
            shelter.Name = Name;
            shelter.LocationName = LocationName;
        }

        public MvxCommand CancelCommand => new MvxCommand (Close);

        public ObservableRangeCollection<AttributeViewModel> Attributes { get; } = new ObservableRangeCollection<AttributeViewModel>();
        
        public Task Initialize()
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
                var shelter = _context.Item;
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
