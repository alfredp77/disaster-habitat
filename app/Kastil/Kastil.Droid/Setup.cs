using Acr.UserDialogs;
using Android.Content;
using Android.Views;
using Kastil.Common.Services;
using Kastil.Core;
using MvvmCross.Droid.Platform;
using MvvmCross.Core.ViewModels;
using MvvmCross.Platform;
using MvvmCross.Platform.Droid.Platform;
using MvvmCross.Platform.Platform;
using Kastil.Core.Services;
using Kastil.Core.ViewModels;
using Kastil.Droid.Views;
using Kastil.PlatformSpecific.Shared;
using MvvmCross.Binding.Bindings.Target.Construction;
using MvvmCross.Binding.Droid.Binders.ViewTypeResolvers;
using MvvmCross.Core.Views;
using MvvmCross.Droid.Support.V7.AppCompat;
using MvvmCross.Droid.Views;
using MvvmCross.Platform.IoC;

namespace Kastil.Droid
{
    public class Setup : MvxAndroidSetup
    {
        public Setup(Context applicationContext) : base(applicationContext)
        {
        }

        protected override IMvxApplication CreateApp()
        {
            return new Core.App();
        }

        protected override IMvxTrace CreateDebugTrace()
        {
            return new DebugTrace();
        }

        protected override void InitializeFirstChance()
        {
            Mvx.RegisterType<IUserDialogs>(() => new UserDialogsImpl(() => Mvx.Resolve<IMvxAndroidCurrentTopActivity>().Activity));
			Mvx.RegisterSingleton<IRestServiceCaller> (() => new RestServiceCaller());			
            Mvx.RegisterSingleton(FolderProviderFactory.Create);
            base.InitializeFirstChance();
        }

        protected override void FillTargetFactories(IMvxTargetBindingFactoryRegistry registry)
        {
            MvxAppCompatSetupHelper.FillTargetFactories(registry);
            base.FillTargetFactories(registry);
        }        
    }
}
