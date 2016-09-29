using System;
using Kastil.Core.ViewModels;
using Kastil.iOS.Views;
using MvvmCross.Binding.BindingContext;
using UIKit;

namespace Kastil.iOS
{
	public partial class AssessmentListView : BaseView<AssessmentListViewModel>
	{
		private CustomTableViewSource _tableSource;

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			SetUpTableView ();

			_tableSource = new CustomTableViewSource (assessmentList, AssessmentListItemCell.Identifier);
			assessmentList.Source = _tableSource;

			var set = this.CreateBindingSet<AssessmentListView, AssessmentListViewModel> ();
			set.Bind (_tableSource).To (vm => vm.Items);
			set.Bind (_tableSource).For (t => t.SelectionChangedCommand).To (vm => vm.AssessmentSelectedCommand);

			set.Apply ();

			assessmentList.ReloadData ();

			ViewModel.Initialize ();
		}

		void SetUpTableView ()
		{
			assessmentList.TranslatesAutoresizingMaskIntoConstraints = false;
			assessmentList.SeparatorStyle = UITableViewCellSeparatorStyle.None;
			assessmentList.UserInteractionEnabled = true;

			assessmentList.RegisterNibForCellReuse (AssessmentListItemCell.Nib, AssessmentListItemCell.Identifier);
		}
	}
}

