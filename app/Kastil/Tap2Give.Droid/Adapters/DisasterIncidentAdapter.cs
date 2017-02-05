using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.Support.V7.Widget;
using Android.Views;
using MvvmCross.Binding.Droid.BindingContext;
using MvvmCross.Droid.Support.V7.RecyclerView;
using Tap2Give.Droid.ViewHolders;

namespace Tap2Give.Droid.Adapters
{
    public class DisasterIncidentAdapter : MvxRecyclerAdapter
    {
        public DisasterIncidentAdapter(IMvxAndroidBindingContext bindingContext) : base(bindingContext)
        {
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            var itemBindingContext = new MvxAndroidBindingContext(parent.Context, this.BindingContext.LayoutInflaterHolder);
            var view = this.InflateViewForHolder(parent, viewType, itemBindingContext);

            return new DisasterIncidentViewHolder(view, itemBindingContext);
        }
    }
}