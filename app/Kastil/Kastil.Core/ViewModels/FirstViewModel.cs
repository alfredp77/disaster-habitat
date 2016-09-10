namespace Kastil.Core.ViewModels
{
    public class FirstViewModel
        : BaseViewModel
    {
        private string _hello = "Hello MvvmCross";
        public string Hello
        {
            get { return _hello; }
            set { SetProperty(ref _hello, value); }
        }
    }
}
