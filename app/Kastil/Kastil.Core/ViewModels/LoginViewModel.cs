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
        public System.Windows.Input.ICommand LoginCommand
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
                await dialog.PromptAsync("Please key in your staff code");
                return;
            }

            dialog.ShowLoading("Logging you in");

            try
            {
                var service = Resolve<IUserService>();
                var user = await service.Login(_staffCode);
                if (user != null)
                    ShowViewModel<HomeViewModel>();
                else
                    await dialog.PromptAsync("Something went wrong. Please try again.");
            }
            catch (Exception ex)
            {
                Mvx.Trace("Fail logging in user {0}. Exception: {1}", _staffCode, ex);
            }
            finally
            {
                dialog.HideLoading();
            }
        }
    }
}