using System.Threading.Tasks;
using Kastil.Common.Utils;
using Kastil.Common.ViewModels;
using Kastil.Core.Events;
using MvvmCross.Core.ViewModels;

namespace Kastil.Core.ViewModels
{
    public class ItemListViewModel<T> : BaseViewModel 
    {
        public ObservableRangeCollection<T> Items { get; set; }
        public string DisasterId { get; set; }

        public void Init(string disasterId)
        {
            DisasterId = disasterId;
        }

        public override Task Initialize()
        {
            Subscribe<EditingDoneEvent>(async e => await OnEditingDone(e));
            return Load();
        }

        protected virtual async Task Load()
        {
            
        }

        private async Task OnEditingDone(EditingDoneEvent evt)
        {
            if (evt.Sender is ShelterViewModel || evt.Sender is AssessmentViewModel)
                await DoRefreshCommand();
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

        protected async Task DoRefreshCommand()
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
    }
}
