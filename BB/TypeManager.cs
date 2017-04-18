using DatabaseManagement;
using System;
using System.Collections.Generic;
using System.Linq;
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
