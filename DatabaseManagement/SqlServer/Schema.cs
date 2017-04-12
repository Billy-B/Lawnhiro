using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseManagement.SqlServer
{
    public sealed class Schema
    {
        public string Name { get; internal set; }

        public int Id { get; internal set; }

        public Database Database { get; internal set; }

        public IReadOnlyCollection<Table> Tables { get; internal set; }

        public IReadOnlyCollection<UniqueConstraint> UniqueConstraints { get; internal set; }

        public IReadOnlyCollection<ForeignKeyConstraint> ForeignKeys { get; internal set; }

        public IReadOnlyCollection<SchemaObject> AllObjects { get; internal set; }

        internal SchemaObject GetObjectOrNull(string name)
        {
            SchemaObject ret;
            _nameIndexed.TryGetValue(name.TrimBracketNotation().ToLowerInvariant(), out ret);
            return ret;
        }

        internal Dictionary<string, SchemaObject> _nameIndexed;

        internal Schema() { }

        public override string ToString()
        {
            return "[" + Name + "]";
        }
    }
}
