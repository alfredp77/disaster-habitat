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
        private string _name;
        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;
                SetTitle();
                RaisePropertyChanged();
            }
        }

        private string _namePlaceholderText;
        public string NamePlaceholderText
        {
            get { return _namePlaceholderText; }
            private set { _namePlaceholderText = value; RaisePropertyChanged();}
        }

        private string _location;
        public string Location
        {
            get { return _location; }
            set
            {
                _location = value;
                RaisePropertyChanged();
            }
        }

        private string _locationPlaceholderText;
        private bool _addMode;

        public string LocationPlaceholderText
        {
            get { return _locationPlaceholderText; }
            private set { _locationPlaceholderText = value; RaisePropertyChanged(); }
        }

        public bool AddMode
        {
            get { return _addMode; }
            private set { _addMode = value; RaisePropertyChanged();}
        }

        public ICommand AddAttributeCommand => new MvxCommand(DoAddAttrCommand);
        private void DoAddAttrCommand()
        {
            var context = Resolve<AttributedEditContext>();
            context.SelectedAttribute = null;
            ShowViewModel<EditAttributedAttributesViewModel>();
        }

        public MvxCommand<AttributeListItemViewModel> AttributeSelectedCommand => new MvxCommand<AttributeListItemViewModel>(DoAttributeSelectedCommand);
        private void DoAttributeSelectedCommand(AttributeListItemViewModel obj)
        {
            var context = Resolve<AttributedEditContext>();
            context.SelectedAttribute = obj.Attribute;
            ShowViewModel<EditAttributedAttributesViewModel>();
        }
        
        public MvxAsyncCommand SaveCommand => new MvxAsyncCommand(DoSaveCommand);
        private async Task DoSaveCommand()
        {
            var dialog = Resolve<IUserDialogs>();
            dialog.ShowLoading(Messages.General.Saving);

            var context = Resolve<AttributedEditContext>();
            try
            {
                await context.CommitChanges();
                Publish(new EditingDoneEvent(this, EditAction.Edit));
                dialog.ShowSuccess($"{context.ItemType} {Messages.General.SavedSuccessfully}");
                Close();
            }
            catch (Exception ex)
            {
                dialog.HideLoading();
                Mvx.Trace($"Unable to save {context.ItemType}, exception: {ex}");
                await dialog.AlertAsync($"Unable to save {context.ItemType}. Please try again");
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
            var context = Resolve<AttributedEditContext>();
            try
            {
                Name = context.ItemName;
                Location = context.ItemLocation;
                NamePlaceholderText = context.NamePlaceholderText;
                LocationPlaceholderText = context.LocationPlaceholderText;
                AddMode = context.IsNew;
                SetTitle();
                Attributes.Clear();
                Attributes.AddRange(context.Attributes.Select(a => new AttributeListItemViewModel(a)));
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
    }
}
