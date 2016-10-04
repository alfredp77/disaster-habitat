using Android.App;
using Android.OS;
using Kastil.Core.ViewModels;

namespace Kastil.Droid.Views
{
    [Activity(Label = "Disasters")]
    public class DisastersView : BaseView
    {
        protected override int LayoutResource => Resource.Layout.DisastersView;
    }
}