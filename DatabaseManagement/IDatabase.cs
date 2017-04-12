using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace DatabaseManagement
{
    public interface IDatabase
    {
        string Name { get; }
        string ConnectionString { get; }
        string ConnectionStringName { get; }
        IReadOnlyCollection<ITable> Tables { get; }
        ISchemaObject GetObjectByName(string name);
        IReadOnlyCollection<IForeignKeyConstraint> ForeignKeys { get; }
        IReadOnlyCollection<IUniqueConstraint> UniqueConstraints { get; }
        IReadOnlyCollection<IView> Views { get; }
        IDbConnection GetConnection();
    }
}
