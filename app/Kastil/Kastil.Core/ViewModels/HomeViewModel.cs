using System;
using System.Threading.Tasks;
using Acr.UserDialogs;
using Kastil.Core.Services;
using MvvmCross.Core.ViewModels;
using MvvmCross.Platform;

namespace Kastil.Core.ViewModels
{
    public class HomeViewModel : BaseViewModel
    {
        MvxAsyncCommand _logoutCommand;
        public System.Windows.Input.ICommand LogoutCommand
        {
            get
            {
                _logoutCommand = _logoutCommand ?? new MvxAsyncCommand(DoLogoutCommand);
                return _logoutCommand;
            }
        }

        private async Task DoLogoutCommand()
        {
            var dialogs = Resolve<IUserDialogs>();
            dialogs.ShowLoading(Messages.Login.LoggingYouOut);

            try
            {
                var service = Resolve<IUserService>();
                await service.Logout();
                Close();
            }
            catch (Exception ex)
            {
                Mvx.Trace("Unable to log user out: {0}", ex);
                await dialogs.PromptAsync(Messages.General.SomethingWentWrongPleaseTryAgain);
            }
            finally
            {
                dialogs.HideLoading();
            }
        }
    }
}