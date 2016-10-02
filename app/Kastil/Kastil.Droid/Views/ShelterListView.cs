using System;
using Android.App;
using Kastil.Droid.Views;
using Android.OS;
using Kastil.Core.ViewModels;

namespace Kastil.Droid
{
	[Activity(Label= "ShelterList")]
	public class ShelterListView : BaseView
	{
		protected override int LayoutResource => Resource.Layout.ShelterListView;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            var vm = (ShelterListViewModel)ViewModel;
            vm.Initialize();
        }
    }
}

