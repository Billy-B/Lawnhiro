using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BB.Measure
{
    [Measurement(0, 1, 0)]
    public struct Length : IComparable<Length>, IFormattable
    {
        internal double _value;

        internal Length(double value)
        {
            _value = value;
        }

        public static readonly Length Zero = new Length();

        public static Length operator +(Length left, Length right)
        {
            return new Length(left._value + right._value);
        }
        public static Length operator -(Length left, Length right)
        {
            return new Length(left._value - right._value);
        }
        public static Length operator *(Length value, double scalar)
        {
            return new Length(value._value * scalar);
        }
        public static Length operator *(double scalar, Length value)
        {
            return value * scalar;
        }
        public static Length operator /(Length dividend, double divisor)
        {
            return new Length(dividend._value / divisor);
        }
        public static double operator /(Length dividend, Length divisor)
        {
            return dividend._value / divisor._value;
        }
        public static bool operator ==(Length left, Length right)
        {
            return left._value == right._value;
        }
        public static bool operator !=(Length left, Length right)
        {
            return left._value != right._value;
        }
        public static bool operator >(Length left, Length right)
        {
            return left._value > right._value;
        }
        public static bool operator <(Length left, Length right)
        {
            return left._value < right._value;
        }
        public static bool operator >=(Length left, Length right)
        {
            return left._value >= right._value;
        }
        public static bool operator <=(Length left, Length right)
        {
            return left._value <= right._value;
        }
        public override bool Equals(object obj)
        {
            return obj is Length && (Length)obj == this;
        }
        public override int GetHashCode()
        {
            return _value.GetHashCode();
        }
        public int CompareTo(Length other)
        {
            return _value.CompareTo(other._value);
        }
        public string ToString(string format, IFormatProvider formatProvider)
        {
            return Measurement.Format(this, format, formatProvider);
        }
        public override string ToString()
        {
            return ToString(null, null);
        }
        public string ToString(string format)
        {
            return ToString(format, null);
        }
        public string ToString(IFormatProvider formatProvider)
        {
            return ToString(null, formatProvider);
        }

        internal const double INCH = 0.0254;
        internal const double FOOT = 0.3048;
        internal const double YARD = 0.9144;
        internal const double MILE = 1609.344;
        internal const double NAUTICAL_MILE = 1852;
        private const double INCH_INV = 1 / INCH;
        private const double FOOT_INV = 1 / FOOT;
        private const double YARD_INV = 1 / YARD;
        private const double MILE_INV = 1 / MILE;
        private const double NAUTICAL_MILE_INV = 1 / NAUTICAL_MILE;

        /*public static Length DistanceBetweenLatLng(double lat1, double lng1, double lat2, double lng2)
        {
            const double RADIUS_EARTH = 6371; // Radius of the earth in km
            double dLat = degreeToRadian(lat2 - lat1);
            double dLon = degreeToRadian(lng2 - lng1);
            double a =
              Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
              Math.Cos(degreeToRadian(lat1)) * Math.Cos(degreeToRadian(lat2)) *
              Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
            double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            return FromKilometres(RADIUS_EARTH * c); // Distance in km
        }*/

        private static double degreeToRadian(double degree)
        {
            return degree * (Math.PI / 180);
        }

        public double Inches
        {
            get { return _value * INCH_INV; }
        }

        public double Metres
        {
            get { return _value; }
        }

        public double Miles
        {
            get { return _value * MILE_INV; }
        }

        public double Feet
        {
            get { return _value * FOOT_INV; }
        }

        public double Yards
        {
            get { return _value * YARD_INV; }
        }

        public static Length FromMetres(double m)
        {
            return new Length(m);
        }
        public static Length FromMiles(double miles)
        {
            return new Length(miles * MILE);
        }
        public static Length FromInches(double inch)
        {
            return new Length(inch * INCH);
        }
        public static Length FromFeet(double ft)
        {
            return new Length(ft * FOOT);
        }
        public static Length FromYards(double yd)
        {
            return new Length(yd * YARD);
        }

        

        public static Speed operator /(Length len, Time time)
        {
            return new Speed(len._value / time._value);
        }

        public static Area operator *(Length left, Length right)
        {
            return new Area(left._value * right._value);
        }
    }
}
