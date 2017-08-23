using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace BB.Measure
{
    public unsafe struct MeasurementDimensions
    {
        private const string THETA = "Θ";

        internal long _val;

        private const int LENGTH_INDEX = 0;
        private const int MASS_INDEX = 1;
        private const int TIME_INDEX = 2;
        private const int CURRENT_INDEX = 3;
        private const int TEMPERATURE_INDEX = 4;
        private const int SUBSTANCE_AMOUNT_INDEX = 5;
        private const int LUMINOUS_INTENSITY_INDEX = 6;

        public static readonly MeasurementDimensions Zero = new MeasurementDimensions();
        public static readonly MeasurementDimensions Length = new MeasurementDimensions(L: 1);
        public static readonly MeasurementDimensions Mass = new MeasurementDimensions(M: 1);
        public static readonly MeasurementDimensions Time = new MeasurementDimensions(T: 1);
        public static readonly MeasurementDimensions Current = new MeasurementDimensions(I: 1);
        public static readonly MeasurementDimensions Temperature = new MeasurementDimensions(Θ: 1);
        public static readonly MeasurementDimensions SubstanceAmount = new MeasurementDimensions(N: 1);
        public static readonly MeasurementDimensions LuminousIntensity = new MeasurementDimensions(J: 1);

        private static readonly string[] _dimensionSymbols =
        {
            "L",
            "M",
            "T",
            "I",
            "Θ",
            "N",
            "J"
        };

        public int LengthDimension
        {
            get
            {
                fixed (long* ptr = &_val)
                {
                    return ((sbyte*)ptr)[LENGTH_INDEX];
                }
            }
        }
        public int MassDimension
        {
            get
            {
                fixed (long* ptr = &_val)
                {
                    return ((sbyte*)ptr)[MASS_INDEX];
                }
            }
        }
        public int TimeDimension
        {
            get
            {
                fixed (long* ptr = &_val)
                {
                    return ((sbyte*)ptr)[TIME_INDEX];
                }
            }
        }
        public int CurrentDimension
        {
            get
            {
                fixed (long* ptr = &_val)
                {
                    return ((sbyte*)ptr)[CURRENT_INDEX];
                }
            }
        }
        public int TemperatureDimension
        {
            get
            {
                fixed (long* ptr = &_val)
                {
                    return ((sbyte*)ptr)[TEMPERATURE_INDEX];
                }
            }
        }
        public int SubstanceAmountDimension
        {
            get
            {
                fixed (long* ptr = &_val)
                {
                    return ((sbyte*)ptr)[SUBSTANCE_AMOUNT_INDEX];
                }
            }
        }
        public int LuminousIntensityDimension
        {
            get
            {
                fixed (long* ptr = &_val)
                {
                    return ((sbyte*)ptr)[LUMINOUS_INTENSITY_INDEX];
                }
            }
        }

        public MeasurementDimensions(int L = 0, int M = 0, int T = 0, int I = 0, int Θ = 0, int N = 0, int J = 0)
        {
            long val = 0;
            sbyte* ptr = (sbyte*)&val;
            ptr[LENGTH_INDEX] = (sbyte)L;
            ptr[MASS_INDEX] = (sbyte)M;
            ptr[TIME_INDEX] = (sbyte)T;
            ptr[CURRENT_INDEX] = (sbyte)I;
            ptr[TEMPERATURE_INDEX] = (sbyte)Θ;
            ptr[SUBSTANCE_AMOUNT_INDEX] = (sbyte)N;
            ptr[LUMINOUS_INTENSITY_INDEX] = (sbyte)J;
            _val = val;
        }

        internal MeasurementDimensions(long val)
        {
            _val = val;
        }

        public static MeasurementDimensions operator +(MeasurementDimensions dim1, MeasurementDimensions dim2)
        {
            long val1 = dim1._val;
            long val2 = dim2._val;

            sbyte* ptr1 = (sbyte*)&val1;
            sbyte* ptr2 = (sbyte*)&val2;
            ptr1[0] += ptr2[0];
            ptr1[1] += ptr2[1];
            ptr1[2] += ptr2[2];
            ptr1[3] += ptr2[3];
            ptr1[4] += ptr2[4];
            ptr1[5] += ptr2[5];
            ptr1[6] += ptr2[6];
            return new MeasurementDimensions(val1);
        }

        public static MeasurementDimensions operator -(MeasurementDimensions left, MeasurementDimensions right)
        {
            long bLeft = left._val;
            long bRight = left._val;

            sbyte* ptrLeft = (sbyte*)&bLeft;
            sbyte* ptrRight = (sbyte*)&bRight;
            ptrLeft[0] -= ptrRight[0];
            ptrLeft[1] -= ptrRight[1];
            ptrLeft[2] -= ptrRight[2];
            ptrLeft[3] -= ptrRight[3];
            ptrLeft[4] -= ptrRight[4];
            ptrLeft[5] -= ptrRight[5];
            ptrLeft[6] -= ptrRight[6];
            return new MeasurementDimensions(bLeft);
        }

        public static MeasurementDimensions operator -(MeasurementDimensions dim)
        {
            long b = dim._val;

            sbyte* ptr = (sbyte*)&b;
            ptr[0] = (sbyte)-ptr[0];
            ptr[1] = (sbyte)-ptr[1];
            ptr[2] = (sbyte)-ptr[2];
            ptr[3] = (sbyte)-ptr[3];
            ptr[4] = (sbyte)-ptr[4];
            ptr[5] = (sbyte)-ptr[5];
            ptr[6] = (sbyte)-ptr[6];
            return new MeasurementDimensions(b);
        }

        public static bool operator ==(MeasurementDimensions left, MeasurementDimensions right)
        {
            return left._val == right._val;
        }
        public static bool operator !=(MeasurementDimensions left, MeasurementDimensions right)
        {
            return left._val != right._val;
        }

        public override bool Equals(object obj)
        {
            return obj is MeasurementDimensions && (MeasurementDimensions)obj == this;
        }

        public override int GetHashCode()
        {
            return _val.GetHashCode();
        }

        internal static void Validate(int dimension, string paramName)
        {
            if (dimension > sbyte.MaxValue || dimension < sbyte.MinValue)
            {
                throw new ArgumentOutOfRangeException(paramName, "Dimension must be between -128 and 127.");
            }
        }

        public override string ToString()
        {
            KeyValuePair<string, int>[] unitsAndDimensions =
            {
                new KeyValuePair<string, int>("L", LengthDimension),
                new KeyValuePair<string, int>("M", MassDimension),
                new KeyValuePair<string, int>("T", TimeDimension),
                new KeyValuePair<string, int>("I", CurrentDimension),
                new KeyValuePair<string, int>("Θ", TemperatureDimension),
                new KeyValuePair<string, int>("N", SubstanceAmountDimension),
                new KeyValuePair<string, int>("J", LuminousIntensityDimension)
            };
            return FormatHelpers.FormatPowerNotation(unitsAndDimensions);
        }
    }
}
