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
    }
}