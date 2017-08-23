using DatabaseManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DatabaseManagement.SQL;
using System.Diagnostics;

namespace BB
{
    internal class QueryNode
    {
        private QueryMetadata _metadata;
        internal Dictionary<JoinedPropertyManager, QueryNode> _indexedByJoinedProp = new Dictionary<JoinedPropertyManager, QueryNode>();
        internal Dictionary<IColumn, int> _indexMapper = new Dictionary<IColumn, int>();
        internal ListInternal<IColumn> _columns;
        internal ListInternal<QueryNode> _nodes = new ListInternal<QueryNode>();
        internal TableBoundClassManager _manager;

        public int Depth { get; internal set; }

        public QueryNode Parent { get; internal set; }

        public bool IncludedInSelect { get; internal set; }

        public JoinedPropertyManager JoinedProperty { get; internal set; }

        public TableExpression TableExpression { get; internal set; }

        public IList<QueryNode> ChildNodes
        {
            get { return _nodes; }
        }

        public IList<IColumn> Columns
        {
            get { return _columns; }
        }

        public QueryNode GetOrCreateChildNode(JoinedPropertyManager joinedProp)
        {
            return _metadata.GetOrCreateChildNode(this, joinedProp);
        }

        internal QueryNode GetChildNode(JoinedPropertyManager joinedProp)
        {
            return _indexedByJoinedProp.GetValueOrDefault(joinedProp);
        }

        internal QueryNode(QueryMetadata metadata)
        {
            _metadata = metadata;
        }

        internal ColumnAccessExpression AccessColumn(IColumn column)
        {
            /*if (_metadata.Finalized)
            {
                throw new InvalidOperationException("Metadata is finalized.");
            }*/
            return Expression.Column(column, TableExpression);
        }

        /*internal void AddColumn(IColumn column, int index)
        {
            _columns.Add(column);
            _indexMapper.Add(column, index);
        }*/

        public bool TryGetColumnIndex(IColumn column, out int index)
        {
            if (!_metadata.ReadOnly)
            {
                throw new InvalidOperationException("Metadata has not been finalized.");
            }
            return _indexMapper.TryGetValue(column, out index);
        }

        /*internal TableValuedExpression BuildExpression()
        {
            TableValuedExpression ret = Expression.Table(Table);
            foreach (var kvp in _indexedByJoinedProp)
            {
                if (kvp.Key.ColumnsAllowNull)
                {
                    ret = Expression.InnerJoin(ret, kvp.Value.BuildExpression());
                }
                else
                {
                    ret = Expression.LeftOuterJoin(ret, kvp.Value.BuildExpression());
                }
            }
            return ret;
        }*/
    }
}
