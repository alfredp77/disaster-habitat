using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MvvmCross.Plugins.Messenger;

namespace Kastil.Core.Events
{

	public class SyncCompletedEvent : MvxMessage
	{
		public bool Successful { get; }
		public SyncCompletedEvent(object sender, bool successful) : base(sender)
        {
			Successful = successful;
		}
	}
}