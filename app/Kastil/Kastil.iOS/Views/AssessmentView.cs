using System;
using Kastil.Core.ViewModels;
using Kastil.iOS.Views;
using UIKit;

namespace Kastil.iOS
{
	public partial class AssessmentView : BaseView<AssessmentViewModel>
	{		
		private CustomTableViewSource _tableSource;

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			SetUpTableView (attributesList, AttributeItemCell.Nib, AttributeItemCell.Identifier);

			_tableSource = new CustomTableViewSource(attributesList, AttributeItemCell.Identifier, 
			                                         UITableViewCellSelectionStyle.None);
			attributesList.Source = _tableSource;

			var set = CreateBindingSet<AssessmentView>();
			set.Bind(nameField).For("Visibility").To(vm => vm.AddMode).WithConversion("Visibility");
			set.Bind(nameField).For(f => f.Placeholder).To(vm => vm.NamePlaceholderText);
			set.Bind(locationLabel).For("Visibility").To(vm => vm.AddMode).WithConversion("InvertedVisibility");
			set.Bind(locationField).For(f => f.UserInteractionEnabled).To(vm => vm.AddMode);
			set.Bind(locationField).For(f => f.Placeholder).To(vm => vm.LocationPlaceholderText);
			set.Bind(nameField).To(vm => vm.Name);
			set.Bind(locationField).To(vm => vm.Location);
			set.Bind(addAttributeButton).To(vm => vm.AddAttributeCommand);
			set.Bind(saveButton).To(vm => vm.SaveCommand);
			set.Bind(cancelButton).To(vm => vm.CancelCommand);

			set.Bind(_tableSource).To(vm => vm.Attributes);
			set.Bind(_tableSource).For(t => t.SelectionChangedCommand).To(vm => vm.AttributeSelectedCommand);
			set.Apply();

			attributesList.ReloadData();

			ViewModel.Initialize();
		}
	}
}

