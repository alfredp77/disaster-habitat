using System;
using System.Collections.Generic;
using System.Text;
using Kastil.Common.ViewModels;
using Kastil.Core.ViewModels;
using Kastil.iOS.PlatformSpecific;
using MvvmCross.Binding.BindingContext;
using MvvmCross.iOS.Views;
using UIKit;

namespace Kastil.iOS.Views
{
    public abstract class BaseView<T> : MvxViewController<T> where T : BaseViewModel
    {
        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
			NavigationController.NavigationBar.Translucent = false;
			EdgesForExtendedLayout = UIKit.UIRectEdge.Top;
            CreateNavBarItems();
        }

		protected MvxFluentBindingDescriptionSet<TView, T> CreateBindingSet<TView> () where TView : BaseView<T>
		{
			var theView = this as TView;
			var set = theView.CreateBindingSet<TView, T>();
			set.Bind().For(v => v.Title).To (vm => vm.Title);
			return set;
		}

        private void CreateNavBarItems()
        {
			if (ViewModel.SettingCommand != null)
				this.AddLeftButton(ButtonTypes.Setting, async (s, e) => await ViewModel.SettingCommand.ExecuteAsync());
			else
				this.AddLeftButton(ButtonTypes.Close, (s, e) => ViewModel.Close());
			
            if (ViewModel.AddCommand != null)
                this.AddRightButton(ButtonTypes.Add, async (s, e) => await ViewModel.AddCommand.ExecuteAsync());
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
