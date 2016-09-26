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
using MvvmCross.Plugins.Messenger;

namespace Kastil.Core.ViewModels
{
    public class AssessmentViewModel : BaseViewModel
    {
        private string _name;
        public string Name
        {
            get { return _name; }
            set { _name = value; RaisePropertyChanged(); }
        }

        private string _location;
        public string Location
        {
            get { return _location; }
            set { _location = value; RaisePropertyChanged(); }
        }

        public ICommand AddAttributeCommand => new MvxCommand(DoAddAttrCommand);
        private void DoAddAttrCommand()
        {
            var context = Resolve<IAssessmentEditContext>();
            context.SelectedAttribute = null;
            ShowViewModel<EditAttributeViewModel>();
        }

        public MvxCommand<AttributeViewModel> AttributeSelectedCommand => new MvxCommand<AttributeViewModel>(DoAttributeSelectedCommand);
        private void DoAttributeSelectedCommand(AttributeViewModel obj)
        {
            var context = Resolve<IAssessmentEditContext>();
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
                var context = Resolve<IAssessmentEditContext>();
                await context.CommitChanges();
				var messenger = Resolve<IMvxMessenger> ();
				messenger.Publish (new EditingDoneEvent (this, EditAction.Edit));
                dialog.ShowSuccess(Messages.General.AssessmentSaved);
                Close();
            }
            catch (Exception ex)
            {
                dialog.HideLoading();
                Mvx.Trace("Unable to save Assessment, exception: {0}", ex);
                await dialog.AlertAsync("Unable to save Assessment. Please try again");
                Close();
            }
            finally
            {
                dialog.HideLoading();
            }
        }

		public MvxCommand CancelCommand => new MvxCommand (Close);

        public ObservableRangeCollection<AttributeViewModel> Attributes { get; } = new ObservableRangeCollection<AttributeViewModel>();
        
		private MvxSubscriptionToken editingDoneToken;
        public Task Initialize()
        {
            var messenger = Resolve<IMvxMessenger>();
            editingDoneToken = messenger.Subscribe<EditingDoneEvent>(OnEditingDone);
            return Refresh();
        }

        private void OnEditingDone(EditingDoneEvent evt)
        {
			if (evt.Sender is EditAttributeViewModel)
            	Refresh();
        }

        protected override void Close()
        {
            editingDoneToken?.Dispose();
            base.Close();
        }

        private async Task Load()
        {
            var dialog = Resolve<IUserDialogs>();
            dialog.ShowLoading(Messages.General.Loading);
            try
            {
                var context = Resolve<IAssessmentEditContext>();
                var assessment = context.Assessment;
                if (assessment != null)
                {
                    Name = assessment.Name;
                    Location = assessment.Location;
                    Attributes.AddRange(assessment.Attributes.Select(a => new AttributeViewModel(a)));
                }
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

        public Task Refresh()
        {
            Attributes.Clear();
            return Load();
        }

        
    }
}
