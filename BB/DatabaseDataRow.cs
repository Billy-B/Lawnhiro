using DatabaseManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace BB
{
    internal class DatabaseDataRow : IObjectDataSource
    {
        private QueryNode _node;

        private object[] _data;

        public object this[IColumn column]
        {
            get
            {
                int index;
                if (!_node.TryGetColumnIndex(column, out index))
                {
                    throw new InvalidOperationException("Column " + column + " was not in query metadata?");
                }
                return _data[index];
            }
        }

        public bool TryGetColumnValue(IColumn col, out object value)
        {
            int index;
            if (_node.TryGetColumnIndex(col, out index))
            {
                value = _data[index];
                return true;
            }
            else
            {
                value = null;
                return false;
            }
        }

        public bool IsColumnDefined(IColumn column)
        {
            int index;
            return _node.TryGetColumnIndex(column, out index);
        }

        private DatabaseDataRow() { }

        internal DatabaseDataRow(QueryNode node, IDataReader reader)
        {
            _node = node;
            _data = new object[reader.FieldCount];
            reader.GetValues(_data);
        }

        public DatabaseDataRow GetJoinedRow(JoinedPropertyManager joinedProperty)
        {
            QueryNode childNode = _node.GetChildNode(joinedProperty);
            if (childNode == null)
            {
                return null;
            }
            else
            {
                return new DatabaseDataRow { _data = this._data, _node = childNode };
            }
        }

        public object PrimaryKey
        {
            get
            {
                throw new NotImplementedException();
            }
        }
    }
}
