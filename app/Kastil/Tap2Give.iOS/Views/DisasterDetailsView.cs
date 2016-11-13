using System;
using MvvmCross.Binding.iOS.Views;
using Tap2Give.Core.ViewModels;
using Tap2Give.iOS.PlatformSpecific;
using Tap2Give.iOS.Views;
using UIKit;

namespace Tap2Give.iOS.Views
{
	public partial class DisasterDetailsView : BaseView<DisasterDetailsViewModel>
	{
		private MvxImageViewLoader _remoteImageLoader;
		private MvxStandardTableViewSource _tableSource;

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();
			disasterDescription.LayoutManager.EnsureLayoutForTextContainer(disasterDescription.TextContainer);
			
			this.AddLeftButton(ButtonTypes.Close, (s, e) => ViewModel.CancelCommand.Execute());

			_remoteImageLoader = new MvxImageViewLoader(() => disasterImage);

			_tableSource = new MvxStandardTableViewSource(aidTable);
			aidTable.Source = _tableSource;

			var set = CreateBindingSet<DisasterDetailsView>();
			set.Bind(_remoteImageLoader).For(i => i.ImageUrl).To(vm => vm.ImageUrl);
			set.Bind(disasterDescription).For(d => d.Text).To(vm => vm.Description);
			set.Bind(donateButton).To(vm => vm.DonateCommand);
			set.Bind(_tableSource).To(vm => vm.AidValues);
			set.Apply();

			ViewModel.Initialize().ContinueWith(t => 
			{
				aidTable.ReloadData();
				View.SetNeedsDisplay();
			});
		}
	}
}

