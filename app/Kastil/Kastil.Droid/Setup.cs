using Acr.UserDialogs;
using Android.Content;
using Kastil.Common.Services;
using Kastil.Core;
using MvvmCross.Droid.Platform;
using MvvmCross.Core.ViewModels;
using MvvmCross.Platform;
using MvvmCross.Platform.Droid.Platform;
using MvvmCross.Platform.Platform;
using Kastil.Core.Services;
using Kastil.PlatformSpecific.Shared;
using MvvmCross.Binding.Bindings.Target.Construction;
using MvvmCross.Droid.Support.V7.AppCompat;

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
