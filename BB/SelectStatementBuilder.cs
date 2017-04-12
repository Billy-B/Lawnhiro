using DatabaseManagement;
using DatabaseManagement.SQL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BB
{
    internal class SelectStatementBuilder
    {
        private List<IColumn> _columnsToSelect = new List<IColumn>();
        private ITable _table;
        private TableValuedExpression _fromExpression;
        private Dictionary<JoinedPropertyManager, TableValuedExpression> _innerJoinMapper = new Dictionary<JoinedPropertyManager, TableValuedExpression>();

        internal SelectStatementBuilder(ITable table)
        {
            _table = table;
            _fromExpression = Expression.Table(table);
        }

        public SelectStatement Build()
        {
            TableValuedExpression expression = Expression.Table(_table);
            throw new NotImplementedException();
            //return Statement.SelectFrom()
        }

        public void AddColumn(IColumn column)
        {
            if (!_columnsToSelect.Contains(column))
            {
                _columnsToSelect.Add(column);
            }
        }
    }
}
