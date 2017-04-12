using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Data.OleDb;
using System.Data.SqlClient;

namespace DatabaseManagement
{
    public static class DatabaseManager
    {
        public static IReadOnlyCollection<IDatabase> AllDatabases { get; private set; }

        private static Dictionary<string, IDatabase> _nameIndexed, _connStrIndexed;

        static DatabaseManager()
        {
            Reload();
            
        }

        public static void Reload()
        {
            List<IDatabase> allDbs = new List<IDatabase>();
            foreach (ConnectionStringSettings conn in ConfigurationManager.ConnectionStrings)
            {
                if (testConnectionString(conn.ConnectionString))
                {
                    IDatabase db = new SqlServer.Database(conn.Name, conn.ConnectionString);
                    allDbs.Add(db);
                }
            }
            AllDatabases = new GenericCollection<IDatabase>(allDbs);
            _nameIndexed = allDbs.ToDictionary(db => db.Name);
            _connStrIndexed = allDbs.ToDictionary(db => db.ConnectionStringName);
        }

        static bool testConnectionString(string connectionString)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }

        public static IDatabase GetDatabaseByName(string dbName)
        {
            if (dbName == null)
            {
                throw new ArgumentNullException();
            }
            IDatabase ret;
            _nameIndexed.TryGetValue(dbName, out ret);
            return ret;
        }

        public static IDatabase GetDatabaseByConnectionStringName(string connectionStringName)
        {
            IDatabase ret;
            if (!_connStrIndexed.TryGetValue(connectionStringName, out ret))
            {
                if (ConfigurationManager.ConnectionStrings[connectionStringName] == null)
                {
                    throw new ArgumentException("No connection string setting named \"" + connectionStringName + "\" defined.", "connectionStringName");
                }
                else
                {
                    throw new ArgumentException("Connection string setting \"" + connectionStringName + "\" is defined, but no IDatabase could be resolved.", "connectionStringName");
                }
            }
            return ret;
        }

        public static ISchemaObject GetObjectByQualifiedName(string name)
        {
            if (AllDatabases.Count == 1)
            {
                return AllDatabases.Single().GetObjectByName(name);
            }

            return null;
        }
    }
}
