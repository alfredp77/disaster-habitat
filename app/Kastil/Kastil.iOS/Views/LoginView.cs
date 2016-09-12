using System;
using Kastil.Core.ViewModels;
using MvvmCross.Binding.BindingContext;
using MvvmCross.iOS.Views;
using UIKit;

namespace Kastil.iOS.Views
{
    public partial class LoginView : BaseView<LoginViewModel>
    {
        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            var set = this.CreateBindingSet<LoginView, LoginViewModel>();
            set.Bind(loginButton).To(vm => vm.LoginCommand);
            set.Bind(staffCodeField).To(vm => vm.StaffCode);
            set.Apply();
        }

        protected override void CreateNavBarItems()
        {
            // do nothing!
        }
    }
}