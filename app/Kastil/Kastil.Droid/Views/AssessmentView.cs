using Android.App;
using Android.OS;
using Kastil.Core.ViewModels;

namespace Kastil.Droid.Views
{
    [Activity(Label = "Assessment")]
    public class AssessmentView : BaseView
    {
        protected override int LayoutResource => Resource.Layout.AssessmentView;
    }
}