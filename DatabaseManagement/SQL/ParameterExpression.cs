using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseManagement.SQL
{
    public class ParameterExpression : ScalarExpression
    {
        public IDataParameter Parameter { get; internal set; }

        internal override ScalarExpression Dispatch(ExpressionVisitor visitor)
        {
            return visitor.VisitParameter(this);
        }

        public override DbType DbType
        {
            get { return Parameter.DbType; }
        }

        public override ExpressionType Type
        {
            get { return ExpressionType.Parameter; }
        }

        internal ParameterExpression() { }

        public override string ToString()
        {
            return Parameter.ParameterName;
        }
    }
}
