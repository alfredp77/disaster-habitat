using System.Reflection;
using Kastil.Common;
using Kastil.Common.Services;
using Kastil.Common.Utils;
using Kastil.Core.Services;
using MvvmCross.Platform;
using MvvmCross.Platform.IoC;
using Newtonsoft.Json;
using JsonSerializer = Kastil.Common.Utils.JsonSerializer;

namespace Kastil.Core
{
    public class App : MvvmCross.Core.ViewModels.MvxApplication
    {
        public override void Initialize()
        {            
            CreatableTypes(typeof(ITap2HelpService).GetTypeInfo().Assembly)
//                .InNamespace(typeof(FakeTap2HelpService).Namespace) // uncomment this line to use the fakes instead                
                .InNamespace(typeof(ITap2HelpService).Namespace)      // comment this line when using fakes
                .EndingWith("Service")
                .AsInterfaces()
                .RegisterAsLazySingleton();

            CreatableTypes(typeof(ISyncService).GetTypeInfo().Assembly)
                //                .InNamespace(typeof(FakeSyncService).Namespace) // uncomment this line to use the fakes instead                
                .InNamespace(typeof(ISyncService).Namespace)      // comment this line when using fakes
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
            Mvx.RegisterSingleton(() => new AttributedEditContext());
            Mvx.RegisterSingleton(() => new AttributedListContext());
			Mvx.RegisterSingleton(() => new AssessmentListHandler());
			Mvx.RegisterSingleton(() => new ShelterListHandler());
            Mvx.LazyConstructAndRegisterSingleton(() => new AssessmentPushService());
            Mvx.LazyConstructAndRegisterSingleton(() => new ShelterPushService());
        }
    }
}
