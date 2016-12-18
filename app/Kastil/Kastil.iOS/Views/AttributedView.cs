using System;
using Kastil.Core.ViewModels;
using Kastil.iOS.Views;
using UIKit;

namespace Kastil.iOS
{
	public partial class AttributedView : BaseView<AttributedViewModel>
	{		
		private CustomTableViewSource _tableSource;

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			SetUpTableView (attributesList, AttributeItemCell.Nib, AttributeItemCell.Identifier);

			_tableSource = new CustomTableViewSource(attributesList, AttributeItemCell.Identifier);
			attributesList.Source = _tableSource;
			locationLabel.Hidden = true;

			var set = CreateBindingSet<AttributedView>();
			set.Bind(nameField).For(f => f.Placeholder).To(vm => vm.NamePlaceholderText);
			set.Bind(locationField).For(f => f.Placeholder).To(vm => vm.LocationPlaceholderText);
			set.Bind(nameField).To(vm => vm.Name);
			set.Bind(locationField).To(vm => vm.Location);
			set.Bind(addAttributeButton).To(vm => vm.AddAttributeCommand);
			set.Bind(saveButton).To(vm => vm.SaveCommand);
			set.Bind(cancelButton).To(vm => vm.CancelCommand);

			set.Bind(_tableSource).To(vm => vm.Attributes);
			set.Bind(_tableSource).For(t => t.SelectionChangedCommand).To(vm => vm.AttributeSelectedCommand);
			set.Apply();

			ViewModel.Initialize().ContinueWith(_ => attributesList.ReloadData());
		}
	}
}

