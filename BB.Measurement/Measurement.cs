using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace BB.Measure
{
    public static class Measurement
    {
        private static readonly Dictionary<MeasurementDimensions, MeasurementTypeProperties> _indexedByDim;
        private static readonly Dictionary<Type, MeasurementTypeProperties> _indexedByType;
        
        private class MeasurementTypeProperties
        {
            public MeasurementDimensions Dimensions;
            public Func<object, double> ValueGetter;
            public Func<double, ValueType> Initializer;
        }

        static Measurement()
        {
            Dictionary<MeasurementDimensions, MeasurementTypeProperties> indexedByDim = new Dictionary<MeasurementDimensions, MeasurementTypeProperties>();
            Dictionary<Type, MeasurementTypeProperties> indexedByType = new Dictionary<Type, MeasurementTypeProperties>();
            foreach (Type type in AppDomain.CurrentDomain.GetAssemblies().SelectMany(a => a.GetTypes()))
            {
                if (type.IsValueType)
                {
                    object[] attributes = type.GetCustomAttributes(typeof(MeasurementAttribute), false);
                    if (attributes.Length == 1)
                    {
                        MeasurementAttribute att = (MeasurementAttribute)attributes[0];
                        FieldInfo[] fields = type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                        if (fields.Length == 1)
                        {
                            FieldInfo field = fields[0];
                            if (field.FieldType == typeof(double))
                            {
                                MeasurementDimensions dim = new MeasurementDimensions(att.DimLength, att.DimMass, att.DimTime, 0, 0, 0,0);

                                DynamicMethod dynValueGetter = new DynamicMethod("get_value", typeof(double), new[] { typeof(object) }, type, true);
                                ILGenerator ilValueGetter = dynValueGetter.GetILGenerator();
                                ilValueGetter.Emit(OpCodes.Ldarg_0);
                                ilValueGetter.Emit(OpCodes.Unbox_Any, type);
                                ilValueGetter.Emit(OpCodes.Ldfld, field);
                                ilValueGetter.Emit(OpCodes.Ret);
                                Func<object, double> valueGetter = (Func<object, double>)dynValueGetter.CreateDelegate(typeof(Func<object, double>));
                                DynamicMethod dynInitializer = new DynamicMethod("init", typeof(ValueType), new[] { typeof(double) }, type, true);
                                
                                ILGenerator ilInitializer = dynInitializer.GetILGenerator();
                                LocalBuilder loc = ilInitializer.DeclareLocal(type);
                                ilInitializer.Emit(OpCodes.Ldloca_S, loc);
                                ilInitializer.Emit(OpCodes.Initobj, type);
                                ilInitializer.Emit(OpCodes.Ldloca_S, loc);
                                ilInitializer.Emit(OpCodes.Ldarg_0);
                                ilInitializer.Emit(OpCodes.Stfld, field);
                                ilInitializer.Emit(OpCodes.Ldloc_0);
                                ilInitializer.Emit(OpCodes.Box, type);
                                ilInitializer.Emit(OpCodes.Ret);
                                Func<double, ValueType> initializer = (Func<double, ValueType>)dynInitializer.CreateDelegate(typeof(Func<double, ValueType>));

                                MeasurementTypeProperties prop = new MeasurementTypeProperties
                                {
                                    Dimensions = dim,
                                    Initializer = initializer,
                                    ValueGetter = valueGetter
                                };
                                if (!indexedByDim.ContainsKey(dim))
                                {
                                    indexedByDim.Add(dim, prop);
                                    indexedByType.Add(type, prop);
                                }
                            }
                        }
                    }
                }
            }
            _indexedByDim = indexedByDim;
            _indexedByType = indexedByType;
        }

        public static T Add<T>(T value1, T value2)
            where T : struct
        {
            return Create<T>(GetSIValue(value1) + GetSIValue(value2));
        }

        public static T Subtract<T>(T left, T right)
            where T : struct
        {
            return Create<T>(GetSIValue(left) - GetSIValue(right));
        }

        public static MeasurementDimensions GetDimensions(ValueType measurement)
        {
            if (measurement == null)
            {
                throw new ArgumentNullException("measurement");
            }
            if (measurement is AbstractMeasurement)
            {
                return ((AbstractMeasurement)measurement)._dim;
            }
            else
            {
                MeasurementTypeProperties prop;
                if (_indexedByType.TryGetValue(measurement.GetType(), out prop))
                {
                    return prop.Dimensions;
                }
                else
                {
                    throw new ArgumentException("Does not represent a measurement.", "measurement");
                }
            }
        }

        public static ValueType Multiply(ValueType measurement1, ValueType measurement2)
        {
            if (measurement1 == null)
            {
                throw new ArgumentNullException("measurement1");
            }
            if (measurement2 == null)
            {
                throw new ArgumentNullException("measurement2");
            }
            double value1, value2;
            MeasurementDimensions dim1, dim2;
            if (measurement1 is AbstractMeasurement)
            {
                AbstractMeasurement am1 = (AbstractMeasurement)measurement1;
                value1 = am1._value;
                dim1 = am1._dim;
            }
            else
            {
                MeasurementTypeProperties prop;
                if (_indexedByType.TryGetValue(measurement1.GetType(), out prop))
                {
                    value1 = prop.ValueGetter(measurement1);
                    dim1 = prop.Dimensions;
                }
                else if (tryConvertDouble(measurement1, out value1))
                {
                    dim1 = MeasurementDimensions.Zero;
                }
                else
                {
                    throw new ArgumentException("Does not represent a measurement.", "measurement1");
                }
            }
            if (measurement2 is AbstractMeasurement)
            {
                AbstractMeasurement am2 = (AbstractMeasurement)measurement2;
                value2 = am2._value;
                dim2 = am2._dim;
            }
            else
            {
                MeasurementTypeProperties prop;
                if (_indexedByType.TryGetValue(measurement2.GetType(), out prop))
                {
                    value2 = prop.ValueGetter(measurement2);
                    dim2 = prop.Dimensions;
                }
                else if (tryConvertDouble(measurement2, out value2))
                {
                    dim2 = MeasurementDimensions.Zero;
                }
                else
                {
                    throw new ArgumentException("Does not represent a measurement.", "measurement2");
                }
            }
            return Create(value1 * value2, dim1 + dim2);
        }

        public static ValueType Divide(ValueType dividend, ValueType divisor)
        {
            if (dividend == null)
            {
                throw new ArgumentNullException("measurement1");
            }
            if (divisor == null)
            {
                throw new ArgumentNullException("measurement2");
            }
            double value1, value2;
            MeasurementDimensions dim1, dim2;
            if (dividend is AbstractMeasurement)
            {
                AbstractMeasurement am1 = (AbstractMeasurement)dividend;
                value1 = am1._value;
                dim1 = am1._dim;
            }
            else
            {
                MeasurementTypeProperties prop;
                if (_indexedByType.TryGetValue(dividend.GetType(), out prop))
                {
                    value1 = prop.ValueGetter(dividend);
                    dim1 = prop.Dimensions;
                }
                else if (tryConvertDouble(dividend, out value1))
                {
                    dim1 = MeasurementDimensions.Zero;
                }
                else
                {
                    throw new ArgumentException("Does not represent a measurement.", "measurement1");
                }
            }
            if (divisor is AbstractMeasurement)
            {
                AbstractMeasurement am2 = (AbstractMeasurement)divisor;
                value2 = am2._value;
                dim2 = am2._dim;
            }
            else
            {
                MeasurementTypeProperties prop;
                if (_indexedByType.TryGetValue(divisor.GetType(), out prop))
                {
                    value2 = prop.ValueGetter(divisor);
                    dim2 = prop.Dimensions;
                }
                else if (tryConvertDouble(divisor, out value2))
                {
                    dim2 = MeasurementDimensions.Zero;
                }
                else
                {
                    throw new ArgumentException("Does not represent a measurement.", "measurement2");
                }
            }
            return Create(value1 / value2, dim1 - dim2);
        }

        public static T Create<T>(double value, Unit unit)
        {
            throw new NotImplementedException();
        }

        public static T Create<T>(double value, string unit)
        {
            throw new NotImplementedException();
        }

        internal static string Format(ValueType measurement, string format, IFormatProvider formatProvider)
        {
            if (formatProvider == null)
            {
                formatProvider = CultureInfo.CurrentCulture;
            }
            NumberFormatInfo numberFormat = formatProvider as NumberFormatInfo;
            CultureInfo culture = formatProvider as CultureInfo;
            if (numberFormat == null)
            {
                numberFormat = NumberFormatInfo.CurrentInfo;
            }
            if (culture == null)
            {
                culture = CultureInfo.CurrentCulture;
            }
            return Format(measurement, format, numberFormat, culture);
        }
        internal static string Format(ValueType measurement, string format, NumberFormatInfo numberFormat, CultureInfo culture)
        {
            throw new NotImplementedException();
            RegionInfo region = new RegionInfo(culture.LCID);
            if (region.IsMetric)
            {

            }
        }

        private static bool tryConvertDouble(ValueType value, out double doubleVal)
        {
            switch (Type.GetTypeCode(value.GetType()))
            {
                case TypeCode.Byte:
                case TypeCode.Decimal:
                case TypeCode.Double:
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                case TypeCode.SByte:
                case TypeCode.Single:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                    doubleVal = Convert.ToDouble(value);
                    return true;
                default:
                    doubleVal = double.NaN;
                    return false;

            }
        }

        public static double GetSIValue(ValueType measurement)
        {
            if (measurement == null)
            {
                throw new ArgumentNullException("measurement");
            }
            if (measurement is AbstractMeasurement)
            {
                return ((AbstractMeasurement)measurement)._value;
            }
            else
            {
                MeasurementTypeProperties prop;
                if (_indexedByType.TryGetValue(measurement.GetType(), out prop))
                {
                    return prop.ValueGetter(measurement);
                }
                else
                {
                    throw new ArgumentException("Does not represent a measurement.", "measurement");
                }
            }
        }

        private static string formatManagedMeasurement(ValueType measurement)
        {
            MeasurementTypeProperties prop = _indexedByType[measurement.GetType()];
            return Format(prop.ValueGetter(measurement), prop.Dimensions);
        }

        public static string Format(double value, MeasurementDimensions dim)
        {
            int dimLen = dim.LengthDimension;
            int dimTime = dim.TimeDimension;
            int dimMass = dim.MassDimension;
            List<string> units = new List<string>();
            if (dimMass != 0)
            {
                if (dimMass == 1)
                {
                    units.Add("kg");
                }
                else
                {
                    units.Add("kg^" + dimMass);
                }
            }
            if (dimLen != 0)
            {
                if (dimLen == 1)
                {
                    units.Add("m");
                }
                else
                {
                    units.Add("m^" + dimLen);
                }
            }
            if (dimTime != 0)
            {
                if (dimTime == 1)
                {
                    units.Add("s");
                }
                else
                {
                    units.Add("s^" + dimTime);
                }
            }
            return value + String.Join("*", units);
        }

        public static T Create<T>(double siValue)
            where T : struct
        {
            MeasurementTypeProperties prop;
            if (_indexedByType.TryGetValue(typeof(T), out prop))
            {
                return (T)prop.Initializer(siValue);
            }
            switch (Type.GetTypeCode(typeof(T)))
            {
                case TypeCode.Byte:
                case TypeCode.Decimal:
                case TypeCode.Double:
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                case TypeCode.SByte:
                case TypeCode.Single:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                    return (T)(ValueType)Convert.ToDouble(siValue);
                default:
                    throw new ArgumentException($"Type {typeof(T)} is not a measurement type.", "T");
            }
        }

        public static ValueType Create(double siValue, MeasurementDimensions dim)
        {
            MeasurementTypeProperties prop;
            if (_indexedByDim.TryGetValue(dim, out prop))
            {
                return prop.Initializer(siValue);
            }
            else if (dim == MeasurementDimensions.Zero)
            {
                return siValue;
            }
            else
            {
                return new AbstractMeasurement(siValue, dim);
            }
        }

        public static T Average<T>(this IEnumerable<T> values)
            where T : struct
        {
            if (values == null)
            {
                throw new ArgumentNullException();
            }
            MeasurementTypeProperties prop;
            if (!_indexedByType.TryGetValue(typeof(T), out prop))
            {
                throw new ArgumentException();
            }
            var valueGetter = prop.ValueGetter;
            return Create<T>(values.Select(v => valueGetter(v)).Average());
        }
    }
}
