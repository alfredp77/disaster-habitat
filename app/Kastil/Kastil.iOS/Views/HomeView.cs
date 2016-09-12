using System;
using Kastil.Core.ViewModels;
using MvvmCross.Binding.BindingContext;
using UIKit;

namespace Kastil.iOS.Views
{
    public partial class HomeView : BaseView<HomeViewModel>
    {
        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            var set = this.CreateBindingSet<HomeView, HomeViewModel>();
            set.Bind(logoutButton).To(vm => vm.LogoutCommand);
            set.Apply();
        }
    }
}