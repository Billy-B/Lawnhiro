using DatabaseManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace BB
{
    internal class JoinedPropertyManager : TableBoundTypePropertyManager
    {
        public IList<IColumn> ReferencedColumns { get; internal set; }

        public IList<IColumn> ForeignKeyColumns { get; internal set; }

        public TableBoundClassManager ReferencedManager { get; internal set; }

        public bool ReferencesPrimaryKey { get; private set; }

        public bool AllowNull { get; internal set; }

        public bool ColumnsAllowNull { get; internal set; }

        public bool ShouldSelectJoined
        {
            get
            {
                return ReferencedManager.Cached;
            }
        }

        internal override void Initialize()
        {
            TableBoundClassManager referencedManager = TypeManager.GetManager(Property.PropertyType) as TableBoundClassManager;
            if (referencedManager == null)
            {
                IsValid = false;
                _initErrorMessage = "Type " + Property.PropertyType + " is not a table-managed type.";
            }
            else
            {
                ReferencedManager = referencedManager;
                IsValid = true;
            }
        }

        public override object FetchValue(IObjectDataSource dataSource)
        {
            DatabaseDataRow row = (DatabaseDataRow)dataSource;
            if (ReferencesPrimaryKey)
            {
                object[] array = new object[ForeignKeyColumns.Count];
                for (int i = 0; i < array.Length; i++)
                {
                    object fkColumnValue = row[ForeignKeyColumns[i]];
                    if (fkColumnValue == null)
                    {
                        return null;
                    }
                    array[i] = fkColumnValue;
                }
                object primaryKey = KeyConverter.GetSystemKey(array);
                return ReferencedManager.GetByPrimaryKey(primaryKey);
            }
            else
            {
                throw new NotSupportedException("Constraint does not reference primary key.");
            }
        }

        public override void AppendUpdate(IList<KeyValuePair<IColumn, object>> updateValues, object newPropValue)
        {
            throw new NotImplementedException();
        }

        public override void AppendInsert(IList<KeyValuePair<IColumn, object>> insertValues, object propValue)
        {
            if (propValue != null)
            {
                for (int i = 0; i < ReferencedColumns.Count; i++)
                {
                    insertValues.Add(new KeyValuePair<IColumn, object>(ForeignKeyColumns[i], ReferencedManager.GetColumnValue(ReferencedColumns[i], propValue)));
                }
            }
        }
    }
}
