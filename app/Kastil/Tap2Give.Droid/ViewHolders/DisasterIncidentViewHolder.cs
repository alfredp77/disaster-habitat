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
using MvvmCross.Binding.BindingContext;
using MvvmCross.Binding.Droid.BindingContext;
using MvvmCross.Droid.Support.V7.AppCompat;
using MvvmCross.Droid.Support.V7.RecyclerView;
using Tap2Give.Core.ViewModels;

namespace Tap2Give.Droid.ViewHolders
{
    public class DisasterIncidentViewHolder : MvxRecyclerViewHolder
    {
        private readonly TextView textView;

        public DisasterIncidentViewHolder(View itemView, IMvxAndroidBindingContext context) : base(itemView, context)
        {
            //TODO: Fix this
            this.textView = itemView.FindViewById<TextView>(Android.Resource.Id.Text1);

            this.DelayBind(() =>
            {
                var set = this.CreateBindingSet<DisasterIncidentViewHolder, DisasterListItemViewModel>();
                set.Bind(this.textView).To(x => x.Name);

                set.Apply();
            });
        }
    }
}