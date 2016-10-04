using Acr.UserDialogs;
using Kastil.Core.Services;
using Kastil.Core.Utils;
using MvvmCross.Core.ViewModels;
using MvvmCross.Platform;
using System;
using System.Linq;
using System.Threading.Tasks;
using MvvmCross.Plugins.Messenger;
using Kastil.Core.Events;
using Kastil.Shared.Models;

namespace Kastil.Core.ViewModels
{
    public class AssessmentListViewModel : BaseViewModel
    {
        public ObservableRangeCollection<AssessmentListItemViewModel> Items { get; } = new ObservableRangeCollection<AssessmentListItemViewModel> ();

        public string DisasterId { get; set; }

        public AssessmentListViewModel()
        {
            Title = "Assessments";
            AllowAddCommand = true; 
        }

		public void Init (string disasterId)
		{
			DisasterId = disasterId;
		}

        public override Task Initialize()
        {            
			Subscribe<EditingDoneEvent> (async e => await OnEditingDone(e));
            return Load();
        }

		private async Task OnEditingDone (EditingDoneEvent evt)
		{
			if (evt.Sender is AssessmentViewModel)
				await DoRefreshCommand();
		}

        private async Task Load()
        {
            var dialog = Resolve<IUserDialogs>();
            dialog.ShowLoading(Messages.General.Loading);
            
            var disasterService = Resolve<ITap2HelpService>();
            try
            {
                var assessments = await disasterService.GetAssessments(DisasterId);
                if (assessments != null)
                {
                    Items.AddRange(assessments.Select(d => new AssessmentListItemViewModel(d)));
                }
            }
            catch (Exception ex)
            {
                dialog.HideLoading();
                Mvx.Trace("Unable to load Assessment list, exception: {0}", ex);
                await dialog.AlertAsync("Unable to load Assessment list. Please try again");
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

        MvxCommand<AssessmentListItemViewModel> _assessmentSelectedCommand;
        public MvxCommand<AssessmentListItemViewModel> AssessmentSelectedCommand
        {
            get
            {
                _assessmentSelectedCommand = _assessmentSelectedCommand ?? new MvxCommand<AssessmentListItemViewModel>(DoAssessmentSelectedCommand);
                return _assessmentSelectedCommand;
            }
        }

        private void DoAssessmentSelectedCommand(AssessmentListItemViewModel itemVm)
        {
            var context = Resolve<IAssessmentEditContext>();
            context.Initialize(itemVm.Value);
            ShowViewModel<AssessmentViewModel>();
        }

        protected override Task DoAddCommand()
        {
            return Task.Run(() =>
            {
                var context = Resolve<IAssessmentEditContext>();
                context.Initialize(disasterId:DisasterId);
                ShowViewModel<AssessmentViewModel>();
            });
        }
    }
}
