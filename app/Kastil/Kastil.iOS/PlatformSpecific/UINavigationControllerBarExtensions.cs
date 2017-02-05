using System;
using CoreGraphics;
using Kastil.iOS.Views;
using MvvmCross.iOS.Views;
using UIKit;

namespace Kastil.iOS.PlatformSpecific
{
    public static class UINavigationControllerBarExtensions
    {
        public static UIBarButtonItem AddLeftButton(this MvxViewController view, 
            ButtonType buttonType, EventHandler eventHandler)
        {
            var button = CreateButton(buttonType);
			button.TouchUpInside += eventHandler;
            var leftNavButton = new UIBarButtonItem(button);
            view.NavigationItem.SetLeftBarButtonItem(leftNavButton, false);
            return leftNavButton;
        }

		public static UIBarButtonItem AddRightButton(this MvxViewController view,
            ButtonType buttonType, EventHandler eventHandler)
        {
            var button = CreateButton(buttonType);
			button.TouchUpInside += eventHandler;
            var rightNavButton = new UIBarButtonItem(button);
            view.NavigationItem.SetRightBarButtonItem(rightNavButton, false);
            return rightNavButton;
        }


        private static UIButton CreateButton(ButtonType buttonType)
        {
            var button = UIButton.FromType(UIButtonType.System);
            if (buttonType.Image != null)
            {
                button.Frame = new CGRect(0, 0, buttonType.Image.Size.Width, buttonType.Image.Size.Height);
                button.SetBackgroundImage(buttonType.Image, UIControlState.Normal);
                button.ContentEdgeInsets = new UIEdgeInsets(10, 20, 10, 20);
            }
            else
            {
                button.SetTitle(buttonType.Text, UIControlState.Normal);
                button.TitleLabel.Font = UIFont.FromName("HelveticaNeue", 14);
                button.Frame = new CGRect(0, 0, buttonType.Text.Length + 45, button.TitleLabel.Frame.Height);
                button.SetTitleColor(UIColor.Black, UIControlState.Normal);
            }
            return button;
        }
    }

    public class ButtonType
    {
        public string Text { get; set; }
        public UIImage Image { get; set; }
    }

    public static class ButtonTypes
    {
		public static ButtonType Setting = new ButtonType { Image = UIImage.FromBundle("NavigationMenuSetting")};
		public static ButtonType Sync = new ButtonType { Image = UIImage.FromBundle("NavigationMenuSync") };
		public static ButtonType Done = new ButtonType { Image = UIImage.FromBundle("NavigationDone") };
		public static ButtonType Next = new ButtonType { Image = UIImage.FromBundle("NavigationDone") };
		public static ButtonType Back = new ButtonType { Text = "Back" };
		public static ButtonType Close = new ButtonType { Image = UIImage.FromBundle("NavigationClose") };
		public static ButtonType Add = new ButtonType { Image = UIImage.FromBundle("NavigationAdd") };
    }

	
}
