using MvvmCross.Platform;

namespace Kastil.Common.Services
{
    public abstract class BaseService
    {
        protected TService Resolve<TService>() where TService : class
        {
            return Mvx.Resolve<TService>();
        }
    }
}