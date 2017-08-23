using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BB.Measure
{
    [Measurement(1, 1, -2)]
    public struct Force
    {
        internal double _value;

        internal Force(double value)
        {
            _value = value;
        }

        public static Force operator +(Force left, Force right)
        {
            return new Force(left._value + right._value);
        }
        public static Force operator -(Force left, Force right)
        {
            return new Force(left._value - right._value);
        }
        public static Force operator *(Force value, double mul)
        {
            return new Force(value._value * mul);
        }
        public static Force operator *(double mul, Force value)
        {
            return new Force(value._value * mul);
        }
        public static Force operator /(Force value, double div)
        {
            return new Force(value._value / div);
        }
        public static bool operator ==(Force left, Force right)
        {
            return left._value == right._value;
        }
        public static bool operator !=(Force left, Force right)
        {
            return left._value != right._value;
        }
        public static bool operator >(Force left, Force right)
        {
            return left._value > right._value;
        }
        public static bool operator <(Force left, Force right)
        {
            return left._value < right._value;
        }
        public static bool operator >=(Force left, Force right)
        {
            return left._value >= right._value;
        }
        public static bool operator <=(Force left, Force right)
        {
            return left._value <= right._value;
        }

        public override bool Equals(object obj)
        {
            return obj is Force && (Force)obj == this;
        }
        public override int GetHashCode()
        {
            return _value.GetHashCode();
        }

        private const double LBF_INV = 4.4482216282509;
        internal const double LBF = 1 / LBF_INV;

        public static Force FromNewtons(double N)
        {
            return new Force(N);
        }
        public static Force FromPounds(double lbf)
        {
            return new Force(lbf * LBF);
        }

        public double Newtons
        {
            get { return _value; }
        }
        public double Pounds
        {
            get { return _value * LBF_INV; }
        }

        public static Acceleration operator /(Force f, Mass m)
        {
            return new Acceleration(f._value / m._value);
        }
        public static Mass operator/(Force f, Acceleration a)
        {
            return new Mass(f._value / a._value);
        }
        public static Energy operator *(Force f, Length l)
        {
            return new Energy(f._value * l._value);
        }
        public static Energy operator *(Length l, Force f)
        {
            return f * l;
        }
        
    }
}
