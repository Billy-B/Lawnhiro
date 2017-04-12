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

        internal static void AddParameter(IDbCommand command, string paramName, object value)
        {
            IDbDataParameter param = command.CreateParameter();
            param.ParameterName = paramName;
            param.Value = value ?? DBNull.Value;
            command.Parameters.Add(param);
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
                                    foreach (object obj in grouping)
                                    {
                                        manager.CreateInsertCommand(command, obj);
                                        using (IDataReader reader = command.ExecuteReader())
                                        {

                                        }
                                        command.Parameters.Clear();
                                    }
                                }
                                else
                                {
                                    foreach (object obj in grouping)
                                    {
                                        manager.CreateInsertCommand(command, obj);
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
                                    manager.CreateUpdateCommand(command, changeTracker);
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

        public IEnumerable<DatabaseDataRow> Enumerate(RowMetadata metadata, IEnumerable<KeyValuePair<string, object>> parameters, DatabaseManagement.SQL.ConditionalExpression whereExpression)
        {
            using (IDbConnection connection = Database.GetConnection())
            {
                connection.Open();
                using (IDbCommand command = connection.CreateCommand())
                {
                    command.CommandText = metadata.BuildSelectStatement(whereExpression).ToString();
                    foreach (var kvp in parameters)
                    {
                        AddParameter(command, kvp.Key, kvp.Value);
                    }
                    using (IDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            yield return new DatabaseDataRow(metadata, reader);
                        }
                    }
                    //command.Parameters.
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

        public IEnumerable Enumerate(TableBoundClassManager typeManager, DatabaseManagement.SQL.SelectStatement selectStatement, IEnumerable<KeyValuePair<string, object>> parameters, RowMetadata metadata)
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
        }

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

            /*foreach (JoinedPropertyManager joinedProp in joinedProperties)
            {
                RowMetadata joinedMetadata = new RowMetadata();
                joinedMetadata.Alias = joinedProp.Property.Name;
                ret.JoinedRowMapper.Add(joinedProp, joinedMetadata);
                AppendJoinedRecursive(joinedMetadata, joinedProp, ref columnIndex);
            }*/
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
