// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;

namespace Kastil.iOS
{
	[Register ("SyncView")]
	partial class SyncView
	{
		[Outlet]
		UIKit.UITextField staffCode { get; set; }

		[Outlet]
		UIKit.UIButton syncButton { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (staffCode != null) {
				staffCode.Dispose ();
				staffCode = null;
			}

			if (syncButton != null) {
				syncButton.Dispose ();
				syncButton = null;
			}
		}
	}
}
