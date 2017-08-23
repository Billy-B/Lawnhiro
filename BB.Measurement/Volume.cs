using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BB.Measure
{
    [Measurement(0, 3, 0)]
    public struct Volume
    {
        internal double _value;

        internal Volume(double value)
        {
            _value = value;
        }

        public static Volume operator +(Volume left, Volume right)
        {
            return new Volume(left._value + right._value);
        }
        public static Volume operator -(Volume left, Volume right)
        {
            return new Volume(left._value - right._value);
        }
        public static Volume operator *(Volume value, double mul)
        {
            return new Volume(value._value * mul);
        }
        public static Volume operator *(double mul, Volume value)
        {
            return new Volume(value._value * mul);
        }
        public static Volume operator /(Volume value, double div)
        {
            return new Volume(value._value / div);
        }
        public static bool operator ==(Volume left, Volume right)
        {
            return left._value == right._value;
        }
        public static bool operator !=(Volume left, Volume right)
        {
            return left._value != right._value;
        }
        public static bool operator >(Volume left, Volume right)
        {
            return left._value > right._value;
        }
        public static bool operator <(Volume left, Volume right)
        {
            return left._value < right._value;
        }
        public static bool operator >=(Volume left, Volume right)
        {
            return left._value >= right._value;
        }
        public static bool operator <=(Volume left, Volume right)
        {
            return left._value <= right._value;
        }

        public override bool Equals(object obj)
        {
            return obj is Volume && (Volume)obj == this;
        }
        public override int GetHashCode()
        {
            return _value.GetHashCode();
        }

        public override string ToString()
        {
            return _value + "m^3";
        }

        internal const double LITRE = .001;
        private const double MILLILITRE = 0.000001;
        private const double CUBIC_INCH = Length.INCH * Length.INCH * Length.INCH;
        private const double CUBIC_FOOT = Length.FOOT * Length.FOOT * Length.FOOT;
        private const double CUBIC_YARD = Length.YARD * Length.YARD * Length.YARD;
        private const double CUBIC_MILE = Length.MILE * Length.MILE * Length.MILE;
        private const double FLUID_OUNCE = MILLILITRE * 29.5735295625;
        private const double CUP = FLUID_OUNCE * 8;
        private const double PINT = FLUID_OUNCE * 16;
        private const double QUART = FLUID_OUNCE * 32;
        private const double GALLON = FLUID_OUNCE * 128;
        private const double TABLESPOON = FLUID_OUNCE * .5;
        private const double TEASPOON = FLUID_OUNCE / 6;

        private const double MILLILITRE_INV = 1000000;
        private const double CUBIC_INCH_INV = 1 / CUBIC_INCH;
        private const double CUBIC_FEET_INV = 1 / CUBIC_FOOT;
        private const double CUBIC_YARD_INV = 1 / CUBIC_YARD;
        private const double CUBIC_MILE_INV = 1 / CUBIC_MILE;
        private const double FLUID_OUNCE_INV = 1 / FLUID_OUNCE;
        private const double CUP_INV = 1 / CUP;
        private const double PINT_INV = 1 / PINT;
        private const double QUART_INV = 1 / QUART;
        private const double GALLON_INV = 1 / GALLON;
        private const double TABLESPOON_INV = 1 / TABLESPOON;
        private const double TEASPOON_INV = 1 / TEASPOON;

        public static Volume FromCubicMetres(double value)
        {
            return new Volume(value);
        }
        public static Volume FromLitres(double L)
        {
            return new Volume(L * LITRE);
        }
        public static Volume FromCubicCentimetres(double value)
        {
            return FromMillilitres(value);
        }
        public static Volume FromMillilitres(double mL)
        {
            return new Volume(mL * MILLILITRE);
        }
        public static Volume FromCubicMiles(double value)
        {
            return new Volume(value * CUBIC_MILE);
        }
        public static Volume FromFluidOunces(double fl_oz)
        {
            return new Volume(fl_oz * FLUID_OUNCE);
        }
        public static Volume FromCups(double cups)
        {
            return new Volume(cups * CUP);
        }
        public static Volume FromPints(double pt)
        {
            return new Volume(pt * PINT);
        }
        public static Volume FromQuarts(double qt)
        {
            return new Volume(qt * QUART);
        }
        public static Volume FromGallons(double gal)
        {
            return new Volume(gal * GALLON);
        }
        public static Volume FromTablespoons(double tbsp)
        {
            return new Volume(tbsp * TABLESPOON);
        }
        public static Volume FromTeaspoons(double tsp)
        {
            return new Volume(tsp * TEASPOON);
        }

        public double CubicMetres
        {
            get { return _value; }
        }
        public double Liters
        {
            get { return _value * 1000; }
        }
        public double CubicCentimetres
        {
            get { return Millilitres; }
        }
        public double Millilitres
        {
            get { return _value * MILLILITRE_INV; }
        }
        public double CubicMiles
        {
            get { return _value * CUBIC_MILE_INV; }
        }
        public double FluidOunces
        {
            get { return _value * FLUID_OUNCE_INV; }
        }
        public double Cups
        {
            get { return _value * CUP_INV; }
        }
        public double Pints
        {
            get { return _value * PINT_INV; }
        }
        public double Quarts
        {
            get { return _value * QUART_INV; }
        }
        public double Gallons
        {
            get { return _value * GALLON_INV; }
        }
        public double Tablespoons
        {
            get { return _value * TABLESPOON_INV; }
        }
        public double Teaspoons
        {
            get { return _value * TEASPOON_INV; }
        }

        public static Area operator /(Volume v, Length l)
        {
            return new Area(v._value / l._value);
        }
        public static Length operator /(Volume v, Area l)
        {
            return new Length(v._value / l._value);
        }
    }
}
