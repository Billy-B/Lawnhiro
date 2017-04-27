using DatabaseManagement;
using DatabaseManagement.SQL;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BB
{
    internal class QueryMetadata
    {
        const int MAX_DEPTH = 5;
        const int MAX_NODES = 10;

        public QueryNode FirstNode { get; private set; }

        public bool Finalized { get; private set; }

        internal ListInternal<ColumnAccessExpression> _columnExpressions = new ListInternal<ColumnAccessExpression>();

        public TableValuedExpression TableExpression { get; private set; }

        private object _lock = new object();

        public void Finalize()
        {
            lock (_lock)
            {
                int columnIndex = 0;

                ListInternal<ColumnAccessExpression> columnExpressions = new ListInternal<ColumnAccessExpression>();
                foreach (IColumn column in FirstNode._manager.UsedColumns)
                {
                    columnExpressions.Add(FirstNode.AccessColumn(column));
                    FirstNode._indexMapper.Add(column, columnIndex++);
                }
                QueryNode[] allChildNodes = FirstNode.ChildNodes.Flatten(n => n.ChildNodes).OrderBy(n => n.Depth).ToArray();
                if (allChildNodes.Length > MAX_NODES - 1)
                {
                    throw new NotSupportedException();
                }
                TableValuedExpression expression = FirstNode.TableExpression;
                if (allChildNodes.Any())
                {
                    int maxDepth = allChildNodes.Last().Depth;
                    if (maxDepth > MAX_DEPTH)
                    {
                        throw new NotSupportedException();
                    }

                    foreach (QueryNode node in allChildNodes)
                    {
                        node.IncludedInSelect = true;
                        JoinedPropertyManager joinedProp = node.JoinedProperty;
                        ConditionalExpression joinCondition = Expression.Equal(node.AccessColumn(joinedProp.ReferencedColumns[0]), node.Parent.AccessColumn(joinedProp.ForeignKeyColumns[0]));
                        for (int i = 1; i < joinedProp.ReferencedColumns.Count; i++)
                        {
                            joinCondition = Expression.AndAlso(joinCondition, Expression.Equal(node.AccessColumn(joinedProp.ReferencedColumns[i]), node.Parent.AccessColumn(joinedProp.ForeignKeyColumns[i])));
                        }
                        if (joinedProp.ColumnsAllowNull)
                        {
                            expression = Expression.LeftOuterJoin(expression, node.TableExpression, joinCondition);
                        }
                        else
                        {
                            expression = Expression.InnerJoin(expression, node.TableExpression, joinCondition);
                        }
                        for (int i = 0; i < joinedProp.ReferencedColumns.Count; i++)
                        {
                            IColumn referencedColumn = joinedProp.ReferencedColumns[i];
                            IColumn fkColumn = joinedProp.ForeignKeyColumns[i];
                            int parentIndex = node.Parent._indexMapper[fkColumn];
                            node._indexMapper.Add(referencedColumn, parentIndex);
                        }
                        foreach (IColumn column in node._manager.UsedColumns.Where(c => !joinedProp.ReferencedColumns.Contains(c)))
                        {
                            columnExpressions.Add(node.AccessColumn(column));
                            node._indexMapper.Add(column, columnIndex++);
                        }
                    }
                }
                _columnExpressions = columnExpressions;
                TableExpression = expression;
                Finalized = true;
            }
        }

        internal QueryNode GetOrCreateChildNode(QueryNode parent, JoinedPropertyManager joinedProp)
        {
            lock (_lock)
            {
                if (Finalized)
                {
                    throw new InvalidOperationException("Metadata is finalized, cannot add nodes.");
                }
                QueryNode ret;
                if (!parent._indexedByJoinedProp.TryGetValue(joinedProp, out ret))
                {
                    string alias = parent.TableExpression.Alias + "_" + joinedProp.Property.Name;
                    ret = new QueryNode(this)
                    {
                        Depth = parent.Depth + 1,
                        Parent = parent,
                        JoinedProperty = joinedProp,
                        _manager = joinedProp.ReferencedManager,
                        TableExpression = Expression.Table(joinedProp.Table, alias)
                    };
                }
                return ret;
            }
        }

        public IList<ColumnAccessExpression> ColumnExpressions
        {
            get { return _columnExpressions; }
        }

        internal QueryMetadata(TableBoundClassManager manager)
        {
            FirstNode = new QueryNode(this)
            {
                _manager = manager,
                TableExpression = Expression.Table(manager.Table, "BASE"),
                Depth = 1
            };
        }

        public static QueryMetadata SimpleColumnMetadata(IEnumerable<IColumn> columns)
        {
            QueryMetadata ret = new QueryMetadata();
            IColumn firstColumn = columns.FirstOrDefault();
            if (firstColumn == null)
            {
                throw new ArgumentException("Cannot be empty.", "columns");
            }
            QueryNode firstNode = new QueryNode(ret)
            {
                TableExpression = Expression.Table(firstColumn.Table)
            };
            int columnIndex = 0;
            ListInternal<ColumnAccessExpression> columnExpressions = new ListInternal<ColumnAccessExpression>();
            foreach (IColumn column in columns)
            {
                if (column.Table != firstColumn.Table)
                {
                    throw new ArgumentException("Columns must all belong to same table.", "columns");
                }
                firstNode._indexMapper.Add(column, columnIndex++);
            }
            ret.TableExpression = firstNode.TableExpression;
            ret.FirstNode = firstNode;
            ret.Finalized = true;
            ret._columnExpressions = columnExpressions;
            return ret;
        }

        private QueryMetadata() { }
    }
}
