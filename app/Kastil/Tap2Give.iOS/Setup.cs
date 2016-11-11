using Acr.UserDialogs;
using Kastil.Common.Services;
using Kastil.PlatformSpecific.Shared;
using MvvmCross.Core.ViewModels;
using MvvmCross.iOS.Platform;
using MvvmCross.iOS.Views.Presenters;
using MvvmCross.Platform;
using MvvmCross.Platform.Platform;
using UIKit;

namespace Tap2Give.iOS
{
    public class Setup : MvxIosSetup
    {
        public Setup(MvxApplicationDelegate applicationDelegate, UIWindow window)
            : base(applicationDelegate, window)
        {
        }

        public Setup(MvxApplicationDelegate applicationDelegate, IMvxIosViewPresenter presenter)
            : base(applicationDelegate, presenter)
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
            Mvx.RegisterType<IUserDialogs>(() => new UserDialogsImpl());
            Mvx.RegisterSingleton<IRestServiceCaller>(() => new RestServiceCaller());
            Mvx.RegisterSingleton(FolderProviderFactory.Create());
            base.InitializeFirstChance();
        }
    }
}
