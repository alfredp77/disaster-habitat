using System.Reflection;
using Kastil.Common;
using Kastil.Common.Fakes;
using Kastil.Common.Services;
using Kastil.Common.Utils;
using MvvmCross.Platform;
using MvvmCross.Platform.IoC;
using Tap2Give.Core.Services;

namespace Tap2Give.Core
{
    public class App : MvvmCross.Core.ViewModels.MvxApplication
    {
        public override void Initialize()
        {
            CreatableTypes(typeof(ITap2HelpService).GetTypeInfo().Assembly)
				//.InNamespace(typeof(FakeTap2HelpService).Namespace) // uncomment this line to use the fakes instead                
				.InNamespace(typeof(ITap2HelpService).Namespace)      // comment this line when using fakes
				.EndingWith("Service")
				.AsInterfaces()
				.RegisterAsLazySingleton();

			CreatableTypes()
				.InNamespace(typeof(ITap2GiveService).Namespace)
				.EndingWith("Service")
				.AsInterfaces()
				.RegisterAsLazySingleton();

            Mvx.RegisterSingleton(() => new Connection());              
            Mvx.RegisterSingleton<IDisasterContext>(() => new DisasterContext());
			Mvx.RegisterSingleton<IPersistenceContextFactory>(() => new FileBasedPersistenceContextFactory());
			Mvx.RegisterType<IJsonSerializer>(() => new JsonSerializer());

            RegisterAppStart<ViewModels.DisastersViewModel>();
        }
    }
}
