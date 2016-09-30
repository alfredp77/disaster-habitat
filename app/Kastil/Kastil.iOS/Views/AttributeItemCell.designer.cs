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
	[Register ("AttributeItemCell")]
	partial class AttributeItemCell
	{
		[Outlet]
		UIKit.UILabel nameLabel { get; set; }

		[Outlet]
		UIKit.UILabel valueLabel { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (nameLabel != null) {
				nameLabel.Dispose ();
				nameLabel = null;
			}

			if (valueLabel != null) {
				valueLabel.Dispose ();
				valueLabel = null;
			}
		}
	}
}
