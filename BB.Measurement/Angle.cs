using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BB.Measure
{
    public struct Angle : IEquatable<Angle>, IComparable<Angle>
    {
        internal double _value;

        internal Angle(double value)
        {
            _value = value;
        }

        public static Angle operator +(Angle left, Angle right)
        {
            return new Angle(left._value + right._value);
        }
        public static Angle operator -(Angle left, Angle right)
        {
            return new Angle(left._value - right._value);
        }
        public static Angle operator *(Angle value, double mul)
        {
            return new Angle(value._value * mul);
        }
        public static Angle operator *(double mul, Angle value)
        {
            return new Angle(value._value * mul);
        }
        public static Angle operator /(Angle value, double div)
        {
            return new Angle(value._value / div);
        }
        public static bool operator ==(Angle left, Angle right)
        {
            return left._value == right._value;
        }
        public static bool operator !=(Angle left, Angle right)
        {
            return left._value != right._value;
        }
        public static bool operator >(Angle left, Angle right)
        {
            return left._value > right._value;
        }
        public static bool operator <(Angle left, Angle right)
        {
            return left._value < right._value;
        }
        public static bool operator >=(Angle left, Angle right)
        {
            return left._value >= right._value;
        }
        public static bool operator <=(Angle left, Angle right)
        {
            return left._value <= right._value;
        }

        public override bool Equals(object obj)
        {
            return obj is Angle && (Angle)obj == this;
        }
        public override int GetHashCode()
        {
            return _value.GetHashCode();
        }

        public int CompareTo(Angle other)
        {
            return _value.CompareTo(other._value);
        }

        bool IEquatable<Angle>.Equals(Angle other)
        {
            return this == other;
        }

        internal const double DEGREES = Math.PI / 180;
        private const double GRADIANS = Math.PI / 200;
        private const double DEGREES_INV = 180 / Math.PI;
        private const double GRADIANS_INV = 200 / Math.PI;

        public static double Sin(Angle a)
        {
            return Math.Sin(a._value);
        }
        public static double Cos(Angle a)
        {
            return Math.Cos(a._value);
        }
        public static double Tan(Angle a)
        {
            return Math.Tan(a._value);
        }
        public static Angle Asin(double d)
        {
            return FromRadians(Math.Asin(d));
        }
        public static Angle Acos(double d)
        {
            return FromRadians(Math.Acos(d));
        }
        public static Angle Atan(double d)
        {
            return FromRadians(Math.Atan(d));
        }

        public double Radians
        {
            get { return _value; }
        }
        public double Degrees
        {
            get { return _value * DEGREES_INV; }
        }
        public double Gradians
        {
            get { return _value * GRADIANS_INV; }
        }

        public static Angle FromDegrees(double degrees)
        {
            return new Angle(degrees * DEGREES);
        }
        public static Angle FromRadians(double rad)
        {
            return new Angle(rad);
        }
        public static Angle FromGradians(double grad)
        {
            return new Angle(grad * GRADIANS);
        }

        public override string ToString()
        {
            return _value.ToString();
        }
    }
}
