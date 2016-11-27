using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MvvmCross.Plugins.Messenger;

namespace Kastil.Core.Events
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
