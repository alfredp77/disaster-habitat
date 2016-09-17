using Android.App;
using Android.OS;
using Kastil.Core.ViewModels;

namespace Kastil.Droid.Views
{
	[Activity(Label="Assesment")]
	public class AssesmentsView : BaseView
	{
		protected override int LayoutResource => Resource.Layout.AssesmentsView;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            var vm = (AssesmentsViewModel)ViewModel;
            vm.Initialize();
        }

        protected override void SetupToolbar()
        {
            // do nothing
        }
    }
}

