using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Acr.UserDialogs;
using Kastil.Core.Services;
using Kastil.Core.Utils;
using Kastil.Shared.Models;
using MvvmCross.Core.ViewModels;
using MvvmCross.Platform;

namespace Kastil.Core.ViewModels
{
    public class AssessmentViewModel : BaseViewModel
    {
        private Assessment _assessment;

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

        public ICommand AddAttributeCommand => new MvxCommand<AssessmentViewModel>(DoAddAttrCommand);

        private void DoAddAttrCommand(AssessmentViewModel obj)
        {
           ShowViewModel<EditAttributeViewModel>(new { disasterId = _disasterId, assessmentId = _assessmentId});
        }

        MvxCommand<AttributeViewModel> _attributeSelectedCommand;
        public MvxCommand<AttributeViewModel> AttributeSelectedCommand
        {
            get
            {
                _attributeSelectedCommand = _attributeSelectedCommand ?? new MvxCommand<AttributeViewModel>(DoAttributeSelectedCommand);
                return _attributeSelectedCommand;
            }
        }

        private void DoAttributeSelectedCommand(AttributeViewModel obj)
        {
            ShowViewModel<EditAttributeViewModel>(new { disasterId= _disasterId, assessmentId= _assessmentId, attributeName = obj.AttributeName, attributeValue = obj.AttributeValue});
        }

        public ObservableRangeCollection<AttributeViewModel> Attributes { get; } = new ObservableRangeCollection<AttributeViewModel>();

        private string _assessmentId;
        private string _disasterId;
        public void Init(string disasterId, string assessmentId)
        {
            _disasterId = disasterId;
            _assessmentId = assessmentId;
        }

        public Task Initialize()
        {
            return Refresh();
        }

        private async Task Load()
        {
            var dialog = Resolve<IUserDialogs>();
            dialog.ShowLoading(Messages.General.Loading);
            try
            {
                var service = Resolve<ITap2HelpService>();
                var assessment = await service.GetAssessment(_disasterId, _assessmentId);
                if (assessment != null)
                {
                    _assessment = assessment;
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
