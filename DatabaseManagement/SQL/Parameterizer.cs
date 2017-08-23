using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseManagement.SQL
{
    internal class Parameterizer : ExpressionVisitor
    {
        public Parameterizer(IDbCommand command)
        {
            _command = command;
        }

        private Dictionary<object, IDbDataParameter> _parametersIndexedByValue = new Dictionary<object, IDbDataParameter>();
        private IDbCommand _command;
        private int _paramCounter;

        protected internal override ScalarExpression VisitConstant(ConstantExpression node)
        {
            IDbDataParameter parameter;
            if (!_parametersIndexedByValue.TryGetValue(node.Value, out parameter))
            {
                parameter = _command.CreateParameter();
                parameter.Value = node.Value;
                parameter.ParameterName = "@p_" + _paramCounter;
                _paramCounter++;
                _parametersIndexedByValue.Add(node.Value, parameter);
                _command.Parameters.Add(parameter);
            }
            return Expression.Parameter(parameter);
        }
    }
}
