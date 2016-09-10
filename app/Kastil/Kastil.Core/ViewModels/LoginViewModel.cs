using System.Threading.Tasks;
using MvvmCross.Core.ViewModels;

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

        private Task DoMyCommand()
        {

        }
    }
}