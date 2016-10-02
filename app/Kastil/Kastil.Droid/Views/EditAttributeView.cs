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

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            var vm = (EditAttributeViewModel)ViewModel;
			vm.Initialize ().ContinueWith (_ => {
				var spinner = FindViewById<Spinner> (Resource.Id.attributeSpinner);
				spinner.Enabled = !vm.EditMode;});
        }
    }
}

