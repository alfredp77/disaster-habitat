using Android.App;
using Android.OS;

namespace Tap2Give.Droid.Views
{
    [Activity(
        Label = "Tap2Give"
        , Icon = "@drawable/icon"
        , ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
    public class DisastersView : BaseView
    {
        protected override int LayoutResource => Resource.Layout.DisastersView;
    }
}
