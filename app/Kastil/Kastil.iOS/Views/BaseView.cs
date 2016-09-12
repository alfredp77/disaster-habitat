using System;
using System.Collections.Generic;
using System.Text;
using Kastil.Core.ViewModels;
using MvvmCross.iOS.Views;

namespace Kastil.iOS.Views
{
    public abstract class BaseView<T> : MvxViewController<T> where T : BaseViewModel
    {
    }
}
