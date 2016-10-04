using Android.App;
using Android.OS;
using Android.Widget;
using Kastil.Core.ViewModels;

namespace Kastil.Droid.Views
{
    [Activity(Label = "EditAttribute")]
    public class EditAttributeView : BaseView 
    {
        protected override int LayoutResource => Resource.Layout.EditAttributeView;
    }
}

