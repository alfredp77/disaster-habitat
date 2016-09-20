using System;
using System.Threading.Tasks;
using Acr.UserDialogs;
using Kastil.Core.Services;
using MvvmCross.Core.ViewModels;
using MvvmCross.Platform;

namespace Kastil.Core.ViewModels
{
    public class SyncViewModel : BaseViewModel
    {
        public SyncViewModel()
        {
            Title = "Synchronize Data";
            AllowAddCommand = false;
            AllowSettingCommand = false;
        }

        private string _staffCode;
        public string StaffCode
        {
            get { return _staffCode; }
            set { _staffCode = value; RaisePropertyChanged(); }
        }


        MvxAsyncCommand _syncCommand;
        public MvxAsyncCommand SyncCommand
        {
            get
            {
                _syncCommand = _syncCommand ?? new MvxAsyncCommand(DoSyncCommand);
                return _syncCommand;
            }
        }

        private async Task DoSyncCommand()
        {
            var dialog = Resolve<IUserDialogs>();
            if (string.IsNullOrEmpty(_staffCode))
            {
                await dialog.AlertAsync(Messages.Login.PleaseKeyInYourStaffCode);
                return;
            }

            dialog.ShowLoading(Messages.General.Syncing);

            try
            {
                var service = Resolve<ISyncService>();
                await service.Sync(_staffCode);
                Close();
            }
            catch (Exception ex)
            {
                dialog.HideLoading();
                Mvx.Trace("Syncing failed for {0}. Exception: {1}", _staffCode, ex);
                await dialog.AlertAsync(Messages.General.SomethingWentWrongPleaseTryAgain);
            }
            finally
            {
                dialog.HideLoading();
            }
        }
    }
}