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
    [Register ("HomeView")]
    partial class HomeView
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton logoutButton { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (logoutButton != null) {
                logoutButton.Dispose ();
                logoutButton = null;
            }
        }
    }
}