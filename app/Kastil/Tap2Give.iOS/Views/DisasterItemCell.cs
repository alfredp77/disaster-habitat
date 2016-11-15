using System;

using Foundation;
using Kastil.Common.ViewModels;
using MvvmCross.Binding.BindingContext;
using MvvmCross.Binding.iOS.Views;
using Tap2Give.Core.ViewModels;
using UIKit;

namespace Tap2Give.iOS.Views
{
	public partial class DisasterItemCell : MvxTableViewCell, IListenableCell
    {
		public const string Identifier = "DisasterItemCell";
		public static readonly NSString Key = new NSString(Identifier);
		public static readonly UINib Nib = UINib.FromName(Identifier, NSBundle.MainBundle);

		public DisasterItemCell(IntPtr handle) : base(handle)
		{
			Initialize();
		}

		private MvxImageViewLoader _remoteImageLoader;
		private void Initialize()
		{
			_remoteImageLoader = new MvxImageViewLoader(() => disasterImage);
			this.DelayBind(() => {
				var imgRect = UIBezierPath.FromRect(new CoreGraphics.CGRect(0, 0, 110, 100));
				disasterDescription.TextContainer.ExclusionPaths = new[] { imgRect };
				disasterDescription.TextContainer.LineBreakMode = UILineBreakMode.TailTruncation;

				container.Layer.CornerRadius = 5f;
				container.ClipsToBounds = true;

				cellSelectedButton.TouchUpInside += (s, e) =>
				{
				    if (CellListener != null)
				    {
                        CellListener.CellSelected(this.DataContext);
				    }
				};
				var set = this.CreateBindingSet<DisasterItemCell, DisasterListItemViewModel>();
				set.Bind(disasterName).To(vm => vm.Name);
				set.Bind(disasterDescription).To(vm => vm.Description);
				set.Bind(_remoteImageLoader).For(i => i.ImageUrl).To(vm => vm.ImageUrl);
				set.Apply();
			});
		}

        public ITableCellListener CellListener { get; set; }		
	}

	public interface ITableCellListener
	{
		void CellSelected(object vm);
	}

    public interface IListenableCell
    {
        ITableCellListener CellListener { get; set; }
    }
}
