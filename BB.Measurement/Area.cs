using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BB.Measure
{
    [Measurement(0, -2, 0)]
    public struct Area
    {
        internal double _value;

        internal Area(double value)
        {
            _value = value;
        }

        public static Area operator +(Area left, Area right)
        {
            return new Area(left._value + right._value);
        }
        public static Area operator -(Area left, Area right)
        {
            return new Area(left._value - right._value);
        }
        public static Area operator *(Area value, double mul)
        {
            return new Area(value._value * mul);
        }
        public static Area operator *(double mul, Area value)
        {
            return new Area(value._value * mul);
        }
        public static Area operator /(Area value, double div)
        {
            return new Area(value._value / div);
        }
        public static bool operator ==(Area left, Area right)
        {
            return left._value == right._value;
        }
        public static bool operator !=(Area left, Area right)
        {
            return left._value != right._value;
        }
        public static bool operator >(Area left, Area right)
        {
            return left._value > right._value;
        }
        public static bool operator <(Area left, Area right)
        {
            return left._value < right._value;
        }
        public static bool operator >=(Area left, Area right)
        {
            return left._value >= right._value;
        }
        public static bool operator <=(Area left, Area right)
        {
            return left._value <= right._value;
        }

        public override bool Equals(object obj)
        {
            return obj is Area && (Area)obj == this;
        }
        public override int GetHashCode()
        {
            return _value.GetHashCode();
        }

        internal const double SQ_INCH = Length.INCH * Length.INCH;
        internal const double SQ_MILE = Length.MILE * Length.MILE;
        internal const double SQ_FT = Length.FOOT * Length.FOOT;
        internal const double SQ_YD = Length.YARD * Length.YARD;
        internal const double ACRE = 4840 * SQ_YD;
        private const double SQ_INCH_INV = 1 / SQ_INCH;
        private const double SQ_MILE_INV = 1 / SQ_MILE;
        private const double SQ_FT_INV = 1 / SQ_FT;
        private const double SQ_YD_INV = 1 / SQ_YD;
        private const double ACRE_INV = 1 / ACRE;

        public static Area FromSquareMetres(double sqm)
        {
            return new Area(sqm);
        }
        public static Area FromSquareCentimetres(double sqcm)
        {
            return new Area(sqcm * .0001);
        }
        public static Area FromSquareMillimetres(double sqcm)
        {
            return new Area(sqcm * .000001);
        }
        public static Area FromSquareKilometres(double sqkm)
        {
            return new Area(sqkm * 1000000);
        }
        public static Area FromSquareMiles(double value)
        {
            return new Area(value * SQ_MILE);
        }
        public static Area FromSquareInches(double sqin)
        {
            return new Area(sqin * SQ_INCH);
        }
        public static Area FromSquareFeet(double sqft)
        {
            return new Area(sqft * SQ_FT);
        }
        public static Area FromSquareYards(double sqyd)
        {
            return new Area(sqyd * SQ_YD);
        }
        public static Area FromAcres(double acres)
        {
            return new Area(acres * ACRE);
        }

        public double SquareMetres
        {
            get { return _value; }
        }
        public double SquareCentimetres
        {
            get { return _value * 10000; }
        }
        public double SquareMillimetres
        {
            get { return _value * 1000000; }
        }
        public double SquareKilometres
        {
            get { return _value * .000001; }
        }
        public double SquareMiles
        {
            get { return _value * SQ_MILE_INV; }
        }
        public double SquareInches
        {
            get { return _value * SQ_INCH_INV; }
        }
        public double SquareFeet
        {
            get { return _value * SQ_FT_INV; }
        }
        public double Acres
        {
            get { return _value * ACRE_INV; }
        }
        public static Volume operator *(Area a, Length l)
        {
            return new Volume(a._value * l._value);
        }
        public static Volume operator *(Length l, Area a)
        {
            return a * l;
        }
    }
}
