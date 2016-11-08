using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
    public abstract class AttributedItemViewModel : BaseViewModel
    {
        private IItemEditContext _context;

        protected AttributedItemViewModel(IItemEditContext context)
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

        public MvxCommand<AttributeViewModel> AttributeSelectedCommand => new MvxCommand<AttributeViewModel>(DoAttributeSelectedCommand);
        private void DoAttributeSelectedCommand(AttributeViewModel obj)
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
        public ObservableRangeCollection<AttributeViewModel> Attributes { get; } = new ObservableRangeCollection<AttributeViewModel>();

        public override Task Initialize()
        {
            Subscribe<EditingDoneEvent>(async e => await OnEditingDone(e));
            return Load();
        }

        private async Task OnEditingDone(EditingDoneEvent evt)
        {
            if (evt.Sender is EditAttributedItemViewModel)
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
                Attributes.AddRange(_context.Attributes.Select(a => new AttributeViewModel(a)));
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
