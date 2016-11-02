using Android.App;
using Android.Content.PM;
using MvvmCross.Droid.Views;

namespace Tap2Give.Droid
{
    [Activity(
        Label = "Tap2Give.Droid"
        , Icon = "@drawable/icon"
        , MainLauncher = true
        , Theme = "@style/Theme.Splash"
        , NoHistory = true
        , ScreenOrientation = ScreenOrientation.Portrait)]
    public class SplashScreen : MvxSplashScreenActivity
    {
        public SplashScreen()
            : base(Resource.Layout.SplashScreen)
        {
        }
    }
}
