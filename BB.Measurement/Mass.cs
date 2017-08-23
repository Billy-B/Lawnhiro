using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BB.Measure
{
    [Measurement(1, 0, 0)]
    public struct Mass
    {
        internal double _value;

        internal const double POUND = 0.45359237;
        private const double POUND_INV = 1 / POUND;

        internal Mass(double kg)
        {
            _value = kg;
        }

        public double Kilograms
        {
            get { return _value; }
        }
        public double Grams
        {
            get { return _value * 1000; }
        }
        public double Tonnes
        {
            get { return _value * 0.001; }
        }

        public double Pounds
        {
            get { return _value * POUND_INV; }
        }

        public static Mass FromKilograms(double kg)
        {
            return new Mass(kg);
        }
        public static Mass FromGrams(double g)
        {
            return new Mass(g * .001);
        }
        public static Mass FromTonnes(double t)
        {
            return new Mass(t * 1000);
        }
        public static Mass FromPounds(double lb)
        {
            return new Mass(lb * POUND);
        }

        public static Mass operator +(Mass left, Mass right)
        {
            return new Mass(left._value + right._value);
        }
        public static Mass operator -(Mass left, Mass right)
        {
            return new Mass(left._value - right._value);
        }
        public static Mass operator *(Mass value, double mul)
        {
            return new Mass(value._value * mul);
        }
        public static Mass operator *(double mul, Mass value)
        {
            return value * mul;
        }
        public static Mass operator /(Mass value, double div)
        {
            return new Mass(value._value / div);
        }
        public static bool operator ==(Mass left, Mass right)
        {
            return left._value == right._value;
        }
        public static bool operator !=(Mass left, Mass right)
        {
            return left._value != right._value;
        }
        public static bool operator >(Mass left, Mass right)
        {
            return left._value > right._value;
        }
        public static bool operator <(Mass left, Mass right)
        {
            return left._value < right._value;
        }
        public static bool operator >=(Mass left, Mass right)
        {
            return left._value >= right._value;
        }
        public static bool operator <=(Mass left, Mass right)
        {
            return left._value <= right._value;
        }

        public override bool Equals(object obj)
        {
            return obj is Mass && (Mass)obj == this;
        }
        public override int GetHashCode()
        {
            return _value.GetHashCode();
        }

        public override string ToString()
        {
            return _value + "kg";
        }

        public static Force operator *(Mass m, Acceleration a)
        {
            return new Force(m._value * a._value);
        }

        public static Force operator *(Acceleration a, Mass m)
        {
            return m * a;
        }
    }
}
