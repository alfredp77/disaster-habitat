using System;
using System.Globalization;
using Android.Text;
using MvvmCross.Platform.Converters;

namespace Kastil.Droid.PlatformSpecific
{
    public class EditTextEnabledValueConverter : MvxValueConverter<bool, InputTypes>
    {
        protected override InputTypes Convert(bool value, Type targetType, object parameter, CultureInfo culture)
        {
			if (value)
				return InputTypes.ClassText;

            return InputTypes.Null;
        }
    }
}