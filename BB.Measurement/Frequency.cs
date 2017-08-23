using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BB.Measure
{
    [Measurement(0, 0, -1)]
    public struct Frequency
    {
        internal double _value;

        internal Frequency(double value)
        {
            _value = value;
        }

        public override string ToString()
        {
            return _value + "Hz";
        }

        public static Frequency operator +(Frequency left, Frequency right)
        {
            return new Frequency(left._value + right._value);
        }
        public static Frequency operator -(Frequency left, Frequency right)
        {
            return new Frequency(left._value - right._value);
        }
        public static Frequency operator *(Frequency value, double mul)
        {
            return new Frequency(value._value * mul);
        }
        public static Frequency operator *(double mul, Frequency value)
        {
            return value * mul;
        }
        public static Frequency operator /(Frequency value, double div)
        {
            return new Frequency(value._value / div);
        }
        public static bool operator ==(Frequency left, Frequency right)
        {
            return left._value == right._value;
        }
        public static bool operator !=(Frequency left, Frequency right)
        {
            return left._value != right._value;
        }
        public static bool operator >(Frequency left, Frequency right)
        {
            return left._value > right._value;
        }
        public static bool operator <(Frequency left, Frequency right)
        {
            return left._value < right._value;
        }
        public static bool operator >=(Frequency left, Frequency right)
        {
            return left._value >= right._value;
        }
        public static bool operator <=(Frequency left, Frequency right)
        {
            return left._value <= right._value;
        }

        public override bool Equals(object obj)
        {
            return obj is Frequency && (Frequency)obj == this;
        }
        public override int GetHashCode()
        {
            return _value.GetHashCode();
        }

        public static double operator*(Frequency f, Time t)
        {
            return f._value * t._value;
        }

        public static double operator *(Time t, Frequency f)
        {
            return f * t;
        }
    }
}
