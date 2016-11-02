using System;
using MvvmCross.Binding.BindingContext;
using MvvmCross.iOS.Views;
using Tap2Give.Core.ViewModels;

namespace Tap2Give.iOS.Views
{
    [MvxFromStoryboard]
    public partial class FirstView : MvxViewController<DisasterIncidentsViewModel>
    {
        public FirstView(IntPtr handle) : base(handle)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            var set = this.CreateBindingSet<FirstView, DisasterIncidentsViewModel>();
            set.Bind(Label).To(vm => vm.Hello);
            set.Bind(TextField).To(vm => vm.Hello);
            set.Apply();
        }
    }
}
