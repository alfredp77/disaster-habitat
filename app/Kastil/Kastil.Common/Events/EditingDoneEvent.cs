using MvvmCross.Plugins.Messenger;

namespace Kastil.Common.Events
{
    public class EditingDoneEvent : MvxMessage
    {
        public EditAction EditAction { get; }

        public EditingDoneEvent(object sender, EditAction editAction) : base(sender)
        {
            EditAction = editAction;
        }
    }

    public enum EditAction
    {
        None,
        Add,
        Edit,
        Delete        
    }

	
}
