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
    [Register ("AttributedView")]
    partial class AttributedView
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

            if (saveButton != null) {
                saveButton.Dispose ();
                saveButton = null;
            }
        }
    }
}