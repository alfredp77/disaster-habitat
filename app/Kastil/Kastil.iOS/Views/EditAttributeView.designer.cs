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
	[Register ("EditAttributeView")]
	partial class EditAttributeView
	{
		[Outlet]
		UIKit.UIButton cancelButton { get; set; }

		[Outlet]
		UIKit.UIButton deleteButton { get; set; }

		[Outlet]
		UIKit.UITextField nameField { get; set; }

		[Outlet]
		UIKit.UIButton saveButton { get; set; }

		[Outlet]
		UIKit.UITextField valueField { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (saveButton != null) {
				saveButton.Dispose ();
				saveButton = null;
			}

			if (deleteButton != null) {
				deleteButton.Dispose ();
				deleteButton = null;
			}

			if (cancelButton != null) {
				cancelButton.Dispose ();
				cancelButton = null;
			}

			if (nameField != null) {
				nameField.Dispose ();
				nameField = null;
			}

			if (valueField != null) {
				valueField.Dispose ();
				valueField = null;
			}
		}
	}
}
