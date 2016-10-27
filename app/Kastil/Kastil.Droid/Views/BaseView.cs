using Android.OS;
using Android.Support.V7.Widget;
using Kastil.Common.ViewModels;
using Kastil.Core.ViewModels;
using MvvmCross.Droid.Support.V7.AppCompat;

namespace Kastil.Droid.Views
{
	public abstract class BaseView : MvxAppCompatActivity
	{
		protected Toolbar Toolbar { get; set; }

		protected override void OnCreate(Bundle bundle)
		{
			base.OnCreate(bundle);

			SetContentView(LayoutResource);

			Toolbar = FindViewById<Toolbar>(Resource.Id.toolbar);
			if (Toolbar != null) {
				SetSupportActionBar(Toolbar);
				SetupToolbar();
			}

			var vm = (BaseViewModel)ViewModel;
			vm.Initialize();
		}

		protected abstract int LayoutResource { get; }

		protected virtual void SetupToolbar()
		{
			// this is displaying the back button on the toolbar
			//            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
			//            SupportActionBar.SetHomeButtonEnabled(true);
		}
	}
}
