using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BB.Measure
{
    [Measurement(1, 2, -2)]
    public struct Energy
    {
        internal double _value;

        internal Energy(double value)
        {
            _value = value;
        }

        public static Energy operator +(Energy left, Energy right)
        {
            return new Energy(left._value + right._value);
        }
        public static Energy operator -(Energy left, Energy right)
        {
            return new Energy(left._value - right._value);
        }
        public static Energy operator *(Energy value, double mul)
        {
            return new Energy(value._value * mul);
        }
        public static Energy operator *(double mul, Energy value)
        {
            return new Energy(value._value * mul);
        }
        public static Energy operator /(Energy value, double div)
        {
            return new Energy(value._value / div);
        }
        public static bool operator ==(Energy left, Energy right)
        {
            return left._value == right._value;
        }
        public static bool operator !=(Energy left, Energy right)
        {
            return left._value != right._value;
        }
        public static bool operator >(Energy left, Energy right)
        {
            return left._value > right._value;
        }
        public static bool operator <(Energy left, Energy right)
        {
            return left._value < right._value;
        }
        public static bool operator >=(Energy left, Energy right)
        {
            return left._value >= right._value;
        }
        public static bool operator <=(Energy left, Energy right)
        {
            return left._value <= right._value;
        }

        public override bool Equals(object obj)
        {
            return obj is Energy && (Energy)obj == this;
        }
        public override int GetHashCode()
        {
            return _value.GetHashCode();
        }

        public override string ToString()
        {
            return _value + "J";
        }

        public static Energy FromJoules(double J)
        {
            return new Energy(J);
        }

        public double Joules
        {
            get { return _value; }
        }

        public static Length operator /(Energy e, Force f)
        {
            return new Length(e._value / f._value);
        }

        public static Force operator /(Energy e, Length l)
        {
            return new Force(e._value / l._value);
        }
    }
}
