using DatabaseManagement;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BB
{
    internal class ColumnPropertyManager : TableBoundTypePropertyManager
    {
        internal ColumnAttribute ColumnAttribute { get; set; }

        public IColumn Column { get; set; }

        private Type _propType;

        internal override void Initialize()
        {
            string columnName = ColumnAttribute.Name ?? Property.Name;
            IColumn column = Table.GetColumnByName(columnName);
            bool isValid;
            Type propType = Property.PropertyType;
            if (column == null)
            {
                _initErrorMessage = "No column \"" + columnName + " in table " + Table;
                isValid = false;
            }
            else
            {
                DbType dbType = column.DbType;
                if (propType == typeof(char) || propType == typeof(char?))
                {
                    if (isValidForCharProperty(column))
                    {
                        isValid = true;
                    }
                    else
                    {
                        isValid = false;
                        _initErrorMessage = "Cannot map column " + column + " to type " + propType + " because it is not char(1) or nchar(1)";
                    }
                }
                else
                {
                    if (isCompatible(propType, dbType))
                    {
                        isValid = true;
                    }
                    else
                    {
                        _initErrorMessage = "Cannot map column " + column + " to type " + propType + ".";
                        isValid = false;
                    }
                }
            }
            Type nullableUnderlyingType = Nullable.GetUnderlyingType(propType);
            if (nullableUnderlyingType != null)
            {
                propType = nullableUnderlyingType;
            }
            lock (this)
            {
                IsValid = isValid;
                Column = column;
                _propType = propType;
            }
        }
        public override void AppendUpdate(IList<KeyValuePair<IColumn, object>> updateValues, object newPropValue)
        {
            updateValues.Add(new KeyValuePair<IColumn, object>(Column, newPropValue));
        }

        public override void AppendInsert(IList<KeyValuePair<IColumn, object>> insertValues, object propValue)
        {
            if (!Column.IsIdentity)
            {
                insertValues.Add(new KeyValuePair<IColumn, object>(Column, propValue));
            }
        }

        private static bool isValidForCharProperty(IColumn column)
        {
            return (column.CharacterMaxLength == 1) && (column.DbType == DbType.AnsiStringFixedLength || column.DbType == DbType.StringFixedLength);
        }

        private static bool isCompatible(Type type, DbType dbType)
        {
            Type nullableUnderlyingType = Nullable.GetUnderlyingType(type);
            if (nullableUnderlyingType != null)
            {
                return isCompatible(nullableUnderlyingType, dbType);
            }
            if (type == typeof(Guid))
            {
                return dbType == DbType.Guid;
            }
            if (type == typeof(DateTimeOffset))
            {
                return dbType == DbType.DateTimeOffset;
            }
            if (type == typeof(TimeSpan))
            {
                return dbType == DbType.Time;
            }
            if (type == typeof(byte[]))
            {
                return dbType == DbType.Binary;
            }
            TypeCode typeCode = Type.GetTypeCode(type);
            switch (typeCode)
            {
                case TypeCode.Boolean:
                case TypeCode.Byte:
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                case TypeCode.SByte:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                    switch (dbType)
                    {
                        case DbType.Boolean:
                        case DbType.Byte:
                        case DbType.Int16:
                        case DbType.Int32:
                        case DbType.Int64:
                        case DbType.SByte:
                        case DbType.UInt16:
                        case DbType.UInt32:
                        case DbType.UInt64:
                        case DbType.VarNumeric:
                            return true;
                        default:
                            return false;
                    }
                case TypeCode.DateTime:
                    return dbType == DbType.Date
                        || dbType == DbType.DateTime
                        || dbType == DbType.DateTime2;
                case TypeCode.Decimal:
                    return dbType == DbType.Currency 
                        || dbType == DbType.Decimal 
                        || dbType == DbType.VarNumeric;
                case TypeCode.Double:
                case TypeCode.Single:
                    return dbType == DbType.Double 
                        || dbType == DbType.Single;
                case TypeCode.String:
                    return dbType == DbType.AnsiString
                        || dbType == DbType.AnsiStringFixedLength
                        || dbType == DbType.String
                        || dbType == DbType.StringFixedLength;
                default:
                    return false;
            }
        }

        public override object FetchValue(IObjectDataSource dataSource)
        {
            DatabaseDataRow row = (DatabaseDataRow)dataSource;
            object ret = row[Column];
            if (ret == DBNull.Value)
            {
                return null;
            }
            return Convert.ChangeType(ret, _propType);
        }
    }
}
