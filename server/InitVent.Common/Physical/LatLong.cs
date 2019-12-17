using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace InitVent.Common.Physical
{
    /// <summary>
    /// The latitude and longitude of a geographic location, as specified in degrees.
    /// </summary>
    public class LatLong : IEquatable<LatLong>
    {
        public double Latitude { get; private set; }
        public double Longitude { get; private set; }

        public bool IsValid()
        {
            return (Latitude >= -90 && Latitude <= 90) &&
                (Longitude >= -180 && Longitude <= 180);
        }

        public LatLong(double latitude, double longitude)
        {
            Latitude = latitude;
            Longitude = longitude;
        }

        public override String ToString()
        {
            return Latitude + ", " + Longitude;
        }

        #region Equality methods

        public override bool Equals(Object other)
        {
            return Equals(other as LatLong);
        }

        public bool Equals(LatLong other)
        {
            return ReferenceEquals(other, null) ? false : Latitude == other.Latitude && Longitude == other.Longitude;
        }

        public override int GetHashCode()
        {
            return Latitude.GetHashCode() ^ Longitude.GetHashCode();
        }

        //public static bool operator ==(LatLong a, LatLong b)
        //{
        //    return ReferenceEquals(a, null) ? ReferenceEquals(b, null) : a.Equals(b);
        //}

        //public static bool operator !=(LatLong a, LatLong b)
        //{
        //    return !(a == b);
        //}

        #endregion

        /// <summary>
        /// Converts the string representation of a latitude/longitude pair to its
        /// double-precision floating-point equivalent.
        /// </summary>
        /// <param name="s">A string containing a latitude/longitude pair to convert.</param>
        /// <returns>A latitude/longitude pair equivalent to that specified in s</returns>
        /// <exception cref="System.ArgumentNullException">s is null.</exception>
        /// <exception cref="System.FormatException">s is not in a valid format.</exception>
        public static LatLong Parse(String s)
        {
            if (s == null)
                throw new ArgumentNullException("s");

            const String decimalPattern = @"[-+]?\d+[.]?\d*";
            var match = Regex.Match(s.Trim(), String.Format(@"(?<lat>{0}),\s*(?<long>{0})", decimalPattern));

            if (!match.Success)
                throw new FormatException("String " + s + " does not encode a latitude/longitude pair.");

            return new LatLong(Double.Parse(match.Groups["lat"].Value), Double.Parse(match.Groups["long"].Value));
        }

        public Distance GetDistance(LatLong other)
        {
            return GetDistance(this, other);
        }

        public static Distance GetDistance(LatLong p, LatLong q)
        {
            return HaversineDistance.GetDistance(p, q);
        }
    }

    /// <summary>
    /// A duplicate but cleaner and more strongly-typed implementation of
    /// <see cref="InitVent.SpoonByte.Domain.Common.HaversineDistance"/>.
    /// </summary>
    /// <remarks>
    /// See http://en.wikipedia.org/wiki/Haversine_formula.
    /// </remarks>
    public static class HaversineDistance
    {
        private static readonly Distance AverageRadiusOfEarth = Distance.Kilometers(6371);

        public static Distance GetDistance(LatLong p, LatLong q)
        {
            if (p == null)
                throw new ArgumentNullException("p");
            if (q == null)
                throw new ArgumentNullException("q");

            double latitudeP = ToRadians(p.Latitude);
            double longitudeP = ToRadians(p.Longitude);
            double latitudeQ = ToRadians(q.Latitude);
            double longitudeQ = ToRadians(q.Longitude);

            double latitudeDifference = latitudeQ - latitudeP;
            double longitudeDifference = longitudeQ - longitudeP;

            double a = Square(Math.Sin(latitudeDifference / 2)) + Math.Cos(latitudeP) * Math.Cos(latitudeQ) * Square(Math.Sin(longitudeDifference / 2));
            double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

            return c * AverageRadiusOfEarth;
        }

        private static double ToRadians(double degrees)
        {
            return (Math.PI * degrees) / 180.0;
        }

        private static double Square(double value)
        {
            return value * value;
        }
    }
}
