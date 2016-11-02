namespace Kastil.Common.Models
{
    public class Location
    {
        

        public string type { get; set; }
        public double[] coordinates { get; set; }

        // ReSharper disable once MemberCanBePrivate.Global
        // This is to allow the class to be serialized by yaml.
        public Location() : this(0, 0)
        {

        }

        public Location(double lat, double lon)
        {
            type = "Point";
            ZoomLevel = 15;
            coordinates = new[] { lon, lat };
        }

        public double Longitude
        {
            get { return coordinates[0]; }
            set { coordinates[0] = value; }
        }

        public double Latitude
        {
            get { return coordinates[1]; }
            set { coordinates[1] = value; }
        }

        public string Name
        {
            get;
            set;
        }

        public string Country { get; set; }

        public int ZoomLevel { get; set; }

        public override string ToString()
        {
            return string.Format("Lat: {0}, Lng: {1}, Country: {2}", Latitude, Longitude, Country);
        }
    }
}