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

        internal override ScalarExpression Dispatch(ExpressionVisitor visitor)
        {
            return visitor.VisitColumnAccess(this);
        }

        public override ExpressionType Type
        {
            get { return ExpressionType.ColumnAccess; }
        }

        public override string ToString()
        {
            return Column.ToString();
        }

        internal ColumnAccessExpression() { }
    }
}
