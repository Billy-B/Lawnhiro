using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseManagement.SQL
{
    public static class Functions
    {
        public static readonly ScalarFunction ABS = new ScalarFunction
        {
            Name = "ABS",
            ReturnType = DbType.VarNumeric,
            Parameters = new ReadOnlyList<FunctionParameter>
            {
                new FunctionParameter
                {
                    ParameterType = DbType.VarNumeric,
                    IsOptional = false
                },
            }
        };

        public static readonly ScalarFunction ACOS = new ScalarFunction
        {
            Name = "ACOS",
            ReturnType = DbType.Int32,
            Parameters = new ReadOnlyList<FunctionParameter>
            {
                new FunctionParameter
                {
                    ParameterType = DbType.Double,
                    IsOptional = false
                }
            }
        };

        public static readonly ScalarFunction ASIN = new ScalarFunction
        {
            Name = "ASIN",
            ReturnType = DbType.Double,
            Parameters = new ReadOnlyList<FunctionParameter>
            {
                new FunctionParameter
                {
                    ParameterType = DbType.Double,
                    IsOptional = false
                }
            }
        };

        public static readonly ScalarFunction ATAN = new ScalarFunction
        {
            Name = "ATAN",
            ReturnType = DbType.Double,
            Parameters = new ReadOnlyList<FunctionParameter>
            {
                new FunctionParameter
                {
                    ParameterType = DbType.Double,
                    IsOptional = false
                }
            }
        };

        public static readonly ScalarFunction ATN2 = new ScalarFunction
        {
            Name = "ATN2",
            ReturnType = DbType.Double,
            Parameters = new ReadOnlyList<FunctionParameter>
            {
                new FunctionParameter
                {
                    ParameterType = DbType.Double,
                    IsOptional = false
                },
                new FunctionParameter
                {
                    ParameterType = DbType.Double,
                    IsOptional = false
                }
            }
        };

        public static readonly ScalarFunction CEILING = new ScalarFunction
        {
            Name = "CEILING",
            ReturnType = DbType.VarNumeric,
            Parameters = new ReadOnlyList<FunctionParameter>
            {
                new FunctionParameter
                {
                    ParameterType = DbType.VarNumeric,
                    IsOptional = false
                }
            }
        };

        public static readonly ScalarFunction COS = new ScalarFunction
        {
            Name = "COS",
            ReturnType = DbType.Double,
            Parameters = new ReadOnlyList<FunctionParameter>
            {
                new FunctionParameter
                {
                    ParameterType = DbType.Double,
                    IsOptional = false
                }
            }
        };

        public static readonly ScalarFunction COT = new ScalarFunction
        {
            Name = "COT",
            ReturnType = DbType.Double,
            Parameters = new ReadOnlyList<FunctionParameter>
            {
                new FunctionParameter
                {
                    ParameterType = DbType.Double,
                    IsOptional = false
                }
            }
        };

        public static readonly ScalarFunction DEGREES = new ScalarFunction
        {
            Name = "DEGREES",
            ReturnType = DbType.VarNumeric,
            Parameters = new ReadOnlyList<FunctionParameter>
            {
                new FunctionParameter
                {
                    ParameterType = DbType.VarNumeric,
                    IsOptional = false
                }
            }
        };

        public static readonly ScalarFunction EXP = new ScalarFunction
        {
            Name = "EXP",
            ReturnType = DbType.Double,
            Parameters = new ReadOnlyList<FunctionParameter>
            {
                new FunctionParameter
                {
                    ParameterType = DbType.Double,
                    IsOptional = false
                }
            }
        };

        public static readonly ScalarFunction FLOOR = new ScalarFunction
        {
            Name = "FLOOR",
            ReturnType = DbType.VarNumeric,
            Parameters = new ReadOnlyList<FunctionParameter>
            {
                new FunctionParameter
                {
                    ParameterType = DbType.VarNumeric,
                    IsOptional = false
                }
            }
        };

        public static readonly ScalarFunction LOG = new ScalarFunction
        {
            Name = "LOG",
            ReturnType = DbType.Double,
            Parameters = new ReadOnlyList<FunctionParameter>
            {
                new FunctionParameter
                {
                    ParameterType = DbType.Double,
                    IsOptional = false
                },
                new FunctionParameter
                {
                    ParameterType = DbType.Int32,
                    IsOptional = true
                }
            }
        };

        public static readonly ScalarFunction LOG10 = new ScalarFunction
        {
            Name = "LOG10",
            ReturnType = DbType.Double,
            Parameters = new ReadOnlyList<FunctionParameter>
            {
                new FunctionParameter
                {
                    ParameterType = DbType.Double,
                    IsOptional = false
                }
            }
        };

        public static readonly ScalarFunction PI = new ScalarFunction
        {
            Name = "PI",
            ReturnType = DbType.Double,
            Parameters = new ReadOnlyList<FunctionParameter>()
        };

        public static readonly ScalarFunction POWER = new ScalarFunction
        {
            Name = "POWER",
            ReturnType = DbType.Double,
            Parameters = new ReadOnlyList<FunctionParameter>
            {
                new FunctionParameter
                {
                    ParameterType = DbType.Double,
                    IsOptional = false
                },
                new FunctionParameter
                {
                    ParameterType = DbType.VarNumeric,
                    IsOptional = false
                }
            }
        };

        public static readonly ScalarFunction RADIANS = new ScalarFunction
        {
            Name = "RADIANS",
            ReturnType = DbType.VarNumeric,
            Parameters = new ReadOnlyList<FunctionParameter>
            {
                new FunctionParameter
                {
                    ParameterType = DbType.VarNumeric,
                    IsOptional = false
                }
            }
        };

        public static readonly ScalarFunction RAND = new ScalarFunction
        {
            Name = "RAND",
            ReturnType = DbType.Double,
            Parameters = new ReadOnlyList<FunctionParameter>
            {
                new FunctionParameter
                {
                    ParameterType = DbType.Int32,
                    IsOptional = true
                }
            }
        };

        public static readonly ScalarFunction ROUND = new ScalarFunction
        {
            Name = "ROUND",
            ReturnType = DbType.VarNumeric,
            Parameters = new ReadOnlyList<FunctionParameter>
            {
                new FunctionParameter
                {
                    ParameterType = DbType.VarNumeric,
                    IsOptional = false
                },
                new FunctionParameter
                {
                    ParameterType = DbType.Int32,
                    IsOptional = false
                }
            }
        };

        public static readonly ScalarFunction SIGN = new ScalarFunction
        {
            Name = "SIGN",
            ReturnType = DbType.VarNumeric,
            Parameters = new ReadOnlyList<FunctionParameter>
            {
                new FunctionParameter
                {
                    ParameterType = DbType.VarNumeric,
                    IsOptional = false
                }
            }
        };

        public static readonly ScalarFunction SIN = new ScalarFunction
        {
            Name = "SIN",
            ReturnType = DbType.Double,
            Parameters = new ReadOnlyList<FunctionParameter>
            {
                new FunctionParameter
                {
                    ParameterType = DbType.Double,
                    IsOptional = false
                }
            }
        };

        public static readonly ScalarFunction SQRT = new ScalarFunction
        {
            Name = "SQRT",
            ReturnType = DbType.Double,
            Parameters = new ReadOnlyList<FunctionParameter>
            {
                new FunctionParameter
                {
                    ParameterType = DbType.Double,
                    IsOptional = false
                }
            }
        };

        public static readonly ScalarFunction SQUARE = new ScalarFunction
        {
            Name = "SQUARE",
            ReturnType = DbType.Double,
            Parameters = new ReadOnlyList<FunctionParameter>
            {
                new FunctionParameter
                {
                    ParameterType = DbType.Double,
                    IsOptional = false
                }
            }
        };

        public static readonly ScalarFunction TAN = new ScalarFunction
        {
            Name = "TAN",
            ReturnType = DbType.Double,
            Parameters = new ReadOnlyList<FunctionParameter>
            {
                new FunctionParameter
                {
                    ParameterType = DbType.Double,
                    IsOptional = false
                }
            }
        };

        public static readonly ScalarFunction ASCII = new ScalarFunction
        {
            Name = "ASCII",
            ReturnType = DbType.Int32,
            Parameters = new ReadOnlyList<FunctionParameter>
            {
                new FunctionParameter
                {
                    ParameterType = DbType.String,
                    IsOptional = false
                }
            }
        };

        public static readonly ScalarFunction CHAR = new ScalarFunction
        {
            Name = "ASCII",
            ReturnType = DbType.AnsiStringFixedLength,
            Parameters = new ReadOnlyList<FunctionParameter>
            {
                new FunctionParameter
                {
                    ParameterType = DbType.Byte,
                    IsOptional = false
                }
            }
        };

        public static readonly ScalarFunction CHARINDEX = new ScalarFunction
        {
            Name = "CHARINDEX",
            ReturnType = DbType.Int32,
            Parameters = new ReadOnlyList<FunctionParameter>
            {
                new FunctionParameter
                {
                    ParameterType = DbType.String,
                    IsOptional = false
                }
            }
        };
    }
}
