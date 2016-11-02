using Android.OS;
using Android.Support.V7.Widget;
using Kastil.Common.ViewModels;
using MvvmCross.Droid.Support.V7.AppCompat;

namespace Tap2Give.Droid.Views
{
    public abstract class BaseView : MvxAppCompatActivity
    {
        protected Toolbar Toolbar { get; set; }

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            SetContentView(LayoutResource);

            Toolbar = FindViewById<Toolbar>(Resource.Id.toolbar);
            if (Toolbar != null)
            {
                SetSupportActionBar(Toolbar);
                SupportActionBar.SetDisplayHomeAsUpEnabled(true);
                SupportActionBar.SetHomeButtonEnabled(true);
            }

            var vm = (BaseViewModel)ViewModel;
            vm.Initialize();
        }

        protected abstract int LayoutResource { get; }
    }
}
