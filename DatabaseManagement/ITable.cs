using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseManagement
{
    public interface ITable : ISchemaObject
    {
        IReadOnlyList<IColumn> Columns { get; }
        IUniqueConstraint PrimaryKey { get; }
        bool HasPrimaryKey { get; }
        bool HasIdentityColumn { get; }
        IColumn IdentityColumn { get; }
        IReadOnlyCollection<ITableConstraint> Constraints { get; }
        IReadOnlyCollection<IForeignKeyConstraint> ForeignKeys { get; }
        IReadOnlyCollection<IUniqueConstraint> UniqueConstraints { get; }
        IColumn GetColumnByName(string name);
    }
}
