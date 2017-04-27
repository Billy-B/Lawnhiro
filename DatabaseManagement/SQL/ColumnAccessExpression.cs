using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseManagement.SQL
{
    public class ColumnAccessExpression : FieldAccessExpression
    {
        public new IColumn Column { get; set; }

        public override DbType DbType
        {
            get { return Column.DbType; }
        }

        public override ExpressionType Type
        {
            get { return ExpressionType.ColumnAccess; }
        }

        public override string ToString()
        {
            return Column.ToString();
        }

        internal override IEnumerable<Expression> EnumerateSubExpressions()
        {
            return Enumerable.Empty<Expression>();
        }

        internal ColumnAccessExpression() { }
    }
}
