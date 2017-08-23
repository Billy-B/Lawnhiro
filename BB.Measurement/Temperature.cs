using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BB.Measure
{
    public struct Temperature
    {
        internal double _value;

        internal const double CELCIUS_OFFSET = 273.15;
        internal const double FAHRENHEIT_OFFSET = 459.67;

        internal const double FAHRENHEIT_RATIO = 1.8;
        internal const double FAHRENHEIT_RATIO_INV = (double)5 / 9;

        public static readonly Temperature AbsoluteZero = new Temperature();

        internal Temperature(double value)
        {
            _value = value;
        }

        public static Temperature operator +(Temperature left, Temperature right)
        {
            return new Temperature(left._value + right._value);
        }
        public static Temperature operator -(Temperature left, Temperature right)
        {
            return new Temperature(left._value - right._value);
        }
        public static Temperature operator *(Temperature value, double mul)
        {
            return new Temperature(value._value * mul);
        }
        public static Temperature operator *(double mul, Temperature value)
        {
            return new Temperature(value._value * mul);
        }
        public static Temperature operator /(Temperature value, double div)
        {
            return new Temperature(value._value / div);
        }
        public static bool operator ==(Temperature left, Temperature right)
        {
            return left._value == right._value;
        }
        public static bool operator !=(Temperature left, Temperature right)
        {
            return left._value != right._value;
        }
        public static bool operator >(Temperature left, Temperature right)
        {
            return left._value > right._value;
        }
        public static bool operator <(Temperature left, Temperature right)
        {
            return left._value < right._value;
        }
        public static bool operator >=(Temperature left, Temperature right)
        {
            return left._value >= right._value;
        }
        public static bool operator <=(Temperature left, Temperature right)
        {
            return left._value <= right._value;
        }

        public override bool Equals(object obj)
        {
            return obj is Temperature && (Temperature)obj == this;
        }
        public override int GetHashCode()
        {
            return _value.GetHashCode();
        }

        public override string ToString()
        {
            return _value + "K";
        }

        public static Temperature FromCelcius(double C)
        {
            double K = C + CELCIUS_OFFSET;
            if (K < 0)
            {
                throw new ArgumentOutOfRangeException("C", "Cannot be less than " + -CELCIUS_OFFSET);
            }
            return new Temperature(K);
        }

        public static Temperature FromKelvin(double K)
        {
            if (K < 0)
            {
                throw new ArgumentOutOfRangeException("K", "Cannot be negative.");
            }
            return new Temperature(K);
        }

        public static Temperature FromFahrenheit(double F)
        {
            double K = (F + FAHRENHEIT_OFFSET) * FAHRENHEIT_RATIO_INV;
            if (K < 0)
            {
                throw new ArgumentOutOfRangeException("F", "Cannot be less than " + -FAHRENHEIT_OFFSET);
            }
            return new Temperature(K);
        }

        public double Kelvin
        {
            get { return _value; }
        }
        public double Celcius
        {
            get { return _value - CELCIUS_OFFSET; }
        }
        public double Fahrenheit
        {
            get { return _value * FAHRENHEIT_RATIO - FAHRENHEIT_OFFSET; }
        }
    }
}
