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

        internal override IEnumerable<Expression> EnumerateSubExpressions()
        {
            return Enumerable.Empty<Expression>();
        }
    }
}
