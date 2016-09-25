using Android.App;
using Android.OS;
using Kastil.Core.ViewModels;

namespace Kastil.Droid.Views
{
	[Activity(Label="AssessmentList")]
	public class AssessmentListView : BaseView
	{
		protected override int LayoutResource => Resource.Layout.AssessmentListView;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            var vm = (AssessmentListViewModel)ViewModel;
            vm.Initialize();
        }

    }
}

