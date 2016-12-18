using System;
using Kastil.Core.ViewModels;
using Kastil.iOS.Views;
using UIKit;

namespace Kastil.iOS
{
	public partial class SyncView : BaseView<SyncViewModel>
	{
		public override void ViewDidLoad()
		{
			base.ViewDidLoad();

			var set = CreateBindingSet<SyncView>();
			set.Bind(email).To(vm => vm.Email);
			set.Bind(password).To(vm => vm.Password);
			set.Bind(syncButton).To(vm => vm.SyncCommand);
			set.Apply();

			ViewModel.Initialize();
		}
	}
}

