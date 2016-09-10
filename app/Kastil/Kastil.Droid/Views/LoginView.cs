using Android.App;

namespace Kastil.Droid.Views
{
    [Activity(Label = "Login")]
    public class LoginView : BaseView
    {
        protected override int LayoutResource => Resource.Layout.LoginView;

        protected override void SetupToolbar()
        {
            // do nothing
        }
    }
}