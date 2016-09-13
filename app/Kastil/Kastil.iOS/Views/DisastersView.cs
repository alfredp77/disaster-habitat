using Kastil.Core.ViewModels;
using MvvmCross.Binding.BindingContext;
using MvvmCross.iOS.Views;
using UIKit;

namespace Kastil.iOS.Views
{
    public partial class DisastersView : BaseView<DisastersViewModel>
    {
        private CustomTableViewSource _tableSource;
        private MvxUIRefreshControl _refreshControl;

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            SetUpTableView();

            _tableSource = new CustomTableViewSource(disastersTable, DisasterItemCell.Identifier);
            disastersTable.Source = _tableSource;
            _refreshControl = new MvxUIRefreshControl();
            disastersTable.AddSubview(_refreshControl);

            var set = this.CreateBindingSet<DisastersView, DisastersViewModel>();
            set.Bind(_tableSource).To(vm => vm.Items);
            set.Bind(_refreshControl).For(r => r.RefreshCommand).To(vm => vm.RefreshCommand);
            set.Bind(_refreshControl).For(r => r.IsRefreshing).To(vm => vm.IsLoading);

            set.Apply();

            disastersTable.ReloadData();

            ViewModel.Initialize();
        }

        void SetUpTableView()
        {
            disastersTable.TranslatesAutoresizingMaskIntoConstraints = false;
            disastersTable.SeparatorStyle = UITableViewCellSeparatorStyle.None;
            disastersTable.UserInteractionEnabled = true;

            disastersTable.RegisterNibForCellReuse(DisasterItemCell.Nib, DisasterItemCell.Identifier);
        }
    }
}