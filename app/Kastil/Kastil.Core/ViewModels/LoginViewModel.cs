using System;
using System.Threading.Tasks;
using Acr.UserDialogs;
using Kastil.Core.Services;
using MvvmCross.Core.ViewModels;
using MvvmCross.Platform;

namespace Kastil.Core.ViewModels
{
    public class LoginViewModel : BaseViewModel
    {
        private string _staffCode;
        public string StaffCode
        {
            get { return _staffCode; }
            set { SetProperty(ref _staffCode, value); }
        }

        MvxAsyncCommand _loginCommand;
        public MvxAsyncCommand LoginCommand
        {
            get
            {
                _loginCommand = _loginCommand ?? new MvxAsyncCommand(DoMyCommand);
                return _loginCommand;
            }
        }

        private async Task DoMyCommand()
        {
            var dialog = Resolve<IUserDialogs>();
            if (string.IsNullOrEmpty(_staffCode))
            {
                await dialog.AlertAsync(Messages.Login.PleaseKeyInYourStaffCode);
                return;
            }

            dialog.ShowLoading(Messages.Login.LoggingYouIn);

            try
            {
                var service = Resolve<IUserService>();
                var user = await service.Login(_staffCode);
                if (user != null)
                    ShowViewModel<HomeViewModel>();
                else
                    await dialog.AlertAsync(Messages.General.SomethingWentWrongPleaseTryAgain);
            }
            catch (Exception ex)
            {
                Mvx.Trace("Fail logging in user {0}. Exception: {1}", _staffCode, ex);
                await dialog.AlertAsync(Messages.General.SomethingWentWrongPleaseTryAgain);
            }
            finally
            {
                dialog.HideLoading();
            }
        }
    }
}