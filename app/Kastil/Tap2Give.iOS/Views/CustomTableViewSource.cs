using System;
using Foundation;
using MvvmCross.Binding.iOS.Views;
using MvvmCross.Platform.Core;
using UIKit;

namespace Tap2Give.iOS.Views
{
	public class CustomTableViewSource : MvxTableViewSource
	{
		private readonly string _cellIdentifier;
		private UITableViewCell _sizingCell;
		private readonly UITableViewCellSelectionStyle _selectionStyle;

		public CustomTableViewSource(UITableView tableView, string cellIdentifier,
									 UITableViewCellSelectionStyle selectionStyle = UITableViewCellSelectionStyle.Default)
			: base(tableView)
		{
			_selectionStyle = selectionStyle;
			_cellIdentifier = cellIdentifier;
		}

		public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
		{
			var item = GetItemAt(indexPath);
			var cell = GetOrCreateCellFor(tableView, indexPath, item);
			cell.SelectionStyle = _selectionStyle;

			var bindable = cell as IMvxDataConsumer;
			if (bindable != null) {
				SetDataContext(bindable, item);
			}

			return cell;
		}

		protected virtual void SetDataContext(IMvxDataConsumer bindable, object dataContext)
		{
			bindable.DataContext = dataContext;
		}

		protected override UITableViewCell GetOrCreateCellFor(UITableView tableView, NSIndexPath indexPath,
			object item)
		{
			return TableView.DequeueReusableCell(_cellIdentifier, indexPath);
		}

		public override nfloat GetHeightForRow(UITableView tableView, NSIndexPath indexPath)
		{
			return CalculateHeight(GetCellForHeightCalculation());
		}

		public override nfloat EstimatedHeight(UITableView tableView, NSIndexPath indexPath)
		{
			return CalculateHeight(GetCellForHeightCalculation());
		}

		private nfloat CalculateHeight(UITableViewCell cell)
		{
			cell.SetNeedsLayout();
			cell.LayoutIfNeeded();

			var size = cell.ContentView.Frame.Size;
			return NMath.Ceiling(size.Height) + 1;
		}

		private UITableViewCell GetCellForHeightCalculation()
		{
			if (_sizingCell == null)
				_sizingCell = TableView.DequeueReusableCell(_cellIdentifier);
			return _sizingCell;
		}
	}
}
