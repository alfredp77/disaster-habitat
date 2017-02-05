using System;
using Kastil.Common.Models;

namespace Kastil.Common.Utils
{
    public static class DistanceCalculator
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="from"><cref="Location">Location</cref> from</param>
        /// <param name="to"><cref="Location">Location</cref> to</param>
        /// <param name="unit">"K" for kms (default), "M" for miles, "N" for nautical miles</param>
        /// <returns></returns>
        public static double MeasureDistance(Location from, Location to, char unit = 'K')
        {
            return MeasureDistance(from.Latitude, from.Longitude, to.Latitude, to.Latitude, unit);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="latFrom"></param>
        /// <param name="lonFrom"></param>
        /// <param name="latTo"></param>
        /// <param name="lonTo"></param>
        /// <param name="unit">"K" for kms (default), "M" for miles, "N" for nautical miles</param>
        /// <returns></returns>
        public static double MeasureDistance(double latFrom, double lonFrom, double latTo, double lonTo, char unit)
        {
            double theta = lonFrom - lonTo;
            double dist = Math.Sin(deg2rad(latFrom)) * Math.Sin(deg2rad(latTo)) +
                          Math.Cos(deg2rad(latFrom)) * Math.Cos(deg2rad(latTo)) *
                          Math.Cos(deg2rad(theta));
            dist = Math.Acos(dist);
            dist = rad2deg(dist);
            dist = dist * 60 * 1.1515;
            if (unit == 'K')
            {
                dist = dist * 1.609344;
            }
            else if (unit == 'N')
            {
                dist = dist * 0.8684;
            }
            return (dist);
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //::  This function converts decimal degrees to radians             :::
        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        private static double deg2rad(double deg)
        {
            return (deg * Math.PI / 180.0);
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //::  This function converts radians to decimal degrees             :::
        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        private static double rad2deg(double rad)
        {
            return (rad / Math.PI * 180.0);
        }
    }
}
