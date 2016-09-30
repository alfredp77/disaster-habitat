using Kastil.Core.ViewModels;
using Kastil.iOS.Views;
using MvvmCross.Binding.BindingContext;

namespace Kastil.iOS
{
	public partial class SyncView : BaseView<SyncViewModel>
	{
		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			var set = CreateBindingSet<SyncView>();
			set.Bind (syncButton).To (vm => vm.SyncCommand);
			set.Bind (staffCode).To (vm => vm.StaffCode);
			set.Apply ();
		}
	}
}

