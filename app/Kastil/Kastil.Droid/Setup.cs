using Acr.UserDialogs;
using Android.Content;
using MvvmCross.Droid.Platform;
using MvvmCross.Core.ViewModels;
using MvvmCross.Platform;
using MvvmCross.Platform.Droid.Platform;
using MvvmCross.Platform.Platform;
using Kastil.Core.Services;
using Kastil.PlatformSpecific.Shared;
using Kastil.Core.Utils;

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
			Mvx.RegisterType<IRestServiceCaller> (() => new RestServiceCaller());
			Mvx.RegisterType<IJsonSerializer> (() => new JsonSerializer());
            base.InitializeFirstChance();
        }
    }
}
