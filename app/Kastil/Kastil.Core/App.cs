using System.Reflection;
using Kastil.Common;
using Kastil.Common.Fakes;
using Kastil.Common.Services;
using Kastil.Common.Utils;
using Kastil.Core.Services;
using Kastil.Common.Models;
using MvvmCross.Platform;
using MvvmCross.Platform.IoC;

namespace Kastil.Core
{
    public class App : MvvmCross.Core.ViewModels.MvxApplication
    {
        public override void Initialize()
        {
            CreatableTypes(typeof(ITap2HelpService).GetTypeInfo().Assembly)
                .InNamespace(typeof(FakeTap2HelpService).Namespace) // uncomment this line to use the fakes instead                
                //.InNamespace(typeof(ITap2HelpService).Namespace)      // comment this line when using fakes
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
            Mvx.RegisterSingleton(() => new AssessmentEditContext());
            Mvx.RegisterSingleton(() => new ShelterEditContext());
        }
    }
}
