using MvvmCross.Platform;

namespace Kastil.Core.Services
{
    public abstract class BaseService
    {
        protected TService Resolve<TService>() where TService : class
        {
            return Mvx.Resolve<TService>();
        }
    }
}