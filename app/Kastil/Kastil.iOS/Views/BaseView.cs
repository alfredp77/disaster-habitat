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

        private void CreateNavBarItems()
        {
            if (ViewModel.SettingCommand != null)
                this.AddLeftButton(ButtonTypes.Setting, async (s, e) => await ViewModel.SettingCommand.ExecuteAsync());
            if (ViewModel.AddCommand != null)
                this.AddRightButton(ButtonTypes.Add, async (s, e) => await ViewModel.AddCommand.ExecuteAsync());
        }

    }
}
