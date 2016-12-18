using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;

namespace Tap2Give.Droid.Views
{
    [Activity(Label = "Disaster Aid")]
    public class DisasterDetailsView : BaseView
    {
        protected override int LayoutResource => Resource.Layout.DisasterDetailsView;
    }
}