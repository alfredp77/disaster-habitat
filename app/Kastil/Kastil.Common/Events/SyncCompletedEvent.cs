using MvvmCross.Plugins.Messenger;

namespace Kastil.Common.Events
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
