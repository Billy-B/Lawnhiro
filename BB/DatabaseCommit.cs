using DatabaseManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BB
{
    internal class DatabaseCommit
    {
        private IDatabase _database;

        public DatabaseCommit(IDatabase database)
        {
            _database = database;
        }

        //public void AddObjectsToInsert(IList<IGrouping<Type, object>> objectsToInsert)
    }
}
