using DatabaseManagement;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using DbExpression = DatabaseManagement.SQL.Expression;
using DbConditionalExpression = DatabaseManagement.SQL.ConditionalExpression;
using ScalarExpression = DatabaseManagement.SQL.ScalarExpression;

namespace BB
{
    internal class DatabaseRepository : IObjectRepository
    {
        private const string BASE_ALIAS = "base";

        private static readonly Dictionary<IDatabase, DatabaseRepository> _mapper = new Dictionary<IDatabase, DatabaseRepository>();
        
        internal static DatabaseRepository GetRepository(IDatabase database)
        {
            lock (_mapper)
            {
                DatabaseRepository ret;
                if (!_mapper.TryGetValue(database, out ret))
                {
                    ret = new DatabaseRepository(database);
                    _mapper.Add(database, ret);
                }
                return ret;
            }
        }

        private DatabaseRepository(IDatabase database)
        {
            Database = database;
        }

        public IDatabase Database { get; private set; }

        /*internal static void AddParameter(IDbCommand command, string paramName, object value)
        {
            IDbDataParameter param = command.CreateParameter();
            param.ParameterName = paramName;
            param.Value = value ?? DBNull.Value;
            command.Parameters.Add(param);
        }*/

        public static DbConditionalExpression GenerateConditional(Expression expr)
        {
            BinaryExpression binExp;
            switch (expr.NodeType)
            {
                case ExpressionType.AndAlso:
                    binExp = (BinaryExpression)expr;
                    return DbExpression.AndAlso(GenerateConditional(binExp.Left), GenerateConditional(binExp.Right));
                case ExpressionType.OrElse:
                    binExp = (BinaryExpression)expr;
                    return DbExpression.OrElse(GenerateConditional(binExp.Left), GenerateConditional(binExp.Right));
                case ExpressionType.Equal:
                    binExp = (BinaryExpression)expr;
                    return DbExpression.Equal(GenerateScalar(binExp.Left), GenerateScalar(binExp.Right));
                case ExpressionType.NotEqual:
                    binExp = (BinaryExpression)expr;
                    return DbExpression.NotEqual(GenerateScalar(binExp.Left), GenerateScalar(binExp.Right));
                case ExpressionType.LessThan:
                    binExp = (BinaryExpression)expr;
                    return DbExpression.LessThan(GenerateScalar(binExp.Left), GenerateScalar(binExp.Right));
                case ExpressionType.LessThanOrEqual:
                    binExp = (BinaryExpression)expr;
                    return DbExpression.LessThanOrEqual(GenerateScalar(binExp.Left), GenerateScalar(binExp.Right));
                case ExpressionType.GreaterThan:
                    binExp = (BinaryExpression)expr;
                    return DbExpression.GreaterThan(GenerateScalar(binExp.Left), GenerateScalar(binExp.Right));
                case ExpressionType.GreaterThanOrEqual:
                    binExp = (BinaryExpression)expr;
                    return DbExpression.GreaterThanOrEqual(GenerateScalar(binExp.Left), GenerateScalar(binExp.Right));
                case ExpressionType.MemberAccess:
                    MemberExpression membExpr = (MemberExpression)expr;
                    PropertyInfo prop = membExpr.Member as PropertyInfo;
                    if (prop == null)
                    {
                        throw new InvalidOperationException("Member " + membExpr.Member + " is not a queryable property.");
                    }
                    PropertyManager propManager = PropertyManager.GetManager(prop);
                    if (propManager == null)
                    {
                        throw new InvalidOperationException("Property " + prop + " is not a managed property.");
                    }
                    ColumnPropertyManager colMgr = propManager as ColumnPropertyManager;
                    if (colMgr != null)
                    {
                        return DbExpression.Equal(DbExpression.Column(colMgr.Column), DbExpression.Constant(true));
                    }
                    throw new NotImplementedException();
                case ExpressionType.Constant:
                    ConstantExpression constExpr = (ConstantExpression)expr;
                    if ((bool)constExpr.Value)
                    {
                        return DbExpression.Equal(DbExpression.Constant(1), DbExpression.Constant(1)); // 1 = 1 (true)
                    }
                    else
                    {
                        return DbExpression.Equal(DbExpression.Constant(1), DbExpression.Constant(0)); // 1 = 0 (false)
                    }
                case ExpressionType.Not:
                    UnaryExpression unaryExp = (UnaryExpression)expr;
                    return DbExpression.Not(GenerateConditional(unaryExp.Operand));
                default:
                    throw new NotImplementedException();
            }
        }

