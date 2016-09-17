using Android.App;
using Android.OS;
using Kastil.Core.ViewModels;

namespace Kastil.Droid.Views
{
	[Activity(Label="AssesmentList")]
	public class AssesmentListView : BaseView
	{
		protected override int LayoutResource => Resource.Layout.AssesmentListView;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            var vm = (AssesmentListViewModel)ViewModel;
            vm.Initialize();
        }

        protected override void SetupToolbar()
        {
            // do nothing
        }
    }
}

