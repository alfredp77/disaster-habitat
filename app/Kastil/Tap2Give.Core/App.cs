using Kastil.Common;
using MvvmCross.Platform;
using MvvmCross.Platform.IoC;

namespace Tap2Give.Core
{
    public class App : MvvmCross.Core.ViewModels.MvxApplication
    {
        public override void Initialize()
        {
            CreatableTypes()
                .EndingWith("Service")
                .AsInterfaces()
                .RegisterAsLazySingleton();

            Mvx.RegisterSingleton(() => new Connection());
            RegisterAppStart<ViewModels.DisasterIncidentsViewModel>();
        }
    }
}
