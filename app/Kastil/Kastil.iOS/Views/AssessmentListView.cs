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
			SetUpTableView (assessmentList, AssessmentListItemCell.Nib, AssessmentListItemCell.Identifier);

			_tableSource = new CustomTableViewSource (assessmentList, AssessmentListItemCell.Identifier);
			assessmentList.Source = _tableSource;

			var set = CreateBindingSet<AssessmentListView> ();
			set.Bind (_tableSource).To (vm => vm.Items);
			set.Bind (_tableSource).For (t => t.SelectionChangedCommand).To (vm => vm.AssessmentSelectedCommand);

			set.Apply ();

			assessmentList.ReloadData ();

			ViewModel.Initialize ();
		}
	}
}

