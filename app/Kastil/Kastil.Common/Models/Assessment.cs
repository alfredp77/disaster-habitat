using System.Collections.Generic;
using Newtonsoft.Json;

namespace Kastil.Common.Models
{
    public class Assessment : Attributed
    {
        public List<Attribute> AssessmentAttributes
        {
            get { return Attributes; }
            set { Attributes = value; }
        }

		[JsonProperty(BACKENDLESSCLASSNAME_ATTRIBUTE)]
		public override string ClassName => BACKENDLESSCLASSNAME;

        public const string BACKENDLESSCLASSNAME = "Assesment";
    }
}