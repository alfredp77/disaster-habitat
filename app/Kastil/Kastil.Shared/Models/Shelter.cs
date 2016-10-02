using Kastil.Core.Utils;
using System;

namespace Kastil.Shared.Models
{
    public class Shelter : Item
    {
        public string AssessmentId { get; set; }
        public Location Location { get; set; }
        public DateTime VerifiedOn { get; set; }

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