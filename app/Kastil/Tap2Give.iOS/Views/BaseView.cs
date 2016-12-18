using Kastil.Common.ViewModels;
using MvvmCross.Binding.BindingContext;
using MvvmCross.iOS.Views;
using UIKit;

namespace Tap2Give.iOS.Views
{
    public abstract class BaseView<T> : MvxViewController<T> where T : BaseViewModel
    {
        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
			NavigationController.NavigationBar.Translucent = false;
			EdgesForExtendedLayout = UIKit.UIRectEdge.Top;
        }

		protected MvxFluentBindingDescriptionSet<TView, T> CreateBindingSet<TView> () where TView : BaseView<T>
		{
			var theView = this as TView;
			var set = theView.CreateBindingSet<TView, T>();
			set.Bind().For(v => v.Title).To (vm => vm.Title);
			return set;
		}

		protected virtual void SetUpTableView(UITableView tableView, UINib nib, string identifier)
		{
			tableView.TranslatesAutoresizingMaskIntoConstraints = false;
			tableView.SeparatorStyle = UITableViewCellSeparatorStyle.None;
			tableView.UserInteractionEnabled = true;

			tableView.RegisterNibForCellReuse (nib, identifier);
		}

    }
}
