using Kastil.Core.ViewModels;
using Kastil.iOS.Views;

namespace Kastil.iOS
{
	public partial class AttributedListView : BaseView<AttributedListViewModel>
	{
		private CustomTableViewSource _tableSource;

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			SetUpTableView (assessmentList, AttributedListItemCell.Nib, AttributedListItemCell.Identifier);

			_tableSource = new CustomTableViewSource (assessmentList, AttributedListItemCell.Identifier);
			assessmentList.Source = _tableSource;

			var set = CreateBindingSet<AttributedListView> ();
			set.Bind (_tableSource).To (vm => vm.Items);
			set.Bind (_tableSource).For (t => t.SelectionChangedCommand).To (vm => vm.ItemSelectedCommand);

			set.Apply ();

			assessmentList.ReloadData ();

			ViewModel.Initialize ();
		}
	}
}

