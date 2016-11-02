using Acr.UserDialogs;
using Kastil.Core.Services;
using MvvmCross.Core.ViewModels;
using MvvmCross.Platform;
using System;
using System.Linq;
using System.Threading.Tasks;
using Kastil.Common.Services;
using Kastil.Common.Utils;

namespace Kastil.Core.ViewModels
{
    public class AssessmentListViewModel : ItemListViewModel<AssessmentListItemViewModel>
    {
        public AssessmentListViewModel()
        {
            Title = "Assessments";
            AllowAddCommand = true;
            Items = new ObservableRangeCollection<AssessmentListItemViewModel>();
        }
        
        protected override async Task Load()
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
