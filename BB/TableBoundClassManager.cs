using DatabaseManagement;
using DatabaseManagement.SQL;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace BB
{
    internal class TableBoundClassManager : ClassManager
    {
        internal TableBoundClassManager(TableAttribute attribute)
        {
            _tableAttribute = attribute;
        }

        public ITable Table { get; private set; }

        public IList<IColumn> UsedColumns { get; private set; }

        public bool HasInsertOutput { get; private set; }

        public new IList<TableBoundTypePropertyManager> ValidManagedProperties { get; private set; }

        public IList<JoinedPropertyManager> JoinedProperties { get; private set; }

        public IList<IColumn> InsertOutputColumns { get; private set; }

        private IColumn[] _primaryKeyColumns;
        private DatabaseRepository _repository;
        private TableAttribute _tableAttribute;
        private QueryMetadata _defaultMetadata;

        internal override IObjectRepository Repository
        {
            get { return _repository; }
        }

        public object GetColumnValue(IColumn col, object obj)
        {
            ObjectExtender extender = ObjectExtender.GetExtender(obj);
            return ((DatabaseDataRow)extender.DataSource)[col];
        }

        protected override IObjectDataSource FetchByPrimaryKey(object primaryKey)
        {
            object[] pkArray = KeyConverter.GetArray(primaryKey);
            QueryMetadata metadata = _defaultMetadata;
            DatabaseDataRow[] rows = _repository.EnumerateRows(metadata, buildQueryIdentifierExpression(metadata, pkArray)).Take(2).ToArray();
            if (rows.Length == 0)
            {
                return null;
            }
            else if (rows.Length == 1)
            {
                return rows[0];
            }
            else
            {
                throw new ArgumentException("Multiple rows with this primary key!");
            }
        }

        internal override void OnInitialize()
        {
            string tableName = _tableAttribute.Name ?? Type.Name;
            Table = AssemblyManager.GetTable(tableName);
            if (Table == null)
            {
                throw new ArgumentException("Cannot find table \"" + tableName + "\".");
            }
            HasInsertOutput = Table.HasIdentityColumn;
            if (HasInsertOutput)
            {
                InsertOutputColumns = new IColumn[] { Table.IdentityColumn };
            }
            _repository = DatabaseRepository.GetRepository(Table.Database);
            foreach (TableBoundTypePropertyManager propMgr in ManagedProperties.OfType<TableBoundTypePropertyManager>())
            {
                propMgr.Table = Table;
            }
        }
        internal override void OnPropertiesInitialized()
        {
            ValidManagedProperties = base.ValidManagedProperties.Cast<TableBoundTypePropertyManager>().ToList();
            HashSet<IColumn> usedColumns = new HashSet<IColumn>();
            foreach (ColumnPropertyManager manager in ValidManagedProperties.OfType<ColumnPropertyManager>())
            {
                usedColumns.Add(manager.Column);
            }
            foreach (JoinedPropertyManager manager in ValidManagedProperties.OfType<JoinedPropertyManager>())
            {
                foreach (IColumn col in manager.ForeignKeyColumns)
                {
                    usedColumns.Add(col);
                }
            }
            IUniqueConstraint primaryKey = Table.PrimaryKey;
            if (primaryKey == null)
            {
                throw new NotSupportedException("Cannot bind to table " + Table + " because it does not have a primary key.");
            }
            IColumn[] pkColumns = primaryKey.ConstrainedColumns.ToArray();
            foreach (IColumn col in pkColumns)
            {
                usedColumns.Add(col);
            }
            List<IColumn> orderedByOrdinal = usedColumns.OrderBy(c => c.OrdinalPosition).ToList();
            IColumn[] insertColumns = orderedByOrdinal.Where(c => !c.IsIdentity).ToArray();
            //SelectStatement selectByPkStatement = Statement.SelectFrom(Expression.Table(Table), orderedByOrdinal.Select(c => Expression.Column(c)), 1, identifierExpr);
            //RowMetadata metadata = buildMetadata();

            lock (this)
            {
                _primaryKeyColumns = pkColumns;
                UsedColumns = orderedByOrdinal;
                JoinedProperties = ValidManagedProperties.OfType<JoinedPropertyManager>().ToList();
                //_metadata = metadata;
                _repository = (DatabaseRepository)Repository;
                QueryMetadata defaultMetadata = new QueryMetadata(this);
                defaultMetadata.Finish();
                _defaultMetadata = defaultMetadata;
            }
        }

        internal override IEnumerable<IObjectDataSource> EnumerateData()
        {
            QueryMetadata metadata = _defaultMetadata;
            return _repository.EnumerateRows(metadata);
        }

        public override System.Collections.IEnumerable QueryWhere(System.Linq.Expressions.LambdaExpression expression)
        {
            return base.QueryWhere(expression);

        }

        public InsertStatement BuildInsertStatement(object obj)
        {
            List<KeyValuePair<IColumn, object>> insertValues = new List<KeyValuePair<IColumn, object>>();
            foreach (TableBoundTypePropertyManager manager in ValidManagedProperties)
            {
                object fieldVal = manager.GetFieldValue(obj);
                manager.AppendInsert(insertValues, fieldVal);
            }
            IColumn[] insertColumns = insertValues.Select(kvp => kvp.Key).ToArray();
            ConstantExpression[] values = insertValues.Select(kvp => Expression.Constant(kvp.Value)).ToArray();
            if (HasInsertOutput)
            {
                return Statement.InsertInto(Table, insertColumns, values, InsertOutputColumns);
            }
            else
            {
                return Statement.InsertInto(Table, insertColumns, values);
            }
        }

        private ConditionalExpression buildNonQueryIdentifierExpression(object obj)
        {
            object primaryKey = GetPrimaryKey(obj);
            object[] pkArray = KeyConverter.GetArray(primaryKey);
            ConditionalExpression ret = Expression.Equal(Expression.Column(_primaryKeyColumns[0]), Expression.Constant(pkArray[0]));
            for(int i = 1; i < _primaryKeyColumns.Length; i++)
            {
                ret = Expression.AndAlso(ret, Expression.Equal(Expression.Column(_primaryKeyColumns[i]), Expression.Constant(pkArray[i])));
            }
            return ret;
            //return buildIdentifierExpression(_defaultMetadata, pkArray);
        }

        private ConditionalExpression buildQueryIdentifierExpression(QueryMetadata metadata, object[] primaryKey)
        {
            IColumn[] pkColumns = _primaryKeyColumns;

            ConditionalExpression ret = Expression.Equal(metadata.FirstNode.AccessColumn(pkColumns[0]), Expression.Constant(primaryKey[0]));
            for (int i = 1; i < pkColumns.Length; i++)
            {
                ret = Expression.AndAlso(ret, Expression.Equal(metadata.FirstNode.AccessColumn(pkColumns[i]), Expression.Constant(primaryKey[i])));
            }
            return ret;
        }

        public UpdateStatement BuildUpdateStatement(ObjectChangeTracker changeTracker)
        {
            List<KeyValuePair<IColumn, object>> updateValues = new List<KeyValuePair<IColumn, object>>();
            foreach (var kvp in changeTracker.PropertyValues)
            {
                ((TableBoundTypePropertyManager)kvp.Key).AppendUpdate(updateValues, kvp.Value);
            }
            return Statement.Update(Table, updateValues.Select(kvp => new KeyValuePair<IColumn, ScalarExpression>(kvp.Key, Expression.Constant(kvp.Value))), buildNonQueryIdentifierExpression(changeTracker.Object));
        }

        public DeleteStatement BuildDeleteStatement(object obj)
        {
            return Statement.DeleteFrom(Table, buildNonQueryIdentifierExpression(obj));
        }

        public override object CreatePrimaryKey(IObjectDataSource source)
        {
            DatabaseDataRow row = (DatabaseDataRow)source;
            object[] array = new object[_primaryKeyColumns.Length];
            for (int i = 0; i < array.Length; i++)
            {
                array[i] = row[_primaryKeyColumns[i]];
            }
            return KeyConverter.GetSystemKey(array);
        }
    }
}
