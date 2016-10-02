﻿using System;
using Android.App;
using Kastil.Droid.Views;
using Android.OS;
using Kastil.Core.ViewModels;

namespace Kastil.Droid
{
    [Activity(Label = "Shelter")]
    public class ShelterView : BaseView
    {
        protected override int LayoutResource => Resource.Layout.ShelterView;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            var vm = (ShelterViewModel)ViewModel;
            vm.Initialize();
        }
    }
}
