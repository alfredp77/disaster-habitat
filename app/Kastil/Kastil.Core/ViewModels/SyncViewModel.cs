using System;
using System.Threading.Tasks;
using Acr.UserDialogs;
using Kastil.Common.Events;
using Kastil.Common.Services;
using Kastil.Common.ViewModels;
using Kastil.Core.Services;
using MvvmCross.Core.ViewModels;
using MvvmCross.Platform;
using MvvmCross.Plugins.Messenger;

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

        private string _email;
        public string Email
        {
            get { return _email; }
            set { _email = value; RaisePropertyChanged(); }
        }

        private string _password;
        public string Password
        {
            get { return _password; }
            set { _password = value; RaisePropertyChanged(); }
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
            if (string.IsNullOrEmpty(_email))
            {
                await dialog.AlertAsync(Messages.Login.EmailIsRequired);
                return;
            }
            if (string.IsNullOrEmpty(_password))
            {
                await dialog.AlertAsync(Messages.Login.PasswordIsRequired);
                return;
            }
            dialog.ShowLoading(Messages.General.Syncing);
			var messenger = Resolve<IMvxMessenger>();
            try
            {
                var loginService = Resolve<ILoginService>();
                var loggedInUser = await loginService.Login(_email, _password);
                if (string.IsNullOrEmpty(loggedInUser?.Token))
                {
                    throw new Exception("Unable to login!");
                }

                var service = Resolve<ISyncService>();
                await service.Sync(loggedInUser);


				messenger.Publish(new SyncCompletedEvent(this, true));
                Close();
            }
            catch (Exception ex)
            {
                dialog.HideLoading();
                Mvx.Trace("Syncing failed for {0}. Exception: {1}", _email, ex);
                await dialog.AlertAsync(Messages.General.SomethingWentWrongPleaseTryAgain);
				messenger.Publish(new SyncCompletedEvent(this, false));
            }
            finally
            {
                dialog.HideLoading();
            }
        }
    }
}