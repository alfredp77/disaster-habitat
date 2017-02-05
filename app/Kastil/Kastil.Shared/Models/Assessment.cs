using System;
using System.Collections.Generic;
using System.Text;

namespace Kastil.Shared.Models
{
    public class Assessment
    {
        public String Location { get; set; }
        public String Name { get; set; }
        public String Id { get; set; }
        public String DisasterId { get; set; }
        public Dictionary<String,String> Data { get; set; }
    }
}
