using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BB.Measure
{
    [Measurement(0, 1, -1)]
    public struct Speed
    {
        private double _value;

        internal const double MPH = Length.MILE / Time.HOUR;
        internal const double KNOT = Length.NAUTICAL_MILE / Time.HOUR;
        internal const double KILOMETRES_PER_HOUR = Unit.KILO / Time.HOUR;
        internal const double FEET_PER_SECOND = Length.FOOT;

        private const double MPH_INV = 1 / MPH;
        private const double KNOT_INV = 1 / KNOT;
        private const double KILOMETRES_PER_HOUR_INV = 1 / KILOMETRES_PER_HOUR;
        internal const double FEET_PER_SECOND_INV = 1 / FEET_PER_SECOND;

        internal Speed(double mps)
        {
            _value = mps;
        }

        public static Speed FromMetresPerSecond(double mps)
        {
            return new Speed(mps);
        }
        public static Speed FromKilometresPerHour(double value)
        {
            return new Speed(value * KILOMETRES_PER_HOUR);
        }
        public static Speed FromMilesPerHour(double mph)
        {
            return new Speed(mph * MPH);
        }
        public static Speed FromKnot(double knot)
        {
            return new Speed(knot * KNOT);
        }

        public double MetresPerSecond
        {
            get { return _value; }
        }
        public double KiloMetresPerHour
        {
            get { return _value * KILOMETRES_PER_HOUR_INV; }
        }
        public double MilesPerHour
        {
            get { return _value * MPH_INV; }
        }
        public double Knots
        {
            get { return _value * KNOT_INV; }
        }

        public static Speed operator +(Speed left, Speed right)
        {
            return new Speed(left._value + right._value);
        }
        public static Speed operator -(Speed left, Speed right)
        {
            return new Speed(left._value - right._value);
        }
        public static Speed operator *(Speed value, double mul)
        {
            return new Speed(value._value * mul);
        }
        public static Speed operator *(double mul, Speed value)
        {
            return value * mul;
        }
        public static Speed operator /(Speed value, double div)
        {
            return new Speed(value._value / div);
        }
        public static bool operator ==(Speed left, Speed right)
        {
            return left._value == right._value;
        }
        public static bool operator !=(Speed left, Speed right)
        {
            return left._value != right._value;
        }
        public static bool operator >(Speed left, Speed right)
        {
            return left._value > right._value;
        }
        public static bool operator <(Speed left, Speed right)
        {
            return left._value < right._value;
        }
        public static bool operator >=(Speed left, Speed right)
        {
            return left._value >= right._value;
        }
        public static bool operator <=(Speed left, Speed right)
        {
            return left._value <= right._value;
        }
        public static Length operator *(Speed speed, Time time)
        {
            return new Length(speed._value * time._value);
        }
        public static Acceleration operator/(Speed speed, Time time)
        {
            return new Acceleration(speed._value / time._value);
        }

        public override bool Equals(object obj)
        {
            return obj is Speed && (Speed)obj == this;
        }
        public override int GetHashCode()
        {
            return _value.GetHashCode();
        }

        public override string ToString()
        {
            return _value + "m/s";
        }
    }
}
