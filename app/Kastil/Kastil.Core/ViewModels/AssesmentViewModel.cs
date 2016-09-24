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
    public class AssesmentViewModel : BaseViewModel
    {
        private Assesment _assesment;

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

        public ICommand AddAttributeCommand => new MvxCommand<AssesmentViewModel>(DoAddAttrCommand);

        private void DoAddAttrCommand(AssesmentViewModel obj)
        {
           ShowViewModel<EditAttributeViewModel>(new { disasterId = _disasterId, assesmentId = _assesmentId});
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
            ShowViewModel<EditAttributeViewModel>(new { disasterId= _disasterId, assesmentId= _assesmentId, attributeName = obj.AttributeName, attributeValue = obj.AttributeValue});
        }

        public ObservableRangeCollection<AttributeViewModel> Attributes { get; } = new ObservableRangeCollection<AttributeViewModel>();

        private string _assesmentId;
        private string _disasterId;
        public void Init(string disasterId, string assesmentId)
        {
            _disasterId = disasterId;
            _assesmentId = assesmentId;
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
                var assesment = await service.GetAssesment(_disasterId, _assesmentId);
                if (assesment != null)
                {
                    _assesment = assesment;
                    Name = assesment.Name;
                    Location = assesment.Location;
                    Attributes.AddRange(assesment.Attributes.Select(a => new AttributeViewModel(a)));
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
