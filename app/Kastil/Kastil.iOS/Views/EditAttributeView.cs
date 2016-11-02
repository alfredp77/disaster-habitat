using System;
using Kastil.Core.ViewModels;
using Kastil.iOS.PlatformSpecific;
using Kastil.iOS.Views;
using UIKit;
using Kastil.Common.Models;

namespace Kastil.iOS
{
	public partial class EditAttributeView : BaseView<EditAssessmentAttributeViewModel>
	{
		public override void ViewDidLoad()
		{
			base.ViewDidLoad();

            //TODO: Sort out the Shelter or Assessment
           
			var set = CreateBindingSet<EditAttributeView>();
			set.Bind(nameField).To(vm => vm.SelectedItem.Caption);
			set.Bind(valueField).To(vm => vm.AttributeValue);
			nameField.ShouldChangeCharacters = (textField, range, replacementString) => false;

			var pickerViewModel = nameField.CreatePicker();
			set.Bind(pickerViewModel).For(p => p.SelectedItem).To(vm => vm.SelectedItem);
			set.Bind(pickerViewModel).For(p => p.ItemsSource).To(vm => vm.Items);
			set.Bind(saveButton).To(vm => vm.AddClickCommand);
			set.Bind(deleteButton).To(vm => vm.DeleteClickCommand);
			set.Bind(deleteButton).For("Visibility").To(vm => vm.EditMode).WithConversion("Visibility");
			set.Bind(cancelButton).To(vm => vm.CancelClickCommand);
			set.Apply();
           

			ViewModel.Initialize();
		}

	}
}

