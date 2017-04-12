using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseManagement
{
    public interface ISchemaObject
    {
        string Name { get; }
        IDatabase Database { get; }
        SchemaObjectType Type { get; }
    }
}
