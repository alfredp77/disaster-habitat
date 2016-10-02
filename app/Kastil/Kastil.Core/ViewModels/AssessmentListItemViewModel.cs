using Kastil.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kastil.Core.ViewModels
{
    public class AssessmentListItemViewModel: BaseViewModel
    {
        public AssessmentListItemViewModel(Assessment value)
        {
            Value = value;
        }

        public Assessment Value { get; }

        public string DisasterId => Value.DisasterId;
        public string AssessmentId => Value.Id;

        public string Text => Value.Name;
        public string ShortDescription
        {
            get
            {
                // can do custom info here
                return Value.LocationName;
            }
        }
       
    }
}
