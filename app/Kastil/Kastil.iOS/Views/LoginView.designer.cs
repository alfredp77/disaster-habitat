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

namespace Kastil.iOS.Views
{
    [Register ("LoginView")]
    partial class LoginView
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton loginButton { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITextField staffCodeField { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (loginButton != null) {
                loginButton.Dispose ();
                loginButton = null;
            }

            if (staffCodeField != null) {
                staffCodeField.Dispose ();
                staffCodeField = null;
            }
        }
    }
}