        public static ScalarExpression GenerateScalar(Expression expr)
        {
            BinaryExpression binExp;
            UnaryExpression unaryExp;
            switch (expr.NodeType)
            {
                case ExpressionType.Add:
                    binExp = (BinaryExpression)expr;
                    return DbExpression.Add(GenerateScalar(binExp.Left), GenerateScalar(binExp.Right));
                case ExpressionType.Subtract:
                    binExp = (BinaryExpression)expr;
                    return DbExpression.Subtract(GenerateScalar(binExp.Left), GenerateScalar(binExp.Right));
                case ExpressionType.Multiply:
                    binExp = (BinaryExpression)expr;
                    return DbExpression.Multiply(GenerateScalar(binExp.Left), GenerateScalar(binExp.Right));
                case ExpressionType.Divide:
                    binExp = (BinaryExpression)expr;
                    return DbExpression.Divide(GenerateScalar(binExp.Left), GenerateScalar(binExp.Right));
                case ExpressionType.Modulo:
                    binExp = (BinaryExpression)expr;
                    return DbExpression.Modulo(GenerateScalar(binExp.Left), GenerateScalar(binExp.Right));
                case ExpressionType.And:
                    binExp = (BinaryExpression)expr;
                    return DbExpression.And(GenerateScalar(binExp.Left), GenerateScalar(binExp.Right));
                case ExpressionType.Or:
                    binExp = (BinaryExpression)expr;
                    return DbExpression.Or(GenerateScalar(binExp.Left), GenerateScalar(binExp.Right));
                case ExpressionType.ExclusiveOr:
                    binExp = (BinaryExpression)expr;
                    return DbExpression.ExclusiveOr(GenerateScalar(binExp.Left), GenerateScalar(binExp.Right));
                case ExpressionType.Constant:
                    ConstantExpression constExp = (ConstantExpression)expr;
                    return DbExpression.Constant(constExp.Value);
                case ExpressionType.MemberAccess:
                    MemberExpression membExp = (MemberExpression)expr;
                    PropertyInfo prop = membExp.Member as PropertyInfo;
                    if (prop != null)
                    {
                        if (prop.GetMethod.IsStatic)
                        {
                            return DbExpression.Constant(prop.GetMethod.Invoke(null, null));
                        }
                        else
                        {
                            PropertyManager manager = PropertyManager.GetManager(prop);
                            if (manager == null)
                            {
                                throw new InvalidOperationException("Property " + prop + " is not a managed property.");
                            }
                            ColumnPropertyManager colMgr = manager as ColumnPropertyManager;
                            if (colMgr != null)
                            {
                                return DbExpression.Column(colMgr.Column);
                            }
                            else
                            {
                                throw new NotImplementedException();
                            }
                        }
                    }
                    else
                    {
                        FieldInfo field = (FieldInfo)membExp.Member;
                        if (field.IsStatic)
                        {
                            return DbExpression.Constant(field.GetValue(null));
                        }
                        else
                        {
                            throw new InvalidOperationException("Non - static field?");
                        }
                    }
                case ExpressionType.Negate:
                    unaryExp = (UnaryExpression)expr;
                    return DbExpression.Negate(GenerateScalar(unaryExp.Operand));
                case ExpressionType.UnaryPlus:
                    unaryExp = (UnaryExpression)expr;
                    return GenerateScalar(unaryExp.Operand);
                case ExpressionType.OnesComplement:
                    unaryExp = (UnaryExpression)expr;
                    return DbExpression.OnesCompliment(GenerateScalar(unaryExp.Operand));
                default:
                    throw new NotImplementedException();
            }
        }

