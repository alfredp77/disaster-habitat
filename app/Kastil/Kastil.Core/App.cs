using System.Reflection;
using Kastil.Common;
using Kastil.Common.Fakes;
using Kastil.Common.Models;
using Kastil.Common.Services;
using Kastil.Common.Utils;
using Kastil.Core.Fakes;
using Kastil.Core.Services;
using MvvmCross.Platform;
using MvvmCross.Platform.IoC;

namespace Kastil.Core
{
    public class App : MvvmCross.Core.ViewModels.MvxApplication
    {
        public override void Initialize()
        {

#if FAKES
            RegisterFakeServices();
#else
            RegisterRealServices();
#endif

            RegisterNonPlatformSpecificStuff();
            RegisterAppStart<ViewModels.DisastersViewModel>();
        }

        private void RegisterFakeServices()
        {
            CreatableTypes(typeof(ITap2HelpService).GetTypeInfo().Assembly)
                    .InNamespace(typeof(FakeTap2HelpService).Namespace)
                    .EndingWith("Service")
                    .Except(typeof(PushService), typeof(SaveOrUpdatePushService), typeof(RemovalPushService))
                    .AsInterfaces()
                    .RegisterAsLazySingleton();

            CreatableTypes(typeof(ISyncService).GetTypeInfo().Assembly)
                    .InNamespace(typeof(FakeSyncService).Namespace)
                    .EndingWith("Service")
                    .AsInterfaces()
                    .RegisterAsLazySingleton();
        }

        private void RegisterRealServices()
        {
            CreatableTypes(typeof(ITap2HelpService).GetTypeInfo().Assembly)
                .InNamespace(typeof(ITap2HelpService).Namespace)
                .EndingWith("Service")
                .Except(typeof(PushService), typeof(SaveOrUpdatePushService), typeof(RemovalPushService))
                .AsInterfaces()
                .RegisterAsLazySingleton();

            Mvx.LazyConstructAndRegisterSingleton<ISyncService>(() =>
            {
                var persistenceContextFactory = Mvx.Resolve<IPersistenceContextFactory>();
                var pullService = Mvx.Resolve<IPullService>();
                var pushService = Mvx.Resolve<IPushService>();
                return new SyncService(persistenceContextFactory, 
                    new ISyncService[]
                    {
                        new DisasterSyncService(pullService, persistenceContextFactory),
                        new AttributeSyncService(pullService), 
                        new AttributedItemSyncService<Assessment, AssessmentAttribute>(pushService, pullService, Mvx.Resolve<IBackendlessQueryProvider>(), persistenceContextFactory),
                        new AttributedItemSyncService<Shelter, ShelterAttribute>(pushService, pullService, Mvx.Resolve<IBackendlessQueryProvider>(), persistenceContextFactory),
                    }
                );
            });

            Mvx.LazyConstructAndRegisterSingleton<IBackendlessResponseParser>(() => new BackendlessResponseParser(Mvx.Resolve<IJsonSerializer>()));
            Mvx.LazyConstructAndRegisterSingleton<ILoginService>(() => new LoginService(Mvx.Resolve<Connection>(), Mvx.Resolve<IRestServiceCaller>(), Mvx.Resolve<IJsonSerializer>()));
            Mvx.LazyConstructAndRegisterSingleton<IBackendlessQueryProvider>(() => new BackendlessQueryProvider());
            Mvx.RegisterSingleton(() => new SaveOrUpdatePushService());
            Mvx.RegisterSingleton(() => new RemovalPushService());
            Mvx.LazyConstructAndRegisterSingleton<IPushService>(() => new PushService(Mvx.Resolve<SaveOrUpdatePushService>(), Mvx.Resolve<RemovalPushService>()));
        }

        private static void RegisterNonPlatformSpecificStuff()
        {
            Mvx.RegisterSingleton(() => new Connection());
			Mvx.RegisterSingleton<IJsonSerializer>(() => new JsonSerializer());
            Mvx.RegisterSingleton<IPersistenceContextFactory>(() => new FileBasedPersistenceContextFactory());
            Mvx.RegisterSingleton(() => new AttributedEditContext());
            Mvx.RegisterSingleton(() => new AttributedListContext());
			Mvx.RegisterSingleton(() => new AssessmentListHandler());
			Mvx.RegisterSingleton(() => new ShelterListHandler());
        }
    }
}
