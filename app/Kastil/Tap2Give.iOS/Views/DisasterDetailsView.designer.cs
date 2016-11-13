// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;

namespace Tap2Give.iOS.Views
{
	[Register ("DisasterDetailsView")]
	partial class DisasterDetailsView
	{
		[Outlet]
		UIKit.UITableView aidTable { get; set; }

		[Outlet]
		UIKit.UITextView disasterDescription { get; set; }

		[Outlet]
		UIKit.UIImageView disasterImage { get; set; }

		[Outlet]
		UIKit.UIButton donateButton { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (aidTable != null) {
				aidTable.Dispose ();
				aidTable = null;
			}

			if (disasterDescription != null) {
				disasterDescription.Dispose ();
				disasterDescription = null;
			}

			if (disasterImage != null) {
				disasterImage.Dispose ();
				disasterImage = null;
			}

			if (donateButton != null) {
				donateButton.Dispose ();
				donateButton = null;
			}
		}
	}
}
