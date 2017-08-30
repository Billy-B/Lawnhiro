using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BB.Measure
{
    public abstract class Unit
    {
        const char SUPERSCRIPT_MINUS = '⁻';

        //public abstract Unit GetBaseSIRepresentation();

        internal abstract double SIConversion { get; }

        internal abstract string Abbreviation { get; }

        private static Dictionary<string, Unit> _stringMapper = new Dictionary<string, Unit>();

        internal Unit Inverse()
        {
            Dictionary<Unit, int> construction = Construction;
            Dictionary<Unit, int> newConstruction = new Dictionary<Unit, int>();
            foreach (var kvp in construction)
            {
                newConstruction.Add(kvp.Key, -kvp.Value);
            }
            return Build(newConstruction, -Dimensions);
        }

        

        internal virtual Dictionary<Unit, int> Construction
        {
            get
            {
                return new Dictionary<Unit, int> { { this, 1 } };
            }
        }

        public abstract MeasurementDimensions Dimensions { get; }

        internal Unit() { }

        public static Unit FromString(string unit)
        {
            Unit ret;
            if (!_stringMapper.TryGetValue(unit, out ret))
            {
                throw new ArgumentException("\"" + unit + "\" is not a valid unit.");
            }
            throw new NotImplementedException();
        }

        public static Unit DefineUnit(string name, Unit equivalentUnit)
        {
            throw new NotImplementedException();
        }

        public static Unit DefineUnit<TMeasurement>(string name, double siConversion)
        {
            throw new NotImplementedException();
        }

        public static Unit DefineUnit(string name, MeasurementDimensions dimensions, double siConversion)
        {
            throw new NotImplementedException();
        }

        internal static Unit Build(Dictionary<Unit, int> construction, MeasurementDimensions dimensions)
        {
            switch (construction.Count)
            {
                case 0:
                    return ONE;
                case 1:
                    var kvp = construction.Single();
                    if (kvp.Value == 1)
                    {
                        return kvp.Key;
                    }
                    break;
            }
            return new ConstructedUnit(construction, dimensions);
        }

        public static readonly Unit Metre = SIBaseUnit("metre", "m", MeasurementDimensions.Length);
        public static readonly Unit Second = SIBaseUnit("second", "s", MeasurementDimensions.Time);
        public static readonly Unit Kilogram = SIBaseUnit("kilogram", "kg", MeasurementDimensions.Mass);
        public static readonly Unit Kelvin = SIBaseUnit("kelvin", "K", MeasurementDimensions.Temperature);
        public static readonly Unit Ampere = SIBaseUnit("ampere", "A", MeasurementDimensions.Current);
        public static readonly Unit Mole = SIBaseUnit("mole", "mol", MeasurementDimensions.SubstanceAmount);
        public static readonly Unit Candela = SIBaseUnit("candela", "cd", MeasurementDimensions.LuminousIntensity);

        internal static readonly Unit ONE = new BaseUnit("one", "one", MeasurementDimensions.Zero, 1);

        internal static Unit SIBaseUnit(string name, string abbreviation, MeasurementDimensions dimension)
        {
            return new BaseUnit(name, abbreviation, dimension, 1);
        }

        internal class BaseUnit : Unit
        {
            internal string _name;
            internal string _abbreviation;
            internal double _siConversion;
            internal MeasurementDimensions _dimensions;

            internal BaseUnit(string name, string abbreviation, MeasurementDimensions dimensions, double siConversion)
            {
                _name = name;
                _abbreviation = abbreviation;
                _dimensions = dimensions;
                _siConversion = siConversion;
            }

            public override MeasurementDimensions Dimensions
            {
                get { return _dimensions; }
            }

            internal override double SIConversion
            {
                get { return _siConversion; }
            }

            internal override string Abbreviation
            {
                get { return _abbreviation; }
            }

            public override string ToString()
            {
                return _name;
            }
        }

        /*internal class SIBaseUnit : Unit
        {
            internal readonly MeasurementDimensions _dimensions;
            internal readonly int _dimRad;
            internal readonly int _dimSr;

            public override Unit GetBaseSIRepresentation()
            {
                return this;
            }

            public SIBaseUnit(MeasurementDimensions dimensions, int dimRad = 0, int dimSr = 0)
            {
                _dimensions = dimensions;
                _dimRad = dimRad;
                _dimSr = dimSr;
            }

            public override MeasurementDimensions Dimensions
            {
                get { return _dimensions; }
            }

            internal override double SIConversion
            {
                get { return 1; }
            }

            internal override Unit Inverse()
            {
                return new SIBaseUnit(-_dimensions, -_dimRad, -_dimSr);
            }

            /*internal override Unit Multiply(Unit value)
            {
                return value.MultiplySIBase(this);
            }

            internal override Unit MultiplyConstructed(ConstructedUnit value)
            {
                Dictionary<Unit, int> construction = new Dictionary<Unit, int>(value._construction);

                if (construction.ContainsKey(this))
                {
                    int newExp = construction[this] + 1;
                    if (newExp == 0)
                    {
                        construction.Remove(this);
                    }
                    else
                    {
                        construction[this] = newExp;
                    }
                }
                else
                {
                    construction.Add(this, 1);
                }
                if (construction.Count == 1 && construction.Single().Value == 0)
                {
                    //return new DimensionlessUnit()
                }
            }
            
            public override string ToString()
            {
                MeasurementDimensions dim = _dimensions;
                KeyValuePair<string, int>[] unitsAndDimensions =
                {
                    new KeyValuePair<string, int>("rad", _dimRad),
                    new KeyValuePair<string, int>("sr", _dimSr),
                    new KeyValuePair<string, int>("m", dim.LengthDimension),
                    new KeyValuePair<string, int>("kg", dim.MassDimension),
                    new KeyValuePair<string, int>("s", dim.TimeDimension),
                    new KeyValuePair<string, int>("A", dim.CurrentDimension),
                    new KeyValuePair<string, int>("K", dim.TemperatureDimension),
                    new KeyValuePair<string, int>("mol", dim.SubstanceAmountDimension),
                    new KeyValuePair<string, int>("cd", dim.LuminousIntensityDimension)
                };
                return FormatHelpers.FormatPowerNotation(unitsAndDimensions);
            }
        }

        internal class SIDerrivedUnit : Unit
        {
            private string _name;
            internal SIBaseUnit _equivalentUnit;

            internal SIDerrivedUnit(string name, SIBaseUnit equivalentUnit)
            {
                _name = name;
                _equivalentUnit = equivalentUnit;
            }

            internal override double SIConversion
            {
                get { return 1; }
            }

            public override Unit GetBaseSIRepresentation()
            {
                return _equivalentUnit;
            }

            internal override Unit Inverse()
            {
                Dictionary<Unit, int> construction = new Dictionary<Unit, int>
                {
                    { this, -1 }
                };
                return new ConstructedUnit(construction, -_equivalentUnit.Dimensions);
            }

            public override MeasurementDimensions Dimensions
            {
                get { return _equivalentUnit._dimensions; }
            }

            public override string ToString()
            {
                return _name;
            }
        }*/

        internal class ScaledUnit : Unit
        {
            internal Unit _unitToScale;
            internal double _scaleFactor;

            internal ScaledUnit(Unit unitToScale, double scaleFactor)
            {
                _unitToScale = unitToScale;
                _scaleFactor = scaleFactor;
            }

            private static readonly string[] _prefixes =
            {
                "y", // -24
                null,
                null,
                "z", // -21
                null,
                null,
                "a", // -18
                null,
                null,
                "f", // -15
                null,
                null,
                "p", // -12
                null,
                null,
                "n", // -9
                null,
                null,
                "µ", // -6
                null,
                null,
                "m", // -3
                "c", // -2
                "d", // -1
                null, // 0
                "da", // 1
                "h", // 2
                "k", // 3
                null,
                null,
                "M", // 6
                null,
                null,
                "G", // 9
                null,
                null,
                "T", // 12
                null,
                null,
                "P", // 15
                null,
                null,
                "E", // 18
                null,
                null,
                "Z", // 21
                null,
                null,
                "Y" // 24
            };

            public override MeasurementDimensions Dimensions
            {
                get { return _unitToScale.Dimensions; }
            }

            internal override double SIConversion
            {
                get { return _scaleFactor; }
            }

            internal override string Abbreviation
            {
                get { return ToString(); }
            }

            public override string ToString()
            {
                double log = Math.Log10(_scaleFactor);
                int intLog = (int)log;
                string prefix = null;
                if (log == intLog && intLog >= -24 && intLog <= 24)
                {
                    prefix = _prefixes[intLog + 24];
                }
                return (prefix ?? (_scaleFactor + "*")) + _unitToScale.Abbreviation;
            }
        }

        internal class ConstructedUnit : Unit
        {
            internal readonly Dictionary<Unit, int> _construction;
            internal double? _siConversion;
            internal MeasurementDimensions _dimensions;

            internal ConstructedUnit(Dictionary<Unit, int> construction, MeasurementDimensions dimensions)
            {
                _construction = construction;
                _dimensions = dimensions;
            }

            public override MeasurementDimensions Dimensions
            {
                get { return _dimensions; }
            }

            internal override double SIConversion
            {
                get
                {
                    if (_siConversion == null)
                    {
                        double ret = 1;
                        foreach (var kvp in _construction)
                        {
                            ret *= Math.Pow(kvp.Key.SIConversion, kvp.Value);
                        }
                        _siConversion = ret;
                        return ret;
                    }
                    else
                    {
                        return _siConversion.Value;
                    }
                }
            }

            internal override Dictionary<Unit, int> Construction
            {
                get { return _construction; }
            }

            internal override string Abbreviation
            {
                get { return ToString(); }
            }

            public override string ToString()
            {
                return FormatHelpers.FormatPowerNotation(_construction.Select(kvp => new KeyValuePair<string, int>(kvp.Key.Abbreviation, kvp.Value)));
            }
        }

        /*internal class SIDerivedUnit : Unit
        {
            private const int DIM_COUNT_MINUS_TWO = DIM_COUNT - 2;

            internal readonly int[] _dimensions;

            public SIDerivedUnit(int[] dimensions)
            {
                _dimensions = dimensions;
            }

            private static readonly string[] _unitLUT =
            {
                "kg",
                "m",
                "s"
            };

            private string _toString;

            public override string ToString()
            {
                string ret = _toString;
                if (ret == null)
                {
                    int[] array = _dimensions;
                    int nZeros = 0;
                    int iOne = -1;
                    int iMinusOne = -1;
                    for (int i = 0; i < DIM_COUNT; i++)
                    {
                        switch (array[i])
                        {
                            case 0:
                                nZeros++;
                                break;
                            case 1:
                                iOne = i;
                                break;
                            case -1:
                                iMinusOne = i;
                                break;
                        }
                    }
                    if (nZeros == DIM_COUNT_MINUS_TWO && iOne != -1 && iMinusOne != -1)
                    {
                        ret = _unitLUT[iOne] + "/" + _unitLUT[iMinusOne];
                    }
                    else
                    {
                        StringBuilder sb = new StringBuilder();
                        for (int i = 0; i < DIM_COUNT; i++)
                        {
                            sb.Append(formatUnit(_unitLUT[i], array[i]));
                            sb.Append('*');
                        }
                        sb.Length--;
                        ret = sb.ToString();
                    }
                    _toString = ret;
                }
                return ret;
            }

            private static string formatUnit(string unit, int pow)
            {
                switch (pow)
                {
                    case 0:
                        return "";
                    case 1:
                        return unit;
                    default:
                        StringBuilder sb = new StringBuilder(pow.ToString());
                        int start;
                        if (pow < 0)
                        {
                            start = 1;
                            sb[0] = SUPERSCRIPT_MINUS;
                        }
                        else
                        {
                            start = 0;
                        }
                        for (int i = start; i < sb.Length; i++)
                        {
                            sb[i] = _superscriptLUT[sb[i] - '0'];
                        }
                        return unit + sb;
                }
            }
        }

        /*internal class DimensionlessUnit : Unit
        {
            internal double _value;

            public DimensionlessUnit(double value)
            {
                _value = value;
            }

            public override string ToString()
            {
                return _value.ToString();
            }
        }*/

        internal const double
            MINUTE = 60,
            HOUR = 3600,
            DAY = 86400,
            WEEEK = 604800,
            YEAR = 31536000;

        /*public static readonly Unit Second = DefineUnit<Time>("second", 1);
        public static readonly Unit Minute = DefineUnit<Time>("minute", MINUTE);
        public static readonly Unit Hour = DefineUnit<Time>("hour", HOUR);
        public static readonly Unit Day = DefineUnit<Time>("day", DAY);
        public static readonly Unit Week = DefineUnit<Time>("day", DAY);
        public static readonly Unit Year = DefineUnit<Time>("year", YEAR);*/

        public string Name { get; private set; }

        public MeasurementSystem System { get; private set; }

        internal string[] _abbreviations;

        private static readonly Dictionary<OrderlessDouble<Unit>, Unit> _productMapper = new Dictionary<OrderlessDouble<Unit>, Unit>();
        private static readonly Dictionary<Tuple<Unit, Unit>, Unit> _quotientMapper = new Dictionary<Tuple<Unit, Unit>, Unit>();

        /*internal class DimensionlessUnit : Unit
        {

            public override MeasurementDimensions Dimensions
            {
                get { return MeasurementDimensions.Zero; }
            }

            internal readonly double _value;

            internal DimensionlessUnit(double value)
            {
                _value = value;
            }

            internal override double SIConversion
            {
                get { return _value; }
            }

            internal override Unit Inverse()
            {
                return new DimensionlessUnit(1 / _value);
            }

            public override Unit GetBaseSIRepresentation()
            {
                throw new InvalidOperationException();
            }

            public override string ToString()
            {
                return _value.ToString();
            }
        }*/

        private class OrderlessDouble<T>
        {
            public T Item1 { get; private set; }
            public T Item2 { get; private set; }
            public OrderlessDouble(T item1, T item2)
            {
                Item1 = item1;
                Item2 = item2;
            }
            public override bool Equals(object obj)
            {
                OrderlessDouble<T> other = obj as OrderlessDouble<T>;
                return other != null && ((other.Item1.Equals(Item1) && other.Item2.Equals(Item2)) || (other.Item1).Equals(Item2) && other.Item2.Equals(Item1));
            }
            public override int GetHashCode()
            {
                return Item1.GetHashCode() ^ Item2.GetHashCode();
            }
        }

        public static Unit operator *(Unit unit, double scalar)
        {
            if (unit == null)
            {
                throw new ArgumentNullException();
            }
            if (scalar == 1)
            {
                return unit;
            }
            ScaledUnit asScaled = unit as ScaledUnit;
            if (asScaled != null)
            {
                return new ScaledUnit(asScaled._unitToScale, asScaled._scaleFactor * scalar);
            }
            else
            {
                return new ScaledUnit(unit, scalar);
            }
        }

        public static Unit operator *(double scalar, Unit unit)
        {
            return unit * scalar;
        }

        public static Unit operator /(double dividend, Unit divisor)
        {
            if (divisor == null)
            {
                throw new ArgumentNullException();
            }
            return dividend * divisor.Inverse();
        }

        public static explicit operator double(Unit unit)
        {
            if (unit == null)
            {
                throw new NullReferenceException();
            }
            if (unit.Dimensions != MeasurementDimensions.Zero)
            {
                throw new InvalidCastException("Cannot cast unit " + unit + " to double because it is not dimensionless.");
            }
            return unit.SIConversion;
        }

        public static Unit operator *(Unit left, Unit right)
        {
            if (left == null)
            {
                throw new ArgumentNullException("dividend");
            }
            if (right == null)
            {
                throw new ArgumentNullException("divisor");
            }
            if (left == ONE)
            {
                return right;
            }
            if (right == ONE)
            {
                return left;
            }
            Dictionary<Unit, int> leftConstruction = left.Construction;
            Dictionary<Unit, int> rightConstruction = right.Construction;
            Dictionary<Unit, int> newConstruction = new Dictionary<Unit, int>(leftConstruction);
            foreach (var kvp in rightConstruction)
            {
                Unit unit = kvp.Key;
                int exp = kvp.Value;
                if (newConstruction.ContainsKey(unit))
                {
                    int newExp = newConstruction[unit] + exp;
                    if (newExp == 0)
                    {
                        newConstruction.Remove(unit);
                    }
                    else
                    {
                        newConstruction[unit] = newExp;
                    }
                }
                else
                {
                    newConstruction.Add(unit, exp);
                }
            }
            return Build(newConstruction, left.Dimensions + right.Dimensions);
        }

        /*internal static Unit Multiply(SIBaseUnit u1, SIBaseUnit u2)
        {
            MeasurementDimensions dim = u1.Dimensions + u2.Dimensions;
            int dimRad = u1._dimRad + u2._dimRad;
            int dimSr = u1._dimSr + u2._dimSr;
            if (dim == MeasurementDimensions.Zero && dimRad == 0 && dimSr == 0)
            {
                return new DimensionlessUnit(1);
            }
            else
            {
                return new SIBaseUnit(dim, dimRad, dimSr);
            }
        }

        internal static Unit Multiply(SIBaseUnit siUnit, DimensionlessUnit dimensionlessUnit)
        {
            double mul = dimensionlessUnit._value;
            if (mul == 1)
            {
                return siUnit;
            }
            else
            {
                return new ScaledUnit(siUnit, mul);
            }
        }

        internal static Unit Multiply(NonSIUnit u1, NonSIUnit u2)
        {
            Dictionary<Unit, int> construction;
            if (u1 == u2)
            {
                construction = new Dictionary<Unit, int>
                {
                    { u1,2 }
                };
            }
            else
            {
                construction = new Dictionary<Unit, int>
                {
                    { u1, 1 },
                    { u2, 1 }
                };
            }
            return new ConstructedUnit(construction, u1.Dimensions + u2.Dimensions);
        }

        internal static Unit Multiply(ConstructedUnit u1, ConstructedUnit u2)
        {
            Dictionary<Unit, int> u1Construction = u1._construction;
            Dictionary<Unit, int> u2Construction = u2._construction;
            Dictionary<Unit, int> newConstruction = new Dictionary<Unit, int>(u1Construction);
            foreach (var kvp in u2Construction)
            {
                Unit unit = kvp.Key;
                int exp = kvp.Value;
                if (newConstruction.ContainsKey(unit))
                {
                    int newExp = newConstruction[unit] + exp;
                    if (newExp == 0)
                    {
                        newConstruction.Remove(unit);
                    }
                    else
                    {
                        newConstruction[unit] = newExp;
                    }
                }
                else
                {
                    newConstruction.Add(unit, exp);
                }
            }
            if (newConstruction.Count == 0)
            {
                return new DimensionlessUnit(1);
            }
            return new ConstructedUnit(newConstruction, u2.Dimensions + u2.Dimensions);
        }*/

        public static Unit operator /(Unit dividend, Unit divisor)
        {
            if (dividend == null)
            {
                throw new ArgumentNullException("dividend");
            }
            if (divisor == null)
            {
                throw new ArgumentNullException("divisor");
            }
            return dividend * divisor.Inverse();
        }

        /*internal static Unit Divide(SIBaseUnit dividend, SIBaseUnit divisor)
        {
            MeasurementDimensions dim = dividend.Dimensions - divisor.Dimensions;
            int dimRad = dividend._dimRad - divisor._dimRad;
            int dimSr = dividend._dimSr - divisor._dimSr;
            if (dim == MeasurementDimensions.Zero && dimRad == 0 && dimSr == 0)
            {
                return new DimensionlessUnit(1);
            }
            else
            {
                return new SIBaseUnit(dim, dimRad, dimSr);
            }
        }

        public static Unit operator ^(Unit @base, int exp)
        {
            Unit ret = @base;
            if (exp < 0)
            {
                for (int i = 0; i > exp; i--)
                {
                    ret /= @base;
                }
            }
            else
            {
                for (int i = 0; i < exp; i++)
                {
                    ret *= @base;
                }
            }
            return ret;
        }


        /*private static readonly Dictionary<Unit<Length>, Unit<Area>> _squareUnitMapper = new Dictionary<Unit<Length>, Unit<Area>>();
        private static readonly Dictionary<Unit<Length>, Unit<Volume>> _cubicUnitMapper = new Dictionary<Unit<Length>, Unit<Volume>>();
        private static readonly Dictionary<Tuple<double, Unit>, Unit> _prefixedUnitMapper = new Dictionary<Tuple<double, Unit>, Unit>();

        internal Unit(string name, MeasurementSystem system, double siConversion, params string[] abbreviations)
        {
            Name = name;
            System = system;
            _siConversion = siConversion;
            _abbreviations = abbreviations;
        }*/

        internal const double YOTTA = 1E24;
        internal const double ZETTA = 1E21;
        internal const double EXA = 1E18;
        internal const double PETA = 1E15;
        internal const double TERA = 1E12;
        public const double GIGA = 1E9;
        public const double MEGA = 1E6;
        public const double KILO = 1E3;
        public const double HECTO = 1E2;
        public const double DECA = 1E1;
        public const double DECI = 1E-1;
        public const double CENTI = 1E-2;
        public const double MILLI = 1E-3;
        public const double MICRO = 1E-6;
        public const double NANO = 1E-9;
        internal const double PICO = 1E-12;
        internal const double FEMTO = 1E-15;
        internal const double ATTO = 1E-18;
        internal const double ZEPTO = 1E-21;
        internal const double YOCTO = 1E-24;

        /*public static readonly Unit<Length> Metre = new Unit<Length>("metre", MeasurementSystem.Metric, 1, "m");
        public static readonly Unit<Length> Inch = new Unit<Length>("inch", MeasurementSystem.Imperial, Length.INCH, "in");
        public static readonly Unit<Length> Foot = new Unit<Length>("foot", MeasurementSystem.Imperial, Length.FOOT, "feet", "ft");
        public static readonly Unit<Length> Yard = new Unit<Length>("yard", MeasurementSystem.Imperial, Length.YARD, "yd");
        public static readonly Unit<Length> Mile = new Unit<Length>("mile", MeasurementSystem.Imperial, Length.MILE, "mi");
        public static readonly Unit<Length> NauticalMile = new Unit<Length>("nautical mile", MeasurementSystem.Imperial, Length.NAUTICAL_MILE, "nmi");
        public static readonly Unit<Mass> Gram = new Unit<Mass>("gram", MILLI, "g");
        public static readonly Unit<Mass> Tonne = new Unit<Mass>("tonne", KILO, "t");
        public static readonly Unit<Mass> Pound = new Unit<Mass>("pound", Mass.POUND, "lb", "lbs");
        public static readonly Unit<Time> Second = new Unit<Time>("second", 1, "s");
        public static readonly Unit<Time> Minute = new Unit<Time>("minute", Time.MINUTE, "min");
        public static readonly Unit<Time> Hour = new Unit<Time>("hour", Time.HOUR, "hr");
        public static readonly Unit<Time> Day = new Unit<Time>("day", Time.DAY, "d");
        public static readonly Unit<Time> Week = new Unit<Time>("week", Time.WEEK, "wk");
        public static readonly Unit<Time> Year = new Unit<Time>("year", Time.YEAR, "yr");
        public static readonly Unit<Speed> MetresPerSecond = new Unit<Speed>("metres per second", 1, "mps");
        public static readonly Unit<Speed> MilesPerHour = new Unit<Speed>("miles per hour", Speed.MPH, "mph");
        public static readonly Unit<Speed> KilometresPerHour = new Unit<Speed>("kilometres per hour", Speed.KILOMETRES_PER_HOUR);
        public static readonly Unit<Speed> Knot = new Unit<Speed>("knot", Speed.KNOT, "kn", "kt");
        public static readonly Unit<Speed> FeetPerSecond = new Unit<Speed>("feet per second", Speed.FEET_PER_SECOND, "fps");
        public static readonly Unit<Area> Acre = new Unit<Area>("acre", Area.ACRE);
        public static readonly Unit<Volume> Litre = new Unit<Volume>("litre", Volume.LITRE, "l");
        
        public override string ToString()
        {
            return Name;
        }

        public static Unit<Area> Square(Unit<Length> lengthUnit)
        {
            lock (_squareUnitMapper)
            {
                Unit<Area> ret;
                if (!_squareUnitMapper.TryGetValue(lengthUnit, out ret))
                {
                    ret = new Unit<Area>("square " + lengthUnit.Name, Math.Pow(lengthUnit._siConversion, 2));
                    _squareUnitMapper.Add(lengthUnit, ret);
                }
                return ret;
            }
        }

        public static Unit<Volume> Cubic(Unit<Length> lengthUnit)
        {
            lock (_cubicUnitMapper)
            {
                Unit<Volume> ret;
                if (!_cubicUnitMapper.TryGetValue(lengthUnit, out ret))
                {
                    ret = new Unit<Volume>("cubic " + lengthUnit.Name, Math.Pow(lengthUnit._siConversion, 3));
                    _cubicUnitMapper.Add(lengthUnit, ret);
                }
                return ret;
            }
        }

        public static Unit<T> Yotta<T>(Unit<T> baseUnit)
            where T : struct
        {
            return prefix<T>(baseUnit, "yotta", "Y", YOTTA);
        }
        public static Unit<T> Zetta<T>(Unit<T> baseUnit)
            where T : struct
        {
            return prefix<T>(baseUnit, "zetta", "Z", ZETTA);
        }
        public static Unit<T> Exa<T>(Unit<T> baseUnit)
            where T : struct
        {
            return prefix<T>(baseUnit, "exa", "E", EXA);
        }
        public static Unit<T> Peta<T>(Unit<T> baseUnit)
            where T : struct
        {
            return prefix<T>(baseUnit, "peta", "P", PETA);
        }
        public static Unit<T> Tera<T>(Unit<T> baseUnit)
            where T : struct
        {
            return prefix<T>(baseUnit, "Tera", "T", TERA);
        }
        public static Unit<T> Giga<T>(Unit<T> baseUnit)
            where T : struct
        {
            return prefix<T>(baseUnit, "giga", "G", GIGA);
        }
        public static Unit<T> Mega<T>(Unit<T> baseUnit)
            where T : struct
        {
            return prefix<T>(baseUnit, "mega", "M", MEGA);
        }
        public static Unit<T> Kilo<T>(Unit<T> baseUnit)
            where T : struct
        {
            return prefix<T>(baseUnit, "kilo", "k", KILO);
        }
        public static Unit<T> Hecto<T>(Unit<T> baseUnit)
            where T : struct
        {
            return prefix<T>(baseUnit, "hecto", "h", HECTO);
        }
        public static Unit<T> Deca<T>(Unit<T> baseUnit)
            where T : struct
        {
            return prefix<T>(baseUnit, "deca", "da", DECA);
        }
        public static Unit<T> Deci<T>(Unit<T> baseUnit)
            where T : struct
        {
            return prefix<T>(baseUnit, "deci", "d", DECI);
        }
        public static Unit<T> Centi<T>(Unit<T> baseUnit)
            where T : struct
        {
            return prefix<T>(baseUnit, "centi", "c", CENTI);
        }
        public static Unit<T> Milli<T>(Unit<T> baseUnit)
            where T : struct
        {
            return prefix<T>(baseUnit, "milli", "m", MILLI);
        }
        public static Unit<T> Micro<T>(Unit<T> baseUnit)
            where T : struct
        {
            return prefix<T>(baseUnit, "micro", "μ", MICRO);
        }
        public static Unit<T> Nano<T>(Unit<T> baseUnit)
            where T : struct
        {
            return prefix<T>(baseUnit, "nano", "n", NANO);
        }
        public static Unit<T> Pico<T>(Unit<T> baseUnit)
            where T : struct
        {
            return prefix<T>(baseUnit, "pico", "p", PICO);
        }
        public static Unit<T> Femto<T>(Unit<T> baseUnit)
            where T : struct
        {
            return prefix<T>(baseUnit, "femto", "f", FEMTO);
        }
        public static Unit<T> Atto<T>(Unit<T> baseUnit)
            where T : struct
        {
            return prefix<T>(baseUnit, "atto", "a", ATTO);
        }
        public static Unit<T> Zepto<T>(Unit<T> baseUnit)
            where T : struct
        {
            return prefix<T>(baseUnit, "zepto", "z", ZEPTO);
        }
        public static Unit<T> Yocto<T>(Unit<T> baseUnit)
            where T : struct
        {
            return prefix<T>(baseUnit, "yocto", "y", YOCTO);
        }

        private static Unit<T> prefix<T>(Unit<T> baseUnit, string prefix, string prefixSymbol, double multiplier)
            where T : struct
        {
            if (baseUnit == null)
            {
                throw new ArgumentNullException("baseUnit");
            }
            if (baseUnit.BaseUnit != null)
            {
                throw new ArgumentException("Already a prefixed unit.", "baseUnit");
            }
            lock (_prefixedUnitMapper)
            {
                Tuple<double, Unit> tuple = new Tuple<double, Unit>(multiplier, baseUnit);
                Unit existing;
                if (_prefixedUnitMapper.TryGetValue(tuple, out existing))
                {
                    return (Unit<T>)existing;
                }
                else
                {
                    string[] abbreviations = (string[])baseUnit._abbreviations.Clone();
                    if (abbreviations.Length != 0)
                    {
                        abbreviations[0] = prefixSymbol + abbreviations[0];
                    }
                    Unit<T> ret = new Unit<T>(prefix + baseUnit.Name, baseUnit.System, baseUnit._siConversion * multiplier, abbreviations)
                    {
                        BaseUnit = baseUnit
                    };
                    _prefixedUnitMapper.Add(tuple, ret);
                    return ret;
                }
            }
        }

        public static class Imperial
        {
            internal const double THOU = 0.0000254;
            internal const double CHAIN = 20.1168;
            internal const double FURLONG = 201.168;
            public static readonly Unit<Length> Thou = new Unit<Length>("thou", MeasurementSystem.Imperial, THOU, "th", "mil");
            public static readonly Unit<Length> Inch = new Unit<Length>("inch", MeasurementSystem.Imperial, Length.INCH, "in");
            public static readonly Unit<Length> Foot = new Unit<Length>("foot", MeasurementSystem.Imperial, Length.FOOT, "feet", "ft");
            public static readonly Unit<Length> Yard = new Unit<Length>("yard", MeasurementSystem.Imperial, Length.YARD, "yd");
            public static readonly Unit<Length> Mile = new Unit<Length>("mile", MeasurementSystem.Imperial, Length.MILE, "mi");
            public static readonly Unit<Length> NauticalMile = new Unit<Length>("nautical mile", MeasurementSystem.Imperial, Length.NAUTICAL_MILE, "nmi");
            public static readonly Unit<Length> Chain = new Unit<Length>("chain", MeasurementSystem.Imperial, CHAIN, "ch");
            public static readonly Unit<Length> Furlong = new Unit<Length>("furlong", MeasurementSystem.Imperial, FURLONG, "fur");
            public static readonly Unit<Area> Acre = new Unit<Area>("acre", MeasurementSystem.Imperial, Area.ACRE);

            internal const double GRAIN = 0.00006479891;
            internal const double OUNCE = 0.028349523125;
            internal const double POUND = 0.45359237;
            internal const double TON = 2000;
            public static readonly Unit<Mass> Grain = new Unit<Mass>("grain", GRAIN, "gr");
            public static readonly Unit<Mass> Ounce = new Unit<Mass>("ounce", OUNCE, "oz");
            public static readonly Unit<Mass> Pound = new Unit<Mass>("pound", POUND, "lb", "lbs");
            public static readonly Unit<Mass> Ton = new Unit<Mass>("ton", TON);
        }

        public static class Metric
        {
            public static readonly Unit<Length> Metre = new Unit<Length>("metre", 1, "m");
            public static readonly Unit<Mass> Gram = new Unit<Mass>("gram", MeasurementSystem.Metric, MILLI, "g");
            public static readonly Unit<Mass> Kilogram = Kilo(Gram);
            public static readonly Unit<Mass> Tonne = new Unit<Mass>("tonne", MeasurementSystem.Metric, KILO, "t");
            public static readonly Unit<Time> Second = new Unit<Time>("second", 1, "s", "sec");
            public static readonly Unit<Force> Newton = new Unit<Force>("newton", MeasurementSystem.Metric, 1, "N");
            public static readonly Unit<Energy> Joule = new Unit<Energy>("joule", MeasurementSystem.Metric, 1, "J");
        }*/
    }

    /*public sealed class Unit<T> : Unit
        where T : struct
    {
        public Unit<T> BaseUnit { get; internal set; }

        public double Convert(double value, Unit<T> conversionsUnit)
        {
            return value * _siConversion / conversionsUnit._siConversion;
        }

        internal Unit(string name, MeasurementSystem system, double siConversion, params string[] abbreviations)
            : base(name, system, siConversion, abbreviations) { }

        internal Unit(string name, double siConversion, params string[] abbreviations)
            : base(name, MeasurementSystem.None, siConversion, abbreviations) { }
    }*/
}
