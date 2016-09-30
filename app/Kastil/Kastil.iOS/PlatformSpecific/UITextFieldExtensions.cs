using System;
using System.Drawing;
using CoreGraphics;
using Kastil.iOS.Views;
using MvvmCross.Binding.iOS.Views;
using MvvmCross.iOS.Views;
using UIKit;

namespace Kastil.iOS.PlatformSpecific
{
	public static class UITextFieldExtensions
	{
		public static MvxPickerViewModel CreatePicker(this UITextField textField)
		{
			var picker = new UIPickerView();
			var pickerViewModel = new MvxPickerViewModel(picker);
			picker.Model = pickerViewModel;
			picker.ShowSelectionIndicator = true;

			var toolbar = new UIToolbar();
			toolbar.BarStyle = UIBarStyle.Black;
			toolbar.Translucent = true;
			toolbar.SizeToFit();
			var doneButton = new UIBarButtonItem("Done", UIBarButtonItemStyle.Done, (s, e) => {
				//This dismisses the picker view and returns control to the main screen.
				textField.ResignFirstResponder();
			});
			toolbar.SetItems(new UIBarButtonItem[] { doneButton }, true);

			//This associates the picker view with the textview           
			textField.InputView = picker;

			//This will insert the toolbar into the pickerview.  This will allow the user to dismiss the 
			//view once a choice has been made.
			textField.InputAccessoryView = toolbar;

			return pickerViewModel;
		}
	}
}
