using Android.App;
using Android.OS;
using Kastil.Core.ViewModels;

namespace Kastil.Droid.Views
{
    [Activity(Label = "Assesment")]
    public class AssesmentView : BaseView
    {
        protected override int LayoutResource => Resource.Layout.AssesmentView;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            var vm = (AssesmentViewModel)ViewModel;
            vm.Initialize();
        }

        protected override void OnResume()
        {
            base.OnResume();
            var vm = (AssesmentViewModel)ViewModel;
            vm.Refresh();
        }

    }
}