using System;

using Foundation;
using Kastil.Core.ViewModels;
using MvvmCross.Binding.BindingContext;
using MvvmCross.Binding.iOS.Views;
using UIKit;

namespace Kastil.iOS
{
	public partial class AttributeItemCell : MvxTableViewCell
	{
		public const string Identifier = "AttributeItemCell";
		public static readonly NSString Key = new NSString (Identifier);
		public static readonly UINib Nib = UINib.FromName (Identifier, NSBundle.MainBundle);

		public AttributeItemCell (IntPtr handle) : base(handle)
        {
			Initialize ();
		}

		private void Initialize ()
		{
			this.DelayBind (() => {
				var set = this.CreateBindingSet<AttributeItemCell, ValuedAttributeListItemViewModel> ();
				set.Bind (nameLabel).To (vm => vm.Key);
				set.Bind (valueLabel).To (vm => vm.Value);
				set.Apply ();
			});
		}
	}
}
