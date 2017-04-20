using DatabaseManagement;
using DatabaseManagement.SQL;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace BB
{
    internal class TableBoundClassManager : ClassManager
    {
        //public string TableName { get; internal set; }

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
        private RowMetadata _metadata;
        private DatabaseRepository _repository;
        //private ColumnPropertyManager[] _insertOutputProperties;
        private TableAttribute _tableAttribute;
        //private ColumnAccessExpression[] _columnsToSelect

        internal override IObjectRepository Repository
        {
            get { return _repository; }
        }

        public object GetColumnValue(IColumn col, object obj)
        {
            //ColumnPropertyManager manager = 
            ObjectExtender extender = ObjectExtender.GetExtender(obj);
            return ((DatabaseDataRow)extender.DataSource)[col];
        }

        protected override IObjectDataSource FetchByPrimaryKey(object primaryKey)
        {
            object[] pkArray = KeyConverter.GetArray(primaryKey);
            RowMetadata metadata = _metadata;
            SelectStatement selectStatement = metadata.BuildSelectStatement(buildIdentifierExpression(pkArray));

            //SelectStatement selectStatement = Statement.SelectFrom(Expression.Table(Table),)
            DatabaseDataRow[] rows = _repository.EnumerateRows(selectStatement, metadata).Take(2).ToArray();
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

        /*private SelectStatement buildSelectStatement(ConditionalExpression whereExpression)
        {
            RowMetadata metadata = _metadata;
            return metadata.BuildSelectStatement(whereExpression);
        }*/

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
            RowMetadata metadata = new RowMetadata();
            int position = 0;
            foreach (IColumn column in orderedByOrdinal)
            {
                metadata.AddSelectColumn(column, position++);
            }
            lock (this)
            {
                _primaryKeyColumns = pkColumns;
                UsedColumns = orderedByOrdinal;
                JoinedProperties = ValidManagedProperties.OfType<JoinedPropertyManager>().ToList();
                _metadata = metadata;
                _repository = (DatabaseRepository)Repository;
            }
        }

        /*private static ConditionalExpression columnEqualsParameterExpression(IColumn column, string parameterName)
        {
            ParameterExpression paramExpr = Expression.Parameter(parameterName, column.DbType);
            ColumnAccessExpression columnExpr = Expression.Column(column);
            return Expression.Equal(columnExpr, paramExpr);
        }*/

        /*public void AssignInsertOutputValues(object[] values)
        {
            int index = 0;
            foreach (ColumnPropertyManager propManager in _insertOutputProperties)
            {
                //propManager.SetValue()
            }
        }*/

        public override IEnumerable<IObjectDataSource> EnumerateData()
        {
            throw new NotImplementedException();
            //return _repository.EnumerateRows(
        }

        public override IEnumerable<IObjectDataSource> EnumerateData(System.Linq.Expressions.Expression expression)
        {
            throw new NotImplementedException();
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

        /*public void CreateInsertCommand(IDbCommand command, object obj)
        {
            List<KeyValuePair<IColumn, object>> insertValues = new List<KeyValuePair<IColumn, object>>();
            foreach (TableBoundTypePropertyManager manager in ValidManagedProperties)
            {
                object fieldVal = manager.GetFieldValue(obj);
                manager.AppendInsert(insertValues, fieldVal);
            }
            IColumn[] insertColumns = insertValues.Select(kvp => kvp.Key).ToArray();
            object[] values = insertValues.Select(kvp => kvp.Value).ToArray();
            string[] parameterNames = new string[values.Length];
            for (int i = 0; i < parameterNames.Length; i++)
            {
                string parameterName = "@value" + i;
                parameterNames[i] = parameterName;
                DatabaseRepository.AddParameter(command, parameterName, values[i]);
            }
            InsertStatement insertStatement;
            if (HasInsertOutput)
            {
                insertStatement = Statement.InsertInto(Table, insertColumns, parameterNames.Select(s => Expression.Parameter(s, DbType.Object)), InsertOutputColumns);
            }
            else
            {
                insertStatement = Statement.InsertInto(Table, insertColumns, parameterNames.Select(s => Expression.Parameter(s, DbType.Object)));
            }
            command.CommandText = insertStatement.ToString();

            //action_addInsertParameters(obj, command);
        }*/

        private ConditionalExpression buildIdentifierExpression(object obj)
        {
            object primaryKey = GetPrimaryKey(obj);
            object[] pkArray = KeyConverter.GetArray(primaryKey);
            return buildIdentifierExpression(pkArray);
        }

        private ConditionalExpression buildIdentifierExpression(object[] primaryKey)
        {
            IColumn[] pkColumns = _primaryKeyColumns;

            ConditionalExpression ret = Expression.Equal(Expression.Column(pkColumns[0]), Expression.Constant(primaryKey[0]));
            for (int i = 1; i < pkColumns.Length; i++)
            {
                ret = Expression.AndAlso(ret, Expression.Equal(Expression.Column(pkColumns[i]), Expression.Constant(primaryKey[i])));
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
            return Statement.Update(Table, updateValues.Select(kvp => new KeyValuePair<IColumn, ScalarExpression>(kvp.Key, Expression.Constant(kvp.Value))), buildIdentifierExpression(changeTracker.Object));
        }

        /*public void CreateUpdateCommand(IDbCommand command, ObjectChangeTracker changeTracker)
        {
            List<KeyValuePair<IColumn, object>> updateValues = new List<KeyValuePair<IColumn, object>>();
            foreach (var kvp in changeTracker.PropertyValues)
            {
                ((TableBoundTypePropertyManager)kvp.Key).AppendUpdate(updateValues, kvp.Value);
            }
            List<KeyValuePair<IColumn, ScalarExpression>> updateExpressions = new List<KeyValuePair<IColumn, ScalarExpression>>();
            int paramNum = 0;
            foreach (var kvp in updateValues)
            {
                IColumn column = kvp.Key;
                string paramName = "@value" + paramNum;
                updateExpressions.Add(new KeyValuePair<IColumn, ScalarExpression>(column, Expression.Parameter(paramName, column.DbType)));
                DatabaseRepository.AddParameter(command, paramName, kvp.Value);
                paramNum++;
            }
            UpdateStatement updateStatement = Statement.Update(Table, updateExpressions, _identifierExpression);
            addPrimaryKeyParameters(changeTracker.Object, command);
            command.CommandText = updateStatement.ToString();
        }*/

        public DeleteStatement BuildDeleteStatement(object obj)
        {
            return Statement.DeleteFrom(Table, buildIdentifierExpression(obj));
        }

        /*public void CreateDeleteCommand(IDbCommand command, object obj)
        {
            command.CommandText = _deleteStatement.ToString();
            addPrimaryKeyParameters(obj, command);
        }*/

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

        /*private void addPrimaryKeyParameters(object obj, IDbCommand command)
        {
            object primaryKey = GetPrimaryKey(obj);
            object[] asArray = KeyConverter.GetArray(obj);
            for (int i = 0; i < asArray.Length; i++)
            {
                DatabaseRepository.AddParameter(command, "pk_" + i, asArray[i]);
            }
        }*/

        /*internal void PrepareInsert()
        {
            lock (_lock)
            {
                if (!_preparedForInsert)
                {
                    List<ColumnPropertyManager> insertColumns = new List<ColumnPropertyManager>();
                    foreach (ColumnPropertyManager mgr in ManagedProperties.OfType<ColumnPropertyManager>())
                    {
                        IColumn col = mgr.Column;
                        if (col == null)
                        {
                            col = Table.GetColumnByName(mgr.ColumnName);
                            mgr.Column = col;
                        }
                        if (col != null)
                        {
                            if (!col.IsIdentity)
                            {
                                insertColumns.Add(mgr);
                            }
                        }
                    }
                    insertColumns = insertColumns.OrderBy(c => c.Column.OrdinalPosition).ToList();
                    StringBuilder sb = new StringBuilder();
                    sb.Append("insert into " + Table + "(");
                    sb.Append(string.Join(",", insertColumns.Select(c => c.Column)));
                    sb.Append(") values (");

                    _insertCommand = "insert into " + Table + " (" + string.Join(",", insertColumns.Select(m => m.Column)) + ") values ( " + string.Join(",", insertColumns.Select(c => "@" + c.Column.Name)) + ")";

                    DynamicMethod dynMethod = new DynamicMethod("method_addInsertParameters", typeof(void), new[] { typeof(object), typeof(IDbCommand) }, typeof(TableBoundTypeManager), true);

                    ILGenerator ilGen = dynMethod.GetILGenerator();
                    ilGen.DeclareLocal(Type);
                    ilGen.Emit(OpCodes.Ldarg_0);
                    ilGen.Emit(OpCodes.Castclass, Type);
                    ilGen.Emit(OpCodes.Stloc_0);

                    foreach (ColumnPropertyManager mgr in insertColumns)
                    {
                        FieldInfo backingField = mgr.BackingField;
                        ilGen.Emit(OpCodes.Ldarg_1);
                        ilGen.Emit(OpCodes.Ldstr, "@" + mgr.Column.Name);
                        ilGen.Emit(OpCodes.Ldloc_0);
                        ilGen.Emit(OpCodes.Ldfld, backingField);
                        if (backingField.FieldType.IsValueType)
                        {
                            ilGen.Emit(OpCodes.Box, backingField.FieldType);
                        }
                        ilGen.Emit(OpCodes.Call, method_createParameter);
                    }
                    ilGen.Emit(OpCodes.Ret);

                    action_addInsertParameters = (Action<object, IDbCommand>)dynMethod.CreateDelegate((typeof(Action<object, IDbCommand>)));

                    _preparedForInsert = true;
                }
            }
        }*/

        //private Action<object, IDbCommand> action_addInsertParameters;
        //private Action<object, IDbCommand> action_addPrimaryKeyParameters;

        /*public void AddPrimaryKeyParamerters(object obj, IDbCommand command)
        {
            action_addPrimaryKeyParameters(obj, command);
        }*/

        //private static readonly MethodInfo method_createParameter = ((Action<IDbCommand, string, object>)DatabaseRepository.AddParameter).Method;
    }
}
