using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using MvvmCross.Binding;
using MvvmCross.Binding.Droid.Target;

namespace Tap2Give.Droid.Bindings
{
    public class LayoutBackgroundBinding : MvxAndroidTargetBinding
    {
        private readonly RelativeLayout _layout;
        public LayoutBackgroundBinding(RelativeLayout view) : base(view)
        {
            this._layout = view;
        }
        protected override void SetValueImpl(object target, object value)
        {
            
        }

        public override void SetValue(object value)
        {
            _layout.SetBackgroundResource(Resource.Drawable.T2H_Icon);
        }
        public override Type TargetType
        {
            get { return typeof(DateTime); }
        }
        public override MvxBindingMode DefaultMode
        {
            get { return MvxBindingMode.OneTime; }
        }
    }
}