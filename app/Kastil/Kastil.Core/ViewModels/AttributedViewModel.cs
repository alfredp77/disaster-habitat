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
    public class AttributedViewModel : BaseViewModel
    {
		private AttributedEditContext _context;
		public AttributedViewModel()
		{
			_context = Resolve<AttributedEditContext>();
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

        public string NamePlaceholderText
        {
			get { return _context.ItemHandler.NamePlaceholderText; }
        }

        public string Location
        {
			get { return _context.ItemLocation; }
			set 
			{
				_context.ItemLocation = value;
				RaisePropertyChanged();
			}
        }

        public string LocationPlaceholderText
        {
			get { return _context.ItemHandler.LocationPlaceholderText; }
        }

        public bool AddMode
        {
			get { return _context.IsNew; }
        }

        public ICommand AddAttributeCommand => new MvxCommand(DoAddAttrCommand);
        private void DoAddAttrCommand()
        {
			_context.SelectedAttribute = null;
            ShowViewModel<EditAttributedAttributesViewModel>();
        }

        public MvxCommand<AttributeListItemViewModel> AttributeSelectedCommand => new MvxCommand<AttributeListItemViewModel>(DoAttributeSelectedCommand);
        private void DoAttributeSelectedCommand(AttributeListItemViewModel obj)
        {
			_context.SelectedAttribute = obj.Attribute;
            ShowViewModel<EditAttributedAttributesViewModel>();
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
				dialog.ShowSuccess($"{_context.ItemType} {Messages.General.SavedSuccessfully}");
                Close();
            }
            catch (Exception ex)
            {
                dialog.HideLoading();
                Mvx.Trace($"Unable to save {_context.ItemType}, exception: {ex}");
                await dialog.AlertAsync($"Unable to save {_context.ItemType}. Please try again");
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
			Title = string.IsNullOrEmpty(Name) ? "New Assessment" : Name;
        }
    }
}
