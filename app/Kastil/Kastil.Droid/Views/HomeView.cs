using Android.App;

namespace Kastil.Droid.Views
{
    [Activity(Label = "Home")]
    public class HomeView : BaseView
    {
        protected override int LayoutResource => Resource.Layout.HomeView;

        protected override void SetupToolbar()
        {
            // do nothing
        }
    }
}