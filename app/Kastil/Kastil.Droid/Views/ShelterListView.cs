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
    }
}

