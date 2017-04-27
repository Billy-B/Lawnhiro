using DatabaseManagement;
using DatabaseManagement.SQL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BB
{
    /*internal class RowMetadata
    {
        internal Dictionary<IColumn, int> ColumnMapper { get; private set; }
        internal Dictionary<JoinedPropertyManager, RowMetadata> JoinedRowMapper { get; private set; }
        internal ITable Table { get; set; }
        internal string Alias { get; set; }
        internal IList<IColumn> SelectedColumns { get; private set; }

        internal RowMetadata()
        {
            ColumnMapper = new Dictionary<IColumn, int>();
            JoinedRowMapper = new Dictionary<JoinedPropertyManager, RowMetadata>();
            SelectedColumns = new List<IColumn>();
        }

        internal RowMetadata(IEnumerable<IColumn> columns) : this()
        {
            int index = 0;
            foreach (IColumn column in columns)
            {
                ColumnMapper.Add(column, index++);
                SelectedColumns.Add(column);
            }
        }

        public void AddSelectColumn(IColumn column, int position)
        {
            ColumnMapper.Add(column, position);
            SelectedColumns.Add(column);
        }

        public SelectStatement BuildSelectStatement(ConditionalExpression whereExpression)
        {
            return Statement.SelectFrom(Expression.Table(Table), SelectedColumns.Select(c => Expression.Column(c)), whereExpression);
        }
    }*/
}
