using System;
using System.Collections.Generic;
using System.Text;
using Kastil.Core.ViewModels;
using Kastil.iOS.PlatformSpecific;
using MvvmCross.iOS.Views;

namespace Kastil.iOS.Views
{
    public abstract class BaseView<T> : MvxViewController<T> where T : BaseViewModel
    {
        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            CreateNavBarItems();
        }

        protected virtual void CreateNavBarItems()
        {
            this.AddLeftButton(ButtonTypes.Setting, (s, e) => { });
            this.AddRightButton(ButtonTypes.Add, (s, e) => { /* do something here */ });
        }
    }
}
