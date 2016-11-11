// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;

namespace Tap2Give.iOS.Views
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
