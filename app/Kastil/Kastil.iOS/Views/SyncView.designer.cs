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
    [Register ("SyncView")]
    partial class SyncView
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITextField email { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.NSLayoutConstraint emailLeading { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITextField password { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton syncButton { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (email != null) {
                email.Dispose ();
                email = null;
            }

            if (emailLeading != null) {
                emailLeading.Dispose ();
                emailLeading = null;
            }

            if (password != null) {
                password.Dispose ();
                password = null;
            }

            if (syncButton != null) {
                syncButton.Dispose ();
                syncButton = null;
            }
        }
    }
}