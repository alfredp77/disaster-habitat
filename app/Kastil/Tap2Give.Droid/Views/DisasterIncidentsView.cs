using Android.App;
using Android.OS;

namespace Tap2Give.Droid.Views
{
    [Activity(
        Label = "Tap2Give"
        , Icon = "@drawable/icon"
        , NoHistory = true
        , LaunchMode = Android.Content.PM.LaunchMode.SingleTop
        , ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
    public class DisasterIncidentsView : BaseView
    {
        //private BindableProgress _bindableProgress;
        protected override int LayoutResource => Resource.Layout.DisasterIncidentsView;
    }
}
