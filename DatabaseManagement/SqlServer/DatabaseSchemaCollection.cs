using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseManagement.SqlServer
{
    /*public class DatabaseSchemaCollection : GenericCollection<Schema>
    {
        private Dictionary<int, Schema> _idMapper;
        private Dictionary<string, Schema> _nameMapper;

        public Database Database { get; private set; }

        internal DatabaseSchemaCollection(Database database, IList<Schema> list)
            : base(list)
        {
            this.Database = database;
            _idMapper = list.ToDictionary(s => s.Id);
            _nameMapper = list.ToDictionary(s => s.Name);
        }

        public Schema this[string schemaName]
        {
            get
            {
                Schema ret;
                if (!TryGetValue(schemaName, out ret))
                {
                    throw new ArgumentException("No Schema \"" + schemaName + "\" exists in database " + Database + ".", "schemaName");
                }
                return ret;
            }
        }

        public Schema GetById(int id)
        {
            Schema ret;
            _idMapper.TryGetValue(id, out ret);
            return ret;
        }

        public bool TryGetValue(string name, out Schema value)
        {
            return _nameMapper.TryGetValue(name.TrimBracketNotation(), out value);
        }
    }*/
}
