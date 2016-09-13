using System;

using Foundation;
using Kastil.Core.ViewModels;
using MvvmCross.Binding.BindingContext;
using MvvmCross.Binding.iOS.Views;
using UIKit;

namespace Kastil.iOS.Views
{
    public partial class DisasterItemCell : MvxTableViewCell
    {
        public const string Identifier = "DisasterItemCell";
        public static readonly NSString Key = new NSString(Identifier);
        public static readonly UINib Nib  = UINib.FromName(Identifier, NSBundle.MainBundle);

        public DisasterItemCell(IntPtr handle) : base(handle)
        {
            Initialize();
        }

        private void Initialize()
        {
            this.DelayBind(() => {
                var set = this.CreateBindingSet<DisasterItemCell, DisasterListItemViewModel>();
                set.Bind(nameLabel).To(vm => vm.Text);
                set.Bind(dateLabel).To(vm => vm.When);
                set.Apply();
            });
        }
    }
}
