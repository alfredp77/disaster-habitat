using System;
using System.Linq;
using System.Threading.Tasks;
using Acr.UserDialogs;
using Kastil.Core.Services;
using Kastil.Core.Utils;
using Kastil.Shared.Models;
using MvvmCross.Platform;

namespace Kastil.Core.ViewModels
{
    public class AssesmentViewModel : BaseViewModel
    {
        public AssesmentViewModel()
        {
            Attributes = new ObservableRangeCollection<AttributeViewModel>();
        }

        private Assesment _assesment;
        private Assesment Assesment
        {
            get { return _assesment; }
            set
            {
                _assesment = value;
                RaisePropertyChanged("AssesmentName");
                RaisePropertyChanged("AssestmentLocation");
            }
        }

        public string AssestmentName => Assesment.Name;
        public string AssestmentLocation => Assesment.Location.Name;
        public ObservableRangeCollection<AttributeViewModel> Attributes { get; }
        public string AssesmentId { get; private set; }
        public string DisasterId { get; private set; }

        public void Init(string disasterId, string assesmentId)
        {
            DisasterId = disasterId;
            AssesmentId = assesmentId;
        }

        public Task Initialize()
        {
            Attributes.Clear();
            return Load();
        }

        private async Task Load()
        {
            var dialog = Resolve<IUserDialogs>();
            dialog.ShowLoading(Messages.General.Loading);
            try
            {
                InitValue(DisasterId, AssesmentId);
            }
            catch (Exception ex)
            {
                dialog.HideLoading();
                Mvx.Trace("Unable to load Assessment, exception: {0}", ex);
                await dialog.AlertAsync("Unable to load Assessment. Please try again");
            }
            finally
            {
                dialog.HideLoading();
            }
        }

        private async void InitValue(string disasterId, string assesmentId)
        {
            if (disasterId == null || assesmentId == null)
            {
                Assesment = new Assesment();
                return;
            }
            var disasterService = Resolve<ITap2HelpService>();
            Assesment =  await disasterService.GetAssesment(disasterId, assesmentId);
            if (Assesment != null)
            {
                Attributes.AddRange(Assesment.Attributes.Select(a => new AttributeViewModel(a)));
            }
        }

        
       
    }
}
