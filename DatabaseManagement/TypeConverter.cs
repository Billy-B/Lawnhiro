using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseManagement
{
    public static class TypeConverter
    {
        private const DbType DB_UNDEFINED = (DbType)(-1);

        private const uint TYPECODE_MAX_VAL = 18;
        private const uint DBTYPE_MAX_VAL = 27;
        private const uint SQLDBTYPE_MAX_VAL = 34;

        private static readonly Type[] _typeCodeToTypeLUT =
        {
            null,
            typeof(object),
            typeof(DBNull),
            typeof(bool),
            typeof(char),
            typeof(sbyte),
            typeof(byte),
            typeof(short),
            typeof(ushort),
            typeof(int),
            typeof(uint),
            typeof(long),
            typeof(ulong),
            typeof(float),
            typeof(double),
            typeof(decimal),
            typeof(DateTime),
            null,
            typeof(string)
        };
        private static readonly Type[] _dbTypeToTypeLUT =
        {
            typeof(string),
            typeof(byte[]),
            typeof(byte),
            typeof(bool),
            typeof(decimal),
            typeof(DateTime),
            typeof(DateTime),
            typeof(decimal),
            typeof(double),
            typeof(Guid),
            typeof(short),
            typeof(int),
            typeof(long),
            typeof(object),
            typeof(sbyte),
            typeof(float),
            typeof(string),
            typeof(TimeSpan),
            typeof(ushort),
            typeof(uint),
            typeof(ulong),
            typeof(decimal),
            typeof(string),
            typeof(string),
            null,
            typeof(object),
            typeof(DateTime),
            typeof(DateTimeOffset)
        };

        private static readonly DbType[] _typeCodeToDbTypeLUT =
        {
            DB_UNDEFINED,
            DbType.Object,
            DB_UNDEFINED,
            DbType.Boolean,
            DbType.StringFixedLength,
            DbType.SByte,
            DbType.Byte,
            DbType.Int16,
            DbType.UInt16,
            DbType.Int32,
            DbType.UInt32,
            DbType.Int64,
            DbType.UInt64,
            DbType.Single,
            DbType.Double,
            DbType.Decimal,
            DbType.DateTime,
            DB_UNDEFINED,
            DbType.String
        };

        private static readonly DbType[] _sqlDbTypeToDbTypeLUT =
        {
            DbType.Int64,
            DbType.Binary,
            DbType.Boolean,
            DbType.AnsiStringFixedLength,
            DbType.DateTime,
            DbType.Decimal,
            DbType.Double,
            DbType.Binary,
            DbType.Int32,
            DbType.Decimal,
            DbType.StringFixedLength,
            DbType.String,
            DbType.String,
            DbType.Single,
            DbType.Guid,
            DbType.DateTime,
            DbType.Int16,
            DbType.Decimal,
            DbType.String,
            DbType.Binary,
            DbType.Byte,
            DbType.Binary,
            DbType.AnsiString,
            DbType.Object,
            DB_UNDEFINED,
            DbType.Xml,
            DB_UNDEFINED,
            DB_UNDEFINED,
            DB_UNDEFINED,
            DB_UNDEFINED,
            DB_UNDEFINED,
            DbType.Date,
            DbType.Time,
            DbType.DateTime2,
            DbType.DateTimeOffset
        };

        private static readonly Dictionary<Type, DbType> _typeToDbTypeMapper = new Dictionary<Type, DbType>
        {
            { typeof(bool), DbType.Boolean },
            { typeof(bool?), DbType.Boolean },
            { typeof(sbyte), DbType.SByte },
            { typeof(sbyte?), DbType.SByte },
            { typeof(byte), DbType.Byte },
            { typeof(byte?), DbType.Byte },
            { typeof(short), DbType.Int16 },
            { typeof(short?), DbType.Int16 },
            { typeof(ushort), DbType.UInt16 },
            { typeof(ushort?), DbType.UInt16 },
            { typeof(int), DbType.Int32 },
            { typeof(int?), DbType.Int32 },
            { typeof(uint), DbType.UInt32 },
            { typeof(uint?), DbType.UInt32 },
            { typeof(long), DbType.Int64 },
            { typeof(long?), DbType.Int64 },
            { typeof(ulong), DbType.UInt64 },
            { typeof(ulong?), DbType.UInt64 },
            { typeof(char), DbType.StringFixedLength },
            { typeof(char?), DbType.StringFixedLength },
            { typeof(float), DbType.Single },
            { typeof(float?), DbType.Single },
            { typeof(double), DbType.Double },
            { typeof(double?), DbType.Double },
            { typeof(decimal), DbType.Decimal },
            { typeof(decimal?), DbType.Decimal },
            { typeof(DateTime), DbType.DateTime },
            { typeof(DateTime?), DbType.DateTime },
            { typeof(DateTimeOffset), DbType.DateTimeOffset },
            { typeof(DateTimeOffset?), DbType.DateTimeOffset },
            { typeof(TimeSpan), DbType.Time },
            { typeof(TimeSpan?), DbType.Time },
            { typeof(Guid), DbType.Guid },
            { typeof(Guid?), DbType.Guid },
            { typeof(string), DbType.String },
            { typeof(byte[]), DbType.Binary },
            { typeof(System.Data.SqlTypes.SqlBinary), DbType.Binary }
        };

        public static Type GetType(TypeCode typeCode)
        {
            uint value = (uint)typeCode;
            if (value > TYPECODE_MAX_VAL || value == 17)
            {
                throw new InvalidEnumArgumentException();
            }
            return _typeCodeToTypeLUT[value];
        }

        public static Type GetType(DbType dbType)
        {
            uint value = (uint)dbType;
            if (value > DBTYPE_MAX_VAL || value == 24)
            {
                throw new InvalidEnumArgumentException();
            }
            return _dbTypeToTypeLUT[value];
        }

        public static DbType GetDbType(TypeCode typeCode)
        {
            uint value = (uint)typeCode;
            if (value > TYPECODE_MAX_VAL)
            {
                throw new InvalidEnumArgumentException();
            }
            DbType ret = _typeCodeToDbTypeLUT[value];
            if (ret == DB_UNDEFINED)
            {
                throw new ArgumentException();
            }
            return ret;
        }

        public static DbType GetDbType(SqlDbType sqlDbType)
        {
            uint value = (uint)sqlDbType;
            if (value > SQLDBTYPE_MAX_VAL)
            {
                throw new InvalidEnumArgumentException();
            }
            DbType ret = _sqlDbTypeToDbTypeLUT[value];
            if (ret == DB_UNDEFINED)
            {
                throw new ArgumentException();
            }
            return ret;
        }

        public static DbType GetDbType(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }
            DbType ret;
            if (!_typeToDbTypeMapper.TryGetValue(type, out ret))
            {
                ret = DbType.Object;
            }
            return ret;
        }

        public static DbType GetDbType(object value)
        {
            if (value == null)
            {
                return DbType.Object;
            }
            return GetDbType(value.GetType());
        }
    }
}
