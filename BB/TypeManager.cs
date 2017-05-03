using DatabaseManagement;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BB
{
    internal abstract class TypeManager
    {
        protected static readonly Dictionary<Type, TypeManager> _mapper = new Dictionary<Type, TypeManager>();

        private bool _initialized;

        public Type Type { get; private set; }

        public AssemblyManager AssemblyManager { get; internal set; }

        internal abstract IObjectRepository Repository { get; }

        internal abstract void Initialize();

        public void EnsureInitialized()
        {
            lock (this)
            {
                if (!_initialized)
                {
                    Initialize();
                    _initialized = true;
                }
            }
        }

        public abstract IEnumerable EnumerateValues();

        public virtual IEnumerable QueryWhere(LambdaExpression expression)
        {
            object compiledDelegate = expression.Compile();
            IEnumerable cast = QueryHelpers.GenericCast(EnumerateValues(), Type);
            return (IEnumerable)EnumerableMethods.Where.MakeGenericMethod(Type).Invoke(null, new[] { cast, compiledDelegate });
        }

        public virtual object QueryFirst()
        {
            return EnumerateValues().Cast<object>().First();
        }

        public virtual object QueryFirstOrDefault()
        {
            return EnumerateValues().Cast<object>().FirstOrDefault();
        }

        public virtual object QueryLast()
        {
            return EnumerateValues().Cast<object>().Last();
        }

        public virtual object QueryLastOrDefault()
        {
            return EnumerateValues().Cast<object>().LastOrDefault();
        }

        public virtual object QuerySingle()
        {
            return EnumerateValues().Cast<object>().Single();
        }

        public virtual object QuerySingleOrDefault()
        {
            return EnumerateValues().Cast<object>().SingleOrDefault();
        }

        public virtual object QueryFirst(LambdaExpression expression)
        {
            return QueryWhere(expression).Cast<object>().First();
        }

        public virtual object QueryFirstOrDefault(LambdaExpression expression)
        {
            return QueryWhere(expression).Cast<object>().FirstOrDefault();
        }

        public virtual object QueryLast(LambdaExpression expression)
        {
            return QueryWhere(expression).Cast<object>().Last();
        }

        public virtual object QueryLastOrDefault(LambdaExpression expression)
        {
            return QueryWhere(expression).Cast<object>().LastOrDefault();
        }

        public virtual object QuerySingle(LambdaExpression expression)
        {
            return QueryWhere(expression).Cast<object>().Single();
        }

        public virtual object QuerySingleOrDefault(LambdaExpression expression)
        {
            return QueryWhere(expression).Cast<object>().SingleOrDefault();
        }

        public virtual bool QueryAny()
        {
            return EnumerateValues().Cast<object>().Any();
        }

        public virtual int QueryCount()
        {
            return EnumerateValues().Cast<object>().Count();
        }

        public virtual int QueryCount(LambdaExpression expression)
        {
            object compiledDelegate = expression.Compile();
            IEnumerable cast = QueryHelpers.GenericCast(EnumerateValues(), Type);
            return (int)EnumerableMethods.CountMatchExpression.MakeGenericMethod(Type).Invoke(null, new[] { cast, compiledDelegate });
        }

        public virtual bool QueryAny(LambdaExpression expression)
        {
            return QueryWhere(expression).Cast<object>().Any();
        }

        public virtual bool QueryAll(LambdaExpression expression)
        {
            object compiledDelegate = expression.Compile();
            IEnumerable cast = QueryHelpers.GenericCast(EnumerateValues(), Type);
            return (bool)EnumerableMethods.All.MakeGenericMethod(Type).Invoke(null, new[] { cast, compiledDelegate });
        }

        //public abstract TResult Execute<TResult>(MethodCallExpression expression);
        /*{
            Type type = typeof(TResult);
            Type exprType = expression.Type;
            Type elemType = QueryHelpers.GetElementType(type);
            switch (expression.NodeType)
            {
                case ExpressionType.Constant:
                    ConstantExpression constExpr = (ConstantExpression)expression;
                    object value = constExpr.Value;
                    IQueryable asQueryable = value as IQueryable;
                    if (exprType.IsGenericType)
                    {
                        if (exprType.GetGenericTypeDefinition() == typeof(Queryable<>))
                        {
                            return (TResult)QueryHelpers.GenericCast(EnumerateValues(), Type);
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
                            //Expression predicateExpression = methodCallExpr.
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
        }*/

        internal PropertyManager GetNonPrimativeValueTypeManager(int index)
        {
            EnsureInitialized();
            return _nonPrimativeValueTypeManagers[index];
        }

        private PropertyManager[] _nonPrimativeValueTypeManagers;

        internal static void Initialize(RuntimeTypeHandle handle)
        {
            Type managedType = Type.GetTypeFromHandle(handle);
            TypeManager typeManager = GetManager(managedType);
            typeManager.Initialize();
        }

        public static bool IsManaged(Type type)
        {
            return _mapper.ContainsKey(type);
        }

        public static TypeManager Create(Type type, AssemblyManager assemblyManager, ManagedTypeBaseAttribute att)
        {
            TypeManager ret = att.CreateTypeManager(type);
            ret.AssemblyManager = assemblyManager;
            ret.Type = type;
            _mapper.Add(type, ret);
            return ret;
        }

        public static TypeManager GetManager(Type type)
        {
            TypeManager ret;
            _mapper.TryGetValue(type, out ret);
            return ret;
        }
    }

    /*public class TypeManager<T>
        where T : class
    {
        internal static TypeManager<T> Instance;

        public T LookupByPrimaryKey(object primaryKey)
        {
            throw new NotImplementedException();
        }

        static TypeManager()
        {
            Type type = typeof(T);
            object[] uninheritedAttributes = type.GetCustomAttributes(false);
            object[] assemblyAttributes = type.Assembly.GetCustomAttributes(false);
            TableAttribute tblAtt = uninheritedAttributes.OfType<TableAttribute>().SingleOrDefault();
            if (tblAtt != null)
            {
                string tableName = tblAtt.Name;
                DatabaseAttribute dbAtt = assemblyAttributes.OfType<DatabaseAttribute>().SingleOrDefault();
                ISchemaObject obj = DatabaseManager.GetObjectByQualifiedName(tableName);
                if (obj == null)
                {
                    if (dbAtt == null)
                    {
                        throw new ArgumentException("Cannot resolve table using qualified name \"" + tableName + "\". If no DatabaseAttribute is defined on the assembly, must supple fully qualified name for the table.");
                    }
                    else
                    {
                        string dbName = dbAtt.Name;
                        IDatabase db = DatabaseManager.GetDatabaseByName(dbName);
                        if (db == null)
                        {
                            throw new ArgumentException("No database \"" + dbName + "\" found.");
                        }
                        obj = db.GetObjectByName(tableName);
                        if (obj == null)
                        {
                            throw new ArgumentException("No table \"" + tableName + "\" in database " + db);
                        }
                    }
                }
                ITable table = obj as ITable;
                if (table == null)
                {
                    throw new ArgumentException("Schema object " + tableName + " is not a table.");
                }
                Instance = new DatabaseTableBoundTypeManager<T>(table);
            }
        }

        internal Cache<T> Cache;
        
    }*/
}
