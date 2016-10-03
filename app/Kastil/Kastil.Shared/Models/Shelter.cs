using Kastil.Core.Utils;
using System;
using Newtonsoft.Json;

namespace Kastil.Shared.Models
{
    public class Shelter : Item
    {
        public string AssessmentId { get; set; }
        public Location Location { get; set; }

		private long VerifiedOn { get; set; }
		[JsonIgnore]
		public DateTimeOffset? DateVerifiedOn {
			get { return VerifiedOn.AsDateTimeOffset(); }
			set { VerifiedOn = value.AsUtcTicks(); }
		}

        public double DistanceFromLocation (Location location)
        {
            return DistanceCalculator.MeasureDistance(location, Location);
        }

        public new string LocationName
        {
            get { return Location != null ? Location.Name + ", " + Location.Country : LocationName; }
            set { LocationName = value; }
        }
    }
}