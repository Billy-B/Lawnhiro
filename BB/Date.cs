using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace BB
{
    /*public struct Date : IComparable, IFormattable, IConvertible, ISerializable, IComparable<Date>, IEquatable<Date>
    {
        // Number of days in a non-leap year
        private const int _daysPerYear = 365;
        // Number of days in 4 years
        private const int _daysPer4Years = _daysPerYear * 4 + 1;       // 1461
        // Number of days in 100 years
        private const int _daysPer100Years = _daysPer4Years * 25 - 1;  // 36524
        // Number of days in 400 years
        private const int _daysPer400Years = _daysPer100Years * 4 + 1; // 146097

        // Number of days from 1/1/0001 to 12/31/1600
        private const int _daysTo1601 = _daysPer400Years * 4;          // 584388
        // Number of days from 1/1/0001 to 12/30/1899
        private const int _daysTo1899 = _daysPer400Years * 4 + _daysPer100Years * 3 - 367;
        // Number of days from 1/1/0001 to 12/31/1969
        internal const int _daysTo1970 = _daysPer400Years * 4 + _daysPer100Years * 3 + DaysPer4Years * 17 + DaysPerYear; // 719,162
        // Number of days from 1/1/0001 to 12/31/9999
        private const int _daysTo10000 = _daysPer400Years * 25 - 366;  // 3652059

        private const int _maxDays = _daysTo10000 - 1;

        private static readonly int[] _daysPerMonth365 =
            { 0, 31, 59, 90, 120, 151, 181, 212, 243, 273, 304, 334, 365 };
        private static readonly int[] _daysPerMonth366 =
            { 0, 31, 60, 91, 121, 152, 182, 213, 244, 274, 305, 335, 366 };

        private int _days;

        private const string _daysField = "days";

        public Date(int year, int month, int day)
        {
            _days = dateToDays(year, month, day);
        }

        private Date(DateTime dateTime)
        {

        }

        public Date(int year, int month, int day, Calendar calender)
            : this(calender.ToDateTime(year, month, day, 0, 0, 0, 0)) { }

        private static int dateToDays(int year, int month, int day)
        {
            if (year < 1 || year > 9999)
            {
                throw new ArgumentOutOfRangeException("year", "Must be between 1 and 9999");
            }
            if (month < 1 || month > 12)
            {
                throw new ArgumentOutOfRangeException("month", "Must be between 1 and 12.");
            }
            int[] daysPerMonthLookup = DateTime.IsLeapYear(year) ? _daysPerMonth366 : _daysPerMonth365;
            int daysPerMonth = daysPerMonthLookup[month];
            if (day < 1 || day > daysPerMonthLookup[month] - daysPerMonthLookup[month - 1])
            {
                throw new ArgumentOutOfRangeException("days", "Must be between 1 and " + daysPerMonth + ".");
            }
            int y = year - 1;
            return y * 365 + y / 4 - y / 100 + y / 400 + daysPerMonthLookup[month - 1] + day - 1;
        }

        private Date(int days)
        {
            _days = days;
        }

        private Date(SerializationInfo info, StreamingContext context)
        {
            if (info == null)
            {
                throw new ArgumentNullException("info");
            }

            bool foundDays = false;
            int serializedDays = 0;

            SerializationInfoEnumerator enumerator = info.GetEnumerator();
            while (enumerator.MoveNext())
            {
                switch (enumerator.Name)
                {
                    case _daysField:
                        serializedDays = Convert.ToInt32(enumerator.Value, CultureInfo.InvariantCulture);
                        foundDays = true;
                        break;
                }
            }
            if (foundDays)
            {
                _days = serializedDays;
            }
            else
            {
                throw new SerializationException("Missing days data");
            }
        }

        public Date AddDays(int days)
        {
            int newDays = _days + days;
            if (newDays > _maxDays)
            {
                throw new ArgumentException("Resultant date would exceed Date.MaxValue.");
            }
            return new Date(newDays);
        }

        public static implicit operator DateTime(Date value)
        {

        }

        public static explicit operator Date(DateTime value)
        {
            return new Date(value);
        }
    }*/
}
