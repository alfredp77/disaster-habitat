using Acr.UserDialogs;
using Kastil.Common.Services;
using Kastil.Core.Services;
using Kastil.Core.ViewModels;
using Kastil.PlatformSpecific.Shared;
using MvvmCross.Core.ViewModels;
using MvvmCross.Core.Views;
using MvvmCross.iOS.Platform;
using MvvmCross.iOS.Views.Presenters;
using MvvmCross.Platform;
using MvvmCross.Platform.Platform;
using UIKit;

namespace Kastil.iOS
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

		protected override void InitializeViewLookup()
		{
			base.InitializeViewLookup();
			var viewsContainer = Mvx.Resolve<IMvxViewsContainer>();
			viewsContainer.Add(typeof(AssessmentViewModel), typeof(AssessmentView));
			viewsContainer.Add(typeof(ShelterViewModel), typeof(AssessmentView));
			viewsContainer.Add(typeof(AssessmentListViewModel), typeof(AssessmentListView));
			viewsContainer.Add(typeof(ShelterListViewModel), typeof(AssessmentListView));
			viewsContainer.Add(typeof(EditAssessmentAttributeViewModel), typeof(EditAttributeView));
			viewsContainer.Add(typeof(EditShelterAttributeViewModel), typeof(EditAttributeView));
		}
    }
}
