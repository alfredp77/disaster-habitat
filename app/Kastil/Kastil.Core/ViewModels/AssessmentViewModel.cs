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
    public class AssessmentViewModel : BaseViewModel
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
			get { return Messages.Placeholders.AssessmentName;}
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
					return Messages.Placeholders.AssessmentLocation;
				return Messages.General.Unknown;
			}
		}

        public bool AddMode => Context.IsNew;
        
        public ICommand AddAttributeCommand => new MvxCommand(DoAddAttrCommand);
        private void DoAddAttrCommand()
        {
            Context.SelectedAttribute = null;
            ShowViewModel<EditAssessmentAttributeViewModel>();
        }

        public MvxCommand<AttributeViewModel> AttributeSelectedCommand => new MvxCommand<AttributeViewModel>(DoAttributeSelectedCommand);
        private void DoAttributeSelectedCommand(AttributeViewModel obj)
        {
            Context.SelectedAttribute = obj.Attribute;
            ShowViewModel<EditAssessmentAttributeViewModel>();
        }

        public MvxAsyncCommand SaveCommand => new MvxAsyncCommand(DoSaveCommand);
        private async Task DoSaveCommand()
        {
            var dialog = Resolve<IUserDialogs>();
            dialog.ShowLoading(Messages.General.Saving);

            try
            {
                SetAssessmentProperties();
                await Context.CommitChanges();
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

        private IAssessmentEditContext Context => Resolve<IAssessmentEditContext>();

        private void SetAssessmentProperties()
        {
            var assessment = Context.Item;
            assessment.Name = Name;
            assessment.LocationName = Location;
        }

        public MvxCommand CancelCommand => new MvxCommand (Close);

        public ObservableRangeCollection<AttributeViewModel> Attributes { get; } = new ObservableRangeCollection<AttributeViewModel>();
        
        public override Task Initialize()
        {
            Subscribe<EditingDoneEvent>(async e => await OnEditingDone(e));
            return Load();
        }

        private async Task OnEditingDone(EditingDoneEvent evt)
        {
			if (evt.Sender is EditAssessmentAttributeViewModel)
            	await Load();
        }

        private async Task Load()
        {
            var dialog = Resolve<IUserDialogs>();
            dialog.ShowLoading(Messages.General.Loading);
            try
            {
                var assessment = Context.Item;
                if (assessment != null)
                {
                    SetTitle();
                    Attributes.Clear();
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

        private void SetTitle()
        {
            Title = string.IsNullOrEmpty(Name) ? Messages.General.DefaultNewAssessmentName : Name;
        }
    }
}
