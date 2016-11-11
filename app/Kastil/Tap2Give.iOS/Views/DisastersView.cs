using System;
using Tap2Give.Core.ViewModels;
using UIKit;

namespace Tap2Give.iOS.Views
{
    public partial class DisastersView : BaseView<DisastersViewModel>
    {
		private CustomTableViewSource _tableSource;

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

			SetUpTableView(disastersTable, DisasterItemCell.Nib, DisasterItemCell.Identifier);

			_tableSource = new CustomTableViewSource(disastersTable, DisasterItemCell.Identifier);
			disastersTable.Source = _tableSource;

			var set = CreateBindingSet<DisastersView>();
			set.Bind(_tableSource).To(vm => vm.DisasterIncidents);
			//set.Bind(_tableSource).For(t => t.SelectionChangedCommand).To(vm => vm.ShowDetailsCommand);

			set.Apply();

			disastersTable.ReloadData();

			ViewModel.Initialize();
        }
    }
}