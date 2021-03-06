﻿using System;

using Foundation;
using Kastil.Core.ViewModels;
using MvvmCross.Binding.BindingContext;
using MvvmCross.Binding.iOS.Views;
using UIKit;

namespace Kastil.iOS
{
	public partial class AttributedListItemCell : MvxTableViewCell
	{
		public const string Identifier = "AttributedListItemCell";
		public static readonly NSString Key = new NSString (Identifier);
		public static readonly UINib Nib = UINib.FromName (Identifier, NSBundle.MainBundle);

		public AttributedListItemCell (IntPtr handle) : base(handle)
        {
			Initialize ();
		}

		private void Initialize ()
		{
			this.DelayBind (() => {
				var set = this.CreateBindingSet<AttributedListItemCell, AttributedListItemViewModel> ();
				set.Bind (nameLabel).To (vm => vm.Text);
				set.Bind (locationLabel).To (vm => vm.LocationName);
				set.Apply ();
			});
		}
	}
}
