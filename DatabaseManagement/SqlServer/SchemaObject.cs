using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseManagement.SqlServer
{
    public abstract class SchemaObject : ISchemaObject
    {
        public int ObjectId { get; internal set; }

        public string Name { get; internal set; }

        public Schema Schema { get; internal set; }

        public DateTime CreatedDate { get; internal set; }

        public DateTime ModifyDate { get; internal set; }

        public abstract SchemaObjectType Type { get; }

        public string FullyQualifiedName
        {
            get { return "[" + Schema.Database.Name + "].[" + Schema.Name + "].[" + this.Name + "]"; }
        }

        internal SchemaObject() { }

        IDatabase ISchemaObject.Database
        {
            get { return Schema.Database; }
        }

        public override string ToString()
        {
            return FullyQualifiedName;
        }
    }
}
