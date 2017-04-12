using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseManagement
{
    internal static class Utility
    {
        public static void AssertNonNull(object parameter, string parameterName)
        {
            if (parameter == null)
            {
                throw new ArgumentNullException("parameterName");
            }
        }
        /*internal static DbType GetDbType(SqlDbType sqlDbType)
        {
            switch (sqlDbType)
            {
                case SqlDbType.BigInt:
                    return DbType.Int64;
                case SqlDbType.Binary:
                case SqlDbType.Image:
                case SqlDbType.Timestamp:
                case SqlDbType.VarBinary:
                    return DbType.Binary;
                case SqlDbType.Bit:
                    return DbType.Boolean;
                case SqlDbType.Char:
                    return DbType.AnsiStringFixedLength;
                case SqlDbType.Date:
                    return DbType.Date;
                case SqlDbType.DateTime:
                case SqlDbType.SmallDateTime:
                    return DbType.DateTime;
                case SqlDbType.DateTime2:
                    return DbType.DateTime2;
                case SqlDbType.DateTimeOffset:
                    return DbType.DateTimeOffset;
                case SqlDbType.Decimal:
                    return DbType.Decimal;
                case SqlDbType.Float:
                    return DbType.Double;
                case SqlDbType.Int:
                    return DbType.Int32;
                case SqlDbType.Money:
                case SqlDbType.SmallMoney:
                    return DbType.Currency;
                case SqlDbType.NChar:
                    return DbType.StringFixedLength;
                case SqlDbType.NText:
                case SqlDbType.NVarChar:
                case SqlDbType.Text:
                    return DbType.String;
                case SqlDbType.Real:
                    return DbType.Single;
                case SqlDbType.SmallInt:
                    return DbType.Int16;
                case SqlDbType.VarChar:
                    return DbType.AnsiString;
                case SqlDbType.Time:
                    return DbType.Time;
                case SqlDbType.TinyInt:
                    return DbType.Byte;
                case SqlDbType.UniqueIdentifier:
                    return DbType.Guid;
                case SqlDbType.Xml:
                    return DbType.Xml;
                case SqlDbType.Structured:
                case SqlDbType.Udt:
                case SqlDbType.Variant:
                    return DbType.Object;
                default:
                    throw new ArgumentException("sqlDbType");
            }
        }

        internal static DbType GetDbType(TypeCode type)
        {
            switch (type)
            {
                case TypeCode.Boolean:
                    return DbType.Boolean;
                case TypeCode.Byte:
                    return DbType.Byte;
                case TypeCode.Char:
                    return DbType.StringFixedLength;
                case TypeCode.DateTime:
                    return DbType.DateTime;
                case TypeCode.Decimal:
                    return DbType.Decimal;
                case TypeCode.Double:
                    return DbType.Double;
                case TypeCode.Int16:
                    return DbType.Int16;
                case TypeCode.Int32:
                    return DbType.Int32;
                case TypeCode.Int64:
                    return DbType.Int64;
                case TypeCode.Object:
                    return DbType.Object;
                case TypeCode.SByte:
                    return DbType.SByte;
                case TypeCode.Single:
                    return DbType.Single;
                case TypeCode.String:
                    return DbType.String;
                case TypeCode.UInt16:
                    return DbType.UInt16;
                case TypeCode.UInt32:
                    return DbType.UInt32;
                case TypeCode.UInt64:
                    return DbType.UInt64;
                default:
                    return DbType.Object;
            }
        }

        internal static DbType GetDbType(Type type)
        {
            TypeCode tc = Type.GetTypeCode(type);
            if (tc == TypeCode.Object)
            {
                if (type == typeof(byte[]))
                {
                    return DbType.Binary;
                }
                else if (type == typeof(DateTimeOffset))
                {
                    return DbType.DateTimeOffset;
                }
                else if (type == typeof(Guid))
                {
                    return DbType.Guid;
                }
                Type nullableType = Nullable.GetUnderlyingType(type);
                if (nullableType != null)
                {
                    return GetDbType(nullableType);
                }
            }
            return GetDbType(tc);
        }

        internal static DbType GetDbType(object value)
        {
            if (value == null)
            {
                return DbType.Object;
            }
            return GetDbType(value.GetType());
        }

        internal static TypeCode GetTypeCode(DbType dbType)
        {
            switch (dbType)
            {
                case DbType.AnsiString:
                case DbType.AnsiStringFixedLength:
                case DbType.String:
                case DbType.StringFixedLength:
                    return TypeCode.String;
                case DbType.Boolean:
                    return TypeCode.Boolean;
                case DbType.Byte:
                    return TypeCode.Byte;
                case DbType.Currency:
                case DbType.Decimal:
                case DbType.VarNumeric:
                    return TypeCode.Decimal;
                case DbType.Double:
                    return TypeCode.Double;
                case DbType.Int16:
                    return TypeCode.Int16;
                case DbType.Int32:
                    return TypeCode.Int32;
                case DbType.Int64:
                    return TypeCode.Int64;
                case DbType.SByte:
                    return TypeCode.SByte;
                case DbType.Single:
                    return TypeCode.Single;
                case DbType.UInt16:
                    return TypeCode.UInt16;
                case DbType.UInt32:
                    return TypeCode.UInt32;
                case DbType.UInt64:
                    return TypeCode.UInt64;
                default:
                    return TypeCode.Object;
            }
        }

        public static Type GetType(DbType dbType)
        {
            TypeCode typeCode = GetTypeCode(dbType);
            if (typeCode == TypeCode.Object)
            {
                switch (dbType)
                {
                    case DbType.Guid:
                        return typeof(Guid);
                    case DbType.DateTimeOffset:
                        return typeof(DateTimeOffset);
                    case DbType.Binary:
                        return typeof(byte[]);
                    default:
                        return typeof(object);
                }
            }
            return GetType(typeCode);
        }*/
    }
}
