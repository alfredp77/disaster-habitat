using System;
using System.Collections.Generic;
using System.Text;
using Kastil.iOS.Views;
using MvvmCross.Core.ViewModels;
using MvvmCross.iOS.Views;
using MvvmCross.iOS.Views.Presenters;
using UIKit;

namespace Kastil.iOS.PlatformSpecific
{
    public class ViewPresenter : MvxModalSupportIosViewPresenter
    {
        public ViewPresenter(UIApplicationDelegate applicationDelegate, UIWindow window) : base(applicationDelegate, window)
        {
        }

        private MvxViewController _modalView = null;

        public override void Show(IMvxIosView view)
        {
            if (_modalView == null && view is IMvxModalIosView && view is MvxViewController)
            {
                _modalView = (MvxViewController)view;
                PresentModalViewController(new UINavigationController(_modalView), true);                
            }
            else
            {
               base.Show(view);
            }
        }

        public override void Close(IMvxViewModel toClose)
        {
            if (_modalView != null && _modalView.ViewModel == toClose)
            {
                _modalView.NavigationController.DismissViewController(true, () => { });
                _modalView = null;
                return;
            }
            base.Close(toClose);
        }
    }
}
