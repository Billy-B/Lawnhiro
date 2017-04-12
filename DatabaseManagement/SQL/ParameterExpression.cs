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
        public string ParameterName { get; internal set; }

        public object Value { get; internal set; }

        internal DbType ParameterType { get; set; }

        public override DbType DbType
        {
            get { return ParameterType; }
        }

        public override ExpressionType Type
        {
            get { return ExpressionType.Parameter; }
        }

        internal ParameterExpression() { }

        public override string ToString()
        {
            return ParameterName;
        }

        internal override IEnumerable<Expression> EnumerateSubExpressions()
        {
            return Enumerable.Empty<Expression>();
        }
    }
}
