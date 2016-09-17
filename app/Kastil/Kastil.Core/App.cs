using MvvmCross.Platform.IoC;

namespace Kastil.Core
{
    public class App : MvvmCross.Core.ViewModels.MvxApplication
    {
        public override void Initialize()
        {
            CreatableTypes()      
                .StartingWith("Fake")          
                .EndingWith("Service")
                .AsInterfaces()
                .RegisterAsLazySingleton();

            RegisterAppStart<ViewModels.DisastersViewModel>();
        }
    }
}
