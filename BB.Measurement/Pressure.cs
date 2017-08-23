using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BB.Measure
{
    [Measurement(1, -1, -2)]
    public struct Pressure
    {
        internal double _value;

        internal Pressure(double value)
        {
            _value = value;
        }

        public static Pressure operator +(Pressure left, Pressure right)
        {
            return new Pressure(left._value + right._value);
        }
        public static Pressure operator -(Pressure left, Pressure right)
        {
            return new Pressure(left._value - right._value);
        }
        public static Pressure operator *(Pressure value, double mul)
        {
            return new Pressure(value._value * mul);
        }
        public static Pressure operator *(double mul, Pressure value)
        {
            return new Pressure(value._value * mul);
        }
        public static Pressure operator /(Pressure value, double div)
        {
            return new Pressure(value._value / div);
        }
        public static bool operator ==(Pressure left, Pressure right)
        {
            return left._value == right._value;
        }
        public static bool operator !=(Pressure left, Pressure right)
        {
            return left._value != right._value;
        }
        public static bool operator >(Pressure left, Pressure right)
        {
            return left._value > right._value;
        }
        public static bool operator <(Pressure left, Pressure right)
        {
            return left._value < right._value;
        }
        public static bool operator >=(Pressure left, Pressure right)
        {
            return left._value >= right._value;
        }
        public static bool operator <=(Pressure left, Pressure right)
        {
            return left._value <= right._value;
        }

        public override bool Equals(object obj)
        {
            return obj is Pressure && (Pressure)obj == this;
        }
        public override int GetHashCode()
        {
            return _value.GetHashCode();
        }
    }
}
