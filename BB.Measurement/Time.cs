using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BB.Measure
{
    [Measurement(0, 0, 1)]
    public struct Time
    {
        internal double _value;

        internal const double MINUTE = 60;
        internal const double HOUR = 3600;
        internal const double DAY = 86400;
        internal const double WEEK = 604800;
        internal const double YEAR = 31536000;

        private const double MINUTE_INV = 1 / MINUTE;
        private const double HOUR_INV = 1 / HOUR;
        private const double DAY_INV = 1 / DAY;
        private const double YEAR_INV = 1 / YEAR;

        internal Time(double s)
        {
            _value = s;
        }

        public static readonly Time Zero = new Time();

        public static Time FromSeconds(double s)
        {
            return new Time(s);
        }
        public static Time FromMinutes(double min)
        {
            return new Time(min * MINUTE);
        }
        public static Time FromHours(double hr)
        {
            return new Time(hr * HOUR);
        }
        public static Time FromDays(double day)
        {
            return new Time(day * DAY);
        }
        public static Time FromYears(double yr)
        {
            return new Time(yr * YEAR);
        }
        public static Time FromMilliseconds(double ms)
        {
            return new Time(ms * 0.001);
        }

        public double Seconds
        {
            get { return _value; }
        }
        public double Milliseconds
        {
            get { return _value * 1000; }
        }
        public double Minutes
        {
            get { return _value * MINUTE_INV; }
        }
        public double Hours
        {
            get { return _value * HOUR_INV; }
        }
        public double Days
        {
            get { return _value * DAY_INV; }
        }
        public double Years
        {
            get { return _value * YEAR_INV; }
        }

        public static Time operator +(Time left, Time right)
        {
            return new Time(left._value + right._value);
        }
        public static Time operator -(Time left, Time right)
        {
            return new Time(left._value - right._value);
        }
        public static Time operator *(Time value, double mul)
        {
            return new Time(value._value * mul);
        }
        public static Time operator *(double mul, Time value)
        {
            return value * mul;
        }
        public static Time operator /(Time value, double div)
        {
            return new Time(value._value / div);
        }
        public static bool operator ==(Time left, Time right)
        {
            return left._value == right._value;
        }
        public static bool operator !=(Time left, Time right)
        {
            return left._value != right._value;
        }
        public static bool operator >(Time left, Time right)
        {
            return left._value > right._value;
        }
        public static bool operator <(Time left, Time right)
        {
            return left._value < right._value;
        }
        public static bool operator >=(Time left, Time right)
        {
            return left._value >= right._value;
        }
        public static bool operator <=(Time left, Time right)
        {
            return left._value <= right._value;
        }

        public static Frequency operator/(double value, Time t)
        {
            return new Frequency(value / t._value); 
        }

        public static implicit operator Time(TimeSpan timeSpan)
        {
            return new Time(timeSpan.Seconds);
        }
        public static explicit operator TimeSpan(Time time)
        {
            return TimeSpan.FromSeconds(time._value);
        }

        public override bool Equals(object obj)
        {
            return obj is Time && (Time)obj == this;
        }
        public override int GetHashCode()
        {
            return _value.GetHashCode();
        }

        public override string ToString()
        {
            return _value + "s";
        }
    }
}
