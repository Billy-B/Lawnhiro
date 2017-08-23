using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BB.Measure
{
    [Measurement(0, 1, -2)]
    public struct Acceleration
    {
        internal double _value;

        internal Acceleration(double value)
        {
            _value = value;
        }
        public static Acceleration operator +(Acceleration left, Acceleration right)
        {
            return new Acceleration(left._value + right._value);
        }
        public static Acceleration operator -(Acceleration left, Acceleration right)
        {
            return new Acceleration(left._value - right._value);
        }
        public static Acceleration operator *(Acceleration value, double mul)
        {
            return new Acceleration(value._value * mul);
        }
        public static Acceleration operator *(double mul, Acceleration value)
        {
            return value * mul;
        }
        public static Acceleration operator /(Acceleration value, double div)
        {
            return new Acceleration(value._value / div);
        }
        public static bool operator ==(Acceleration left, Acceleration right)
        {
            return left._value == right._value;
        }
        public static bool operator !=(Acceleration left, Acceleration right)
        {
            return left._value != right._value;
        }
        public static bool operator >(Acceleration left, Acceleration right)
        {
            return left._value > right._value;
        }
        public static bool operator <(Acceleration left, Acceleration right)
        {
            return left._value < right._value;
        }
        public static bool operator >=(Acceleration left, Acceleration right)
        {
            return left._value >= right._value;
        }
        public static bool operator <=(Acceleration left, Acceleration right)
        {
            return left._value <= right._value;
        }

        public override bool Equals(object obj)
        {
            return obj is Acceleration && (Acceleration)obj == this;
        }
        public override int GetHashCode()
        {
            return _value.GetHashCode();
        }

        public static Speed operator *(Acceleration a, Time t)
        {
            return new Speed(a._value * t._value);
        }

        public static Speed operator *(Time t, Acceleration a)
        {
            return a * t;
        }

        

        public override string ToString()
        {
            return _value + "m/s^2";
        }
    }
}