        public void Commit(IList<IGrouping<Type, object>> objectsToInsert, IList<IGrouping<Type, object>> objectsToDelete, IList<IGrouping<Type, ObjectChangeTracker>> objectsToUpdate)
        {
            using (IDbConnection conn = Database.GetConnection())
            {
                conn.Open();
                using (IDbTransaction trans = conn.BeginTransaction())
                {
                    using (IDbCommand command = conn.CreateCommand())
                    {
                        command.Transaction = trans;
                        if (objectsToInsert != null)
                        {
                            foreach (var grouping in objectsToInsert)
                            {
                                TableBoundClassManager manager = (TableBoundClassManager)TypeManager.GetManager(grouping.Key);
                                if (manager.HasInsertOutput)
                                {
                                    RowMetadata outputMetadata = new RowMetadata(manager.InsertOutputColumns);
                                    
                                    foreach (object obj in grouping)
                                    {
                                        DatabaseManagement.SQL.InsertStatement insertStatement = manager.BuildInsertStatement(obj);
                                        ObjectExtender extender = ObjectExtender.GetExtender(obj);
                                        insertStatement.Prepare(command);
                                        using (IDataReader reader = command.ExecuteReader())
                                        {
                                            reader.Read();
                                            extender.DataSource = new DatabaseDataRow(outputMetadata, reader);
                                        }
                                        command.Parameters.Clear();
                                    }
                                }
                                else
                                {
                                    foreach (object obj in grouping)
                                    {
                                        DatabaseManagement.SQL.InsertStatement insertStatement = manager.BuildInsertStatement(obj);
                                        insertStatement.Prepare(command);
                                        command.ExecuteNonQuery();
                                        command.Parameters.Clear();
                                    }
                                }
                            }
                        }
                        if (objectsToUpdate != null)
                        {
                            foreach (var grouping in objectsToUpdate)
                            {
                                TableBoundClassManager manager = (TableBoundClassManager)TypeManager.GetManager(grouping.Key);
                                foreach (ObjectChangeTracker changeTracker in grouping)
                                {
                                    DatabaseManagement.SQL.UpdateStatement updateStatement = manager.BuildUpdateStatement(changeTracker);
                                    updateStatement.Prepare(command);
                                    command.ExecuteNonQuery();
                                    command.Parameters.Clear();
                                }
                            }
                        }
                        try
                        {
                            trans.Commit();
                        }
                        catch
                        {
                            trans.Rollback();
                            throw;
                        }
                    }
                }
            }
        }

        public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
        {
            return new Queryable<TElement> { Expression = expression, Provider = this };
        }

        public IQueryable CreateQuery(Expression expression)
        {
            return QueryHelpers.CreateQuery(this, expression);
        }

        public TResult Execute<TResult>(Expression expression)
        {
            
            Type type = typeof(TResult);
            Type exprType = expression.Type;
            Type elemType = QueryHelpers.GetElementType(type);
            switch (expression.NodeType)
            {
                case ExpressionType.Constant:
                    if (exprType.IsGenericType)
                    {
                        if (exprType.GetGenericTypeDefinition() == typeof(Queryable<>))
                        {
                            TableBoundClassManager manager = (TableBoundClassManager)TypeManager.GetManager(elemType);
                            return (TResult)QueryHelpers.GenericCast(enumerateFullTable(manager), elemType);
                        }
                    }
                    throw new NotSupportedException();
                case ExpressionType.Call:
                    MethodCallExpression methodCallExpr = (MethodCallExpression)expression;
                    Expression[] arguments = methodCallExpr.Arguments.ToArray();
                    MethodInfo method = methodCallExpr.Method;
                    if (method.IsGenericMethod)
                    {
                        MethodInfo genericDef = method.GetGenericMethodDefinition();
                        if (genericDef == QueryableMethods.Where)
                        {

                        }
                        else
                        {
                            throw new NotSupportedException("Unqueryable method");
                        }
                    }
                    throw new NotSupportedException();
                default:
                    throw new NotSupportedException();
            }
        }

        public object Execute(Expression expression)
        {
            throw new NotImplementedException();
        }

