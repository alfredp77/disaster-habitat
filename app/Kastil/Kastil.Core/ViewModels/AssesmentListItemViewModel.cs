using Kastil.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kastil.Core.ViewModels
{
    public class AssesmentListItemViewModel: BaseViewModel
    {
        public AssesmentListItemViewModel(Assesment value)
        {
            Value = value;
        }

        public Assesment Value { get; }

        public string DisasterId => Value.DisasterId;
        public string AssesmentId => Value.Id;

        public string Text => Value.Name;
        public string ShortDescription
        {
            get
            {
                // can do custom info here
                return Value.Location;
            }
        }
       
    }
}
