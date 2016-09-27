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
            set
            {
                _name = value;
                RaisePropertyChanged();
                SaveName(_name);
            }
        }

        private void SaveName(string name)
        {
            if (_editMode)
                return;
            var context = Resolve<IAssessmentEditContext>();
            context.Assessment.Name = name;
        }

        private string _location;
        public string Location
        {
            get { return _location; }
            set
            {
                _location = value;
                RaisePropertyChanged();
                SaveLocation(_location);
            }
        }

        private void SaveLocation(string location)
        {
            if (_editMode)
                return;
            var context = Resolve<IAssessmentEditContext>();
            context.Assessment.Location = location;
        }

        private bool _editMode;
        

        public bool EditMode
        {
            get {return _editMode;}
            private set
            {
                _editMode = value;
                RaisePropertyChanged("EditMode");
                RaisePropertyChanged("AddMode");
            }
        }

        private string _assessmentTitle;
        public string AssessmentTitle
        {
            get { return _assessmentTitle; }
            private set { _assessmentTitle = value; RaisePropertyChanged(); }
        }

        public bool AddMode => !EditMode;

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
				Publish(new EditingDoneEvent (this, EditAction.Edit));
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
        
        public Task Initialize()
        {
            Subscribe<EditingDoneEvent>(OnEditingDone);
            return Refresh();
        }

        private void OnEditingDone(EditingDoneEvent evt)
        {
			if (evt.Sender is EditAttributeViewModel)
            	Refresh();
        }

        private async Task Load()
        {
            var dialog = Resolve<IUserDialogs>();
            dialog.ShowLoading(Messages.General.Loading);
            try
            {
                var context = Resolve<IAssessmentEditContext>();
                EditMode = !context.IsNew;
                var assessment = context.Assessment;
                if (assessment != null)
                {
                    Name = assessment.Name;
                    Location = assessment.Location;
                    Attributes.AddRange(assessment.Attributes.Select(a => new AttributeViewModel(a)));
                }
                AssessmentTitle = EditMode ? Name : "New Assessment";

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
