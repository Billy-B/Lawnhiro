using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseManagement.SQL
{
    public class ConstantExpression : ScalarExpression
    {
        public object Value { get; private set; }

        private DbType _type;

        internal override ScalarExpression Dispatch(ExpressionVisitor visitor)
        {
            return visitor.VisitConstant(this);
        }

        internal ConstantExpression(object value)
        {
            Value = value;
            _type = TypeConverter.GetDbType(value);
        }

        internal ConstantExpression(object value, DbType dbType)
        {
            Type conversionType = TypeConverter.GetType(dbType);
            Value = Convert.ChangeType(value, conversionType);
        }

        public override DbType DbType
        {
            get { return _type; }
        }

        public override ExpressionType Type
        {
            get { return ExpressionType.Constant; }
        }

        public override string ToString()
        {
            object value = Value;
            if (value == null || value == DBNull.Value)
            {
                return "NULL";
            }
            else
            {
                switch(_type)
                {
                    case DbType.AnsiString:
                    case DbType.AnsiStringFixedLength:
                    case DbType.String:
                    case DbType.StringFixedLength:
                        return "'" + ((string)value).Replace("'", "''") + "'";
                    case DbType.Binary:
                        return byteArrayToString((byte[])value);
                    case DbType.Boolean:
                        return ((bool)value) ? "1" : "0";
                    case DbType.Date:
                        return "'" + ((DateTime)value).ToShortDateString() + "'";
                    case DbType.DateTime:
                    case DbType.DateTime2:
                    case DbType.DateTimeOffset:
                    case DbType.Guid:
                    case DbType.Time:
                        return "'" + value.ToString() + "'";
                    default:
                        return value.ToString();
                }
            }
        }
        private static string byteArrayToString(byte[] bytes)
        {
            StringBuilder hex = new StringBuilder(bytes.Length * 2);
            foreach (byte b in bytes)
                hex.AppendFormat("{0:x2}", b);
            return hex.ToString();
        }
    }
}
