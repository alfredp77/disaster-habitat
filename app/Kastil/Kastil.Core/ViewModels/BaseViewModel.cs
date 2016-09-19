using System.Threading.Tasks;
using Acr.UserDialogs;
using Kastil.Core.Services;
using MvvmCross.Core.ViewModels;
using MvvmCross.Platform;

namespace Kastil.Core.ViewModels
{
    public abstract class BaseViewModel : MvxViewModel
    {
        protected TService Resolve<TService>() where TService : class
        {
            return Mvx.Resolve<TService>();
        }

        protected void Close()
        {
            Close(this);
        }

        MvxAsyncCommand _logoutCommand;
        public MvxAsyncCommand LogoutCommand
        {
            get
            {
                _logoutCommand = _logoutCommand ?? new MvxAsyncCommand(DoLogoutCommand);
                return _logoutCommand;
            }
        }

        private async Task DoLogoutCommand()
        {
            var userService = Resolve<IUserService>();
            await userService.Logout();
            Close();
        }

		public bool AllowAddCommand { get; protected set;}

        MvxAsyncCommand _addCommand;
        public MvxAsyncCommand AddCommand
        {
            get
            {
                if (_addCommand == null && AllowAddCommand)
                {
                    _addCommand = new MvxAsyncCommand(DoAddCommand);
                }
                return _addCommand;
            }
        }

        protected virtual Task DoAddCommand()
        {
			var dialog = Resolve<IUserDialogs> ();
			return dialog.AlertAsync ("Test");
        }

		public bool AllowSettingCommand { get; protected set; } = false;
        MvxAsyncCommand _settingCommand;
        public MvxAsyncCommand SettingCommand
        {
            get
            {
                if (_settingCommand == null && AllowSettingCommand)
                {
                    _settingCommand = new MvxAsyncCommand(DoSettingCommand);
                }
                return _settingCommand;
            }
        }

        protected virtual async Task DoSettingCommand()
        {
			var dialog = Resolve<IUserDialogs> ();
            if (await dialog.ConfirmAsync(Messages.General.SynchronizeData))
            {
                dialog.ShowLoading(Messages.General.Syncing);
                try
                {
                    var service = Resolve<ISyncService>();
                    await service.Sync();
                }
                finally
                {
                    dialog.HideLoading();
                }
            }
        }


        public string Title { get; protected set; }
    }
}