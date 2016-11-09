using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Acr.UserDialogs;
using Kastil.Common.Utils;
using Kastil.Common.ViewModels;
using Kastil.Core.Events;
using Kastil.Core.Services;
using MvvmCross.Core.ViewModels;
using MvvmCross.Platform;

namespace Kastil.Core.ViewModels
{
    public abstract class AttributedViewModel : BaseViewModel
    {
        private IItemEditContext _context;

        protected AttributedViewModel(IItemEditContext context)
        {
            _context = context;
        }


        public string Name
        {
            get
            {
                return _context.ItemName;
            }
            set
            {
                _context.ItemName = value;
                SetTitle();
                RaisePropertyChanged();
            }
        }

        public abstract string NamePlaceholderText { get; }


        public string Location
        {
            get
            {
                return _context.ItemLocation;
            }
            set
            {
                _context.ItemLocation = value;
                RaisePropertyChanged();
            }
        }

        public abstract string LocationPlaceholderText { get; }
        public abstract string ItemType { get; }

        public bool AddMode => _context.IsNew;

        public ICommand AddAttributeCommand => new MvxCommand(DoAddAttrCommand);
        private void DoAddAttrCommand()
        {
            _context.SelectedAttribute = null;
			NavigateToEditScreen();
        }

        public MvxCommand<AttributeListItemViewModel> AttributeSelectedCommand => new MvxCommand<AttributeListItemViewModel>(DoAttributeSelectedCommand);
        private void DoAttributeSelectedCommand(AttributeListItemViewModel obj)
        {
            _context.SelectedAttribute = obj.Attribute;
			NavigateToEditScreen();
        }
        
        public MvxAsyncCommand SaveCommand => new MvxAsyncCommand(DoSaveCommand);
        private async Task DoSaveCommand()
        {
            var dialog = Resolve<IUserDialogs>();
            dialog.ShowLoading(Messages.General.Saving);
        
            try
            {
                await _context.CommitChanges();
                Publish(new EditingDoneEvent(this, EditAction.Edit));
                dialog.ShowSuccess($"{ItemType} {Messages.General.SavedSuccessfully}");
                Close();
            }
            catch (Exception ex)
            {
                dialog.HideLoading();
                Mvx.Trace($"Unable to save {ItemType}, exception: {ex}");
                await dialog.AlertAsync($"Unable to save {ItemType}. Please try again");
                Close();
            }
            finally
            {
                dialog.HideLoading();
            }
        }
        

        
        public MvxCommand CancelCommand => new MvxCommand(Close);
        public ObservableRangeCollection<AttributeListItemViewModel> Attributes { get; } = new ObservableRangeCollection<AttributeListItemViewModel>();

        public override Task Initialize()
        {
            Subscribe<EditingDoneEvent>(async e => await OnEditingDone(e));
            return Load();
        }

        private async Task OnEditingDone(EditingDoneEvent evt)
        {
            if (evt.Sender is EditAttributedAttributesViewModel)
                await Load();
        }
        		
        private async Task Load()
        {
            var dialog = Resolve<IUserDialogs>();
            dialog.ShowLoading(Messages.General.Loading);
            try
            {
                SetTitle();
                Attributes.Clear();
                Attributes.AddRange(_context.Attributes.Select(a => new AttributeListItemViewModel(a)));
            }
            catch (Exception ex)
            {
                dialog.HideLoading();
                Mvx.Trace("Unable to load Assessment, exception: {0}", ex);
                await dialog.AlertAsync("Unable to load Assessment. Please try again");
                Close();
            }
            finally
            {
                dialog.HideLoading();
            }
        }
        
        private void SetTitle()
        {
            Title = Name;
        }

        protected abstract void NavigateToEditScreen();
        

    }
}
