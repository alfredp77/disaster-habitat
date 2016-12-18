// WARNING
//
// This file has been generated automatically by Xamarin Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System;
using System.CodeDom.Compiler;
using UIKit;

namespace Kastil.iOS
{
    [Register ("AttributedListItemCell")]
    partial class AttributedListItemCell
    {
        [Outlet]
        UIKit.UILabel locationLabel { get; set; }


        [Outlet]
        UIKit.UILabel nameLabel { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (locationLabel != null) {
                locationLabel.Dispose ();
                locationLabel = null;
            }

            if (nameLabel != null) {
                nameLabel.Dispose ();
                nameLabel = null;
            }
        }
    }
}