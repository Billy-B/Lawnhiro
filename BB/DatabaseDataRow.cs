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
        private RowMetadata _metadata;

        private object[] _data;

        public object this[IColumn col]
        {
            get
            {
                return _data[_metadata.ColumnMapper[col]];
            }
        }

        public bool TryGetColumnValue(IColumn col, out object value)
        {
            int index;
            if (_metadata.ColumnMapper.TryGetValue(col, out index))
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

        public bool IsColumnDefined(IColumn col)
        {
            return _metadata.ColumnMapper.ContainsKey(col);
        }

        private DatabaseDataRow() { }

        internal DatabaseDataRow(RowMetadata metadata, IDataReader reader)
        {
            _metadata = metadata;
            _data = new object[reader.FieldCount];
            reader.GetValues(_data);
        }

        public DatabaseDataRow GetJoinedRow(JoinedPropertyManager joinedProperty)
        {
            RowMetadata joinedMetadata;
            if (_metadata.JoinedRowMapper.TryGetValue(joinedProperty, out joinedMetadata))
            {
                return new DatabaseDataRow { _data = this._data, _metadata = joinedMetadata };
            }
            else
            {
                return null;
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
