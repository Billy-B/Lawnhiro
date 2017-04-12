using DatabaseManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BB
{
    internal class AssemblyManager
    {
        private static readonly Dictionary<Assembly, AssemblyManager> _mapper = new Dictionary<Assembly, AssemblyManager>();

        public static AssemblyManager GetManager(Assembly assembly)
        {
            return _mapper[assembly];
        }

        internal AssemblyManager()
        {
            
        }

        public static AssemblyManager Create(Assembly assembly)
        {
            ManagedAssemblyAttribute att = assembly.GetCustomAttribute<ManagedAssemblyAttribute>();
            
            AssemblyManager ret;
            if (att == null)
            {
                ret = new AssemblyManager();
            }
            else
            {
                ret = att.CreateManager();
            }
            
            _mapper.Add(assembly, ret);
            return ret;
        }

        private IDatabase _defaultDb;

        public Assembly Assembly { get; internal set; }

        public IDatabase DefaultDatabase
        {
            get
            {
                if (_defaultDb == null && DefaultDatabaseName != null)
                {
                    _defaultDb = DatabaseManager.GetDatabaseByName(DefaultDatabaseName);
                }
                return _defaultDb;
            }
        }

        public string DefaultDatabaseName { get; internal set; }

        public ITable GetTable(string name)
        {

            ITable ret = DatabaseManager.GetObjectByQualifiedName(name) as ITable;
            if (ret == null)
            {
                if (DefaultDatabase != null)
                {
                    ret = DefaultDatabase.GetObjectByName(name) as ITable;
                }
            }
            return ret;
        }
    }
}
