using System;
using System.Threading.Tasks;
using Acr.UserDialogs;
using Kastil.Shared.Models;
using MvvmCross.Core.ViewModels;

namespace Kastil.Core.ViewModels
{
    public class DisasterListItemViewModel : BaseViewModel
    {
        public DisasterListItemViewModel(Disaster value)
        {
            Value = value;
        }

        public Disaster Value { get; }

        public string Text => Value.Name;
        public DateTimeOffset When => Value.When;

        MvxCommand _actionCommand;
        public MvxCommand ActionCommand
        {
            get
            {
                _actionCommand = _actionCommand ?? new MvxCommand(DoActionCommand);
                return _actionCommand;
            }
        }

        private void  DoActionCommand()
        {
            var actionSheetConfig = new ActionSheetConfig();
            actionSheetConfig.Add(Messages.Disaster.Assesment, DoShowAssesment);
            actionSheetConfig.Add(Messages.Disaster.Shelters, DoShowShelters);

            var dialog = Resolve<IUserDialogs>();
            dialog.ActionSheet(actionSheetConfig);
        }

        private void DoShowAssesment()
        {
            ShowViewModel<AssesmentViewModel>(new {disasterId = Value.Id});
        }

        private void DoShowShelters()
        {
            ShowViewModel<SheltersViewModel>(new { disasterId = Value.Id });
        }

    }
}