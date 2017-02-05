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
    [Register ("EditAttributedAttributesView")]
    partial class EditAttributedAttributesView
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
            if (cancelButton != null) {
                cancelButton.Dispose ();
                cancelButton = null;
            }

            if (deleteButton != null) {
                deleteButton.Dispose ();
                deleteButton = null;
            }

            if (nameField != null) {
                nameField.Dispose ();
                nameField = null;
            }

            if (saveButton != null) {
                saveButton.Dispose ();
                saveButton = null;
            }

            if (valueField != null) {
                valueField.Dispose ();
                valueField = null;
            }
        }
    }
}