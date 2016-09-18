using Kastil.Core.Services;
using Kastil.Core.Utils;
using MvvmCross.Platform;
using MvvmCross.Platform.IoC;

namespace Kastil.Core
{
    public class App : MvvmCross.Core.ViewModels.MvxApplication
    {
        public override void Initialize()
        {
            CreatableTypes()
                //.InNamespace(typeof(FakeTap2HelpService).Namespace) // uncomment this line to use the fakes instead                
                .InNamespace(typeof(ITap2HelpService).Namespace)
                .EndingWith("Service")
                .AsInterfaces()
                .RegisterAsLazySingleton();

            RegisterNonPlatformSpecificStuff();
            RegisterAppStart<ViewModels.DisastersViewModel>();
        }

        private static void RegisterNonPlatformSpecificStuff()
        {
            Mvx.RegisterSingleton(() => new Connection());
            Mvx.RegisterSingleton<IPersistenceContextFactory>(() => new FileBasedPersistenceContextFactory());
            Mvx.RegisterType<IJsonSerializer>(() => new JsonSerializer());
        }
    }
}