        private IEnumerable enumerateFullTable(TableBoundClassManager manager)
        {
            RowMetadata metadata = GenerateMetadata(manager);

            string selectStatement = generateSelectStatement(metadata);
            return enumerateType(manager, metadata);
        }

        public IEnumerable<DatabaseDataRow> EnumerateRows(DatabaseManagement.SQL.SelectStatement selectStatement, RowMetadata metadata)
        {
            using (IDbConnection connection = Database.GetConnection())
            {
                connection.Open();
                using (IDbCommand command = connection.CreateCommand())
                {
                    selectStatement.Prepare(command);
                    using (IDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            yield return new DatabaseDataRow(metadata, reader);
                        }
                    }
                }
            }
        }

        public IEnumerable<DatabaseDataRow> EnumerateRows(DatabaseManagement.SQL.SelectStatement selectStatement)
        {
            RowMetadata defaultMetadata = new RowMetadata();
            if (selectStatement.SelectedFields == null)
            {
                throw new NotSupportedException("SelectedFields null?");
            }
            int position = 0;
            foreach (var expression in selectStatement.SelectedFields)
            {
                DatabaseManagement.SQL.ColumnAccessExpression colAccess = expression as DatabaseManagement.SQL.ColumnAccessExpression;
                if (colAccess == null)
                {
                    throw new NotSupportedException("Non-ColumnAccessExpression in selected fields, cannot build default metadata.");
                }
                defaultMetadata.AddSelectColumn(colAccess.Column, position++);
            }
            //defaultMetadata.
            using (IDbConnection connection = Database.GetConnection())
            {
                connection.Open();
                using (IDbCommand command = connection.CreateCommand())
                {
                    selectStatement.Prepare(command);
                    using (IDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            yield return new DatabaseDataRow(defaultMetadata, reader);
                        }
                    }
                }
            }
        }

        /*public IEnumerable Enumerate(TableBoundClassManager typeManager, DatabaseManagement.SQL.SelectStatement selectStatement, IEnumerable<KeyValuePair<string, object>> parameters, RowMetadata metadata)
        {
            using (IDbConnection connection = Database.GetConnection())
            {
                connection.Open();
                using (IDbCommand command = connection.CreateCommand())
                {
                    foreach (var kvp in parameters)
                    {
                        AddParameter(command, kvp.Key, kvp.Value);
                    }
                    using (IDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            DatabaseDataRow row = new DatabaseDataRow(metadata, reader);
                            yield return typeManager.GetAttachedObject(row);
                        }
                    }
                    //command.Parameters.
                }
            }
        }*/

        internal static RowMetadata GenerateMetadata(TableBoundClassManager manager)
        {
            int columnIndex = 0;
            JoinedPropertyManager[] joinedProperties = manager.JoinedProperties.Where(p => p.ShouldSelectJoined).ToArray();
            IColumn[] selectableColumns = manager.UsedColumns.ToArray();
            RowMetadata ret = new RowMetadata();
            ret.Alias = BASE_ALIAS;
            ret.Table = manager.Table;
            foreach (IColumn col in selectableColumns)
            {
                ret.ColumnMapper.Add(col, columnIndex++);
            }

            foreach (JoinedPropertyManager joinedProp in joinedProperties)
            {
                RowMetadata joinedMetadata = new RowMetadata();
                joinedMetadata.Alias = joinedProp.Property.Name;
                ret.JoinedRowMapper.Add(joinedProp, joinedMetadata);
                AppendJoinedRecursive(joinedMetadata, joinedProp, ref columnIndex);
            }
            return ret;
        }

        internal static void AppendJoinedRecursive(RowMetadata metadata, JoinedPropertyManager joinedProp, ref int columnIndex)
        {
            string alias = metadata.Alias;
            metadata.Alias = joinedProp.Property.Name;
            TableBoundClassManager referencedManager = joinedProp.ReferencedManager;
            metadata.Table = referencedManager.Table;
            IList<IColumn> fkColumns = joinedProp.ForeignKeyColumns;
            IList<IColumn> referencedColumns = joinedProp.ReferencedColumns;
            JoinedPropertyManager[] nextJoinedProps = referencedManager.JoinedProperties.Where(p => p.ShouldSelectJoined).ToArray();

            IColumn[] selectableColumns = referencedManager.UsedColumns.Union(referencedColumns).Union(nextJoinedProps.SelectMany(p => p.ForeignKeyColumns)).ToArray();
            foreach (IColumn col in selectableColumns)
            {
                metadata.ColumnMapper.Add(col, columnIndex++);
            }
            foreach (JoinedPropertyManager next in nextJoinedProps.Where(p => p.ShouldSelectJoined))
            {
                RowMetadata joinedMetadata = new RowMetadata();
                joinedMetadata.Alias = alias + "." + next.Property.Name;
                metadata.JoinedRowMapper.Add(next, joinedMetadata);
                AppendJoinedRecursive(joinedMetadata, next, ref columnIndex);
            }
        }

        private static string generateSelectStatement(RowMetadata metadata)
        {
            StringBuilder selectedColumnsClause = new StringBuilder("select ");
            StringBuilder selectedTablesClause = new StringBuilder("from " + metadata.Table + " as " + metadata.Alias + "\n");
            //appendColumns(selectedColumnsClause, metadata.ColumnMapper.Keys);
            appendJoinedRowsRecursive(selectedColumnsClause, selectedTablesClause, metadata);
            selectedColumnsClause.Remove(selectedColumnsClause.Length - 3, 1);
            return selectedColumnsClause.ToString() + selectedTablesClause.ToString();
        }

        private static void appendColumns(StringBuilder selectedColumnsClause, IEnumerable<IColumn> columns)
        {
            foreach (IColumn col in columns)
            {
                selectedColumnsClause.AppendLine(col + ",");
            }
        }

        private static void appendColumns(StringBuilder selectedColumnsClause, RowMetadata metadata)
        {
            string alias = metadata.Alias;
            foreach (IColumn col in metadata.ColumnMapper.Keys)
            {
                selectedColumnsClause.AppendLine(alias + "." + col.Name + ",");
            }
        }

        private static void appendJoinedRowsRecursive(StringBuilder selectedColumnsClause, StringBuilder selectedTablesClause, RowMetadata metadata)
        {
            appendColumns(selectedColumnsClause, metadata);
            string baseAlias = metadata.Alias;
            foreach (var joinedRowMapping in metadata.JoinedRowMapper)
            {
                JoinedPropertyManager prop = joinedRowMapping.Key;
                RowMetadata joinedMetadata = joinedRowMapping.Value;
                string alias = joinedMetadata.Alias;
                selectedTablesClause.AppendLine(" inner join " + joinedMetadata.Table + " as " + alias + " on");
                selectedTablesClause.AppendLine(baseAlias + "." + prop.ForeignKeyColumns[0].Name + " = " + alias + "." + prop.ReferencedColumns[0].Name);
                for (int i = 1; i < prop.ForeignKeyColumns.Count; i++)
                {
                    selectedTablesClause.AppendLine("and " + baseAlias + "." + prop.ForeignKeyColumns[i].Name + " = " + alias + "." + prop.ReferencedColumns[i].Name);
                }
                appendJoinedRowsRecursive(selectedColumnsClause, selectedTablesClause, joinedMetadata);
            }
        }

        /*private string generateSelectStatement(ITable table, IEnumerable<IColumn> columns)
        {
            return "select " + string.Join(",", columns) + " from " + table;
        }*/

        private IEnumerable enumerateType(TableBoundClassManager manager, RowMetadata metadata)
        {
            string selectCommand = generateSelectStatement(metadata);

            using (IDbConnection conn = Database.GetConnection())
            {
                conn.Open();
                using (IDbCommand command = conn.CreateCommand())
                {
                    command.CommandText = selectCommand;
                    using (IDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            yield return manager.GetAttachedObject(new DatabaseDataRow(metadata, reader));
                            //reader.
                            //yield return mgr.GetUninitializedObject()
                        }
                    }
                }
            }
        }

        //private DatabaseManagement.SQL.
    }
}
