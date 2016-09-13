using Android.App;
using Android.OS;
using Kastil.Core.ViewModels;

namespace Kastil.Droid.Views
{
    [Activity(Label = "Home")]
    public class DisastersView : BaseView
    {
        protected override int LayoutResource => Resource.Layout.DisastersView;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            var vm = (DisastersViewModel) ViewModel;
            vm.Initialize();
        }

        protected override void SetupToolbar()
        {
            // do nothing
        }
    }
}