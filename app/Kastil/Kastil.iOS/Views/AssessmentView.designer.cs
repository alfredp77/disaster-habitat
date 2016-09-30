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
	[Register ("AssessmentView")]
	partial class AssessmentView
	{
		[Outlet]
		UIKit.UIButton addAttributeButton { get; set; }

		[Outlet]
		UIKit.UITableView attributesList { get; set; }

		[Outlet]
		UIKit.UIButton cancelButton { get; set; }

		[Outlet]
		UIKit.UITextField locationField { get; set; }

		[Outlet]
		UIKit.UILabel locationLabel { get; set; }

		[Outlet]
		UIKit.UITextField nameField { get; set; }

		[Outlet]
		UIKit.UILabel nameLabel { get; set; }

		[Outlet]
		UIKit.UIButton saveButton { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (addAttributeButton != null) {
				addAttributeButton.Dispose ();
				addAttributeButton = null;
			}

			if (attributesList != null) {
				attributesList.Dispose ();
				attributesList = null;
			}

			if (cancelButton != null) {
				cancelButton.Dispose ();
				cancelButton = null;
			}

			if (locationField != null) {
				locationField.Dispose ();
				locationField = null;
			}

			if (locationLabel != null) {
				locationLabel.Dispose ();
				locationLabel = null;
			}

			if (nameField != null) {
				nameField.Dispose ();
				nameField = null;
			}

			if (nameLabel != null) {
				nameLabel.Dispose ();
				nameLabel = null;
			}

			if (saveButton != null) {
				saveButton.Dispose ();
				saveButton = null;
			}
		}
	}
}
