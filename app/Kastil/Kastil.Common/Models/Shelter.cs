
using System.Collections.Generic;

namespace Kastil.Common.Models
{
    public class Shelter : Attributed
    {
        public List<Attribute> ShelterAttributes
        {
            get { return Attributes; }
            set { Attributes = value; }
        }        
    }
}