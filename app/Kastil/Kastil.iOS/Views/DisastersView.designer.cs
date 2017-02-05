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
    [Register ("DisastersView")]
    partial class DisastersView
    {
        [Outlet]
        UIKit.UITableView disastersTable { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (disastersTable != null) {
                disastersTable.Dispose ();
                disastersTable = null;
            }
        }
    }
}