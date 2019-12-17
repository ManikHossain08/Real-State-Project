using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InitVent.Common.Physical
{
    /// <summary>
    /// Represents a physical distance that is unit agnostic.
    /// </summary>
    /// <remarks>
    /// The main purpose of this class is to reduce confusion when passing around
    /// distances using different units (say, miles and kilometers).
    /// </remarks>
    public struct Distance : IComparable, IComparable<Distance>, IEquatable<Distance>
    {
        public static readonly Distance Zero = new Distance() { rawDistanceInKilometers = 0 };

        #region Constants from System.Double

        /// <summary>
        /// Represents the smallest positive Distance value greater than zero. This
        /// field is constant.
        /// </summary>
        public static readonly Distance Epsilon = new Distance() { rawDistanceInKilometers = Double.Epsilon };
        /// <summary>
        /// Represents the largest possible value of a Distance. This field is constant.
        /// </summary>
        public static readonly Distance MaxValue = new Distance() { rawDistanceInKilometers = Double.MaxValue };
        /// <summary>
        /// Represents the smallest possible value of a Distance. This field is constant.
        /// </summary>
        public static readonly Distance MinValue = new Distance() { rawDistanceInKilometers = Double.MinValue };
        /// <summary>
        /// Represents a value that is not a number (NaN). This field is constant.
        /// </summary>
        public static readonly Distance NaN = new Distance() { rawDistanceInKilometers = Double.NaN };
        /// <summary>
        /// Represents negative infinity. This field is constant.
        /// </summary>
        public static readonly Distance NegativeInfinity = new Distance() { rawDistanceInKilometers = Double.NegativeInfinity };
        /// <summary>
        /// Represents positive infinity. This field is constant.
        /// </summary>
        public static readonly Distance PositiveInfinity = new Distance() { rawDistanceInKilometers = Double.PositiveInfinity };

        #endregion

        private const double MilesPerKilometer = 0.621371192;

        /// <summary>
        /// The actual stored distance.
        /// </summary>
        /// <remarks>
        /// The kilometer SI unit is actually more exact than the mile (which both lacks
        /// precision in its definition as well as varies from country to country; see
        /// http://en.wikipedia.org/wiki/Mile), and hence makes it a more natural choice
        /// for the internal representation of length.
        /// </remarks>
        private double rawDistanceInKilometers;

        public double DistanceInKilometers
        {
            get { return rawDistanceInKilometers; }
            private set { rawDistanceInKilometers = value; }
        }
        public double DistanceInMiles
        {
            get { return rawDistanceInKilometers * MilesPerKilometer; }
            private set { rawDistanceInKilometers = value / MilesPerKilometer; }
        }

        public static Distance Kilometers(double kilometers)
        {
            return new Distance() { rawDistanceInKilometers = kilometers };
        }

        public static Distance Miles(double miles)
        {
            return new Distance() { DistanceInMiles = miles };
        }

        public override string ToString()
        {
            return DistanceInMiles + " miles";
        }

        #region Equality methods

        public override bool Equals(Object other)
        {
            return (other is Distance) ? Equals((Distance)other) : false;
        }

        public bool Equals(Distance other)
        {
            return rawDistanceInKilometers == other.rawDistanceInKilometers;
        }

        public override int GetHashCode()
        {
            return rawDistanceInKilometers.GetHashCode();
        }

        public static bool operator ==(Distance a, Distance b)
        {
            return a.Equals(b);
        }

        public static bool operator !=(Distance a, Distance b)
        {
            return !(a == b);
        }

        #endregion

        #region Comparison methods

        int IComparable.CompareTo(Object other)
        {
            if (!(other is Distance))
                throw new ArgumentException("Cannot compare distance to " + other, "other");

            return CompareTo((Distance)other);
        }

        public int CompareTo(Distance other)
        {
            return Math.Sign(rawDistanceInKilometers - other.rawDistanceInKilometers);
        }

        public static bool operator <(Distance a, Distance b)
        {
            return a.CompareTo(b) < 0;
        }

        public static bool operator >(Distance a, Distance b)
        {
            return b < a;
        }

        public static bool operator >=(Distance a, Distance b)
        {
            return !(a < b);
        }

        public static bool operator <=(Distance a, Distance b)
        {
            return !(b < a);
        }

        #endregion

        #region Arithmetic methods

        public static Distance operator +(Distance d)
        {
            return d;
        }

        public static Distance operator -(Distance d)
        {
            return new Distance() { rawDistanceInKilometers = -d.rawDistanceInKilometers };
        }

        public static Distance operator +(Distance a, Distance b)
        {
            return new Distance() { rawDistanceInKilometers = a.rawDistanceInKilometers + b.rawDistanceInKilometers };
        }

        public static Distance operator -(Distance a, Distance b)
        {
            return new Distance() { rawDistanceInKilometers = a.rawDistanceInKilometers - b.rawDistanceInKilometers };
        }

        public static Distance operator *(Distance d, double s)
        {
            return new Distance() { rawDistanceInKilometers = d.rawDistanceInKilometers * s };
        }

        public static Distance operator *(double s, Distance d)
        {
            return d * s;
        }

        public static Distance operator /(Distance d, double s)
        {
            return new Distance() { rawDistanceInKilometers = d.rawDistanceInKilometers / s };
        }

        public static double operator /(Distance a, Distance b)
        {
            return a.rawDistanceInKilometers / b.rawDistanceInKilometers;
        }

        public static double operator %(Distance a, Distance b)
        {
            return a.rawDistanceInKilometers % b.rawDistanceInKilometers;
        }

        #endregion

        #region Special value methods

        public static bool IsInfinity(Distance d)
        {
            return Double.IsInfinity(d.rawDistanceInKilometers);
        }

        public static bool IsNaN(Distance d)
        {
            return Double.IsNaN(d.rawDistanceInKilometers);
        }

        public static bool IsNegativeInfinity(Distance d)
        {
            return Double.IsNegativeInfinity(d.rawDistanceInKilometers);
        }

        public static bool IsPositiveInfinity(Distance d)
        {
            return Double.IsPositiveInfinity(d.rawDistanceInKilometers);
        }

        #endregion
    }
}
