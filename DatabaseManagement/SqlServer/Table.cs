using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DatabaseManagement.SQL;

namespace DatabaseManagement.SqlServer
{
    public class Table : SchemaObject, ITable
    {
        internal Table() { }

        public Column IdentityColumn { get; internal set; }

        public UniqueConstraint PrimaryKey { get; internal set; }

        public bool HasPrimaryKey
        {
            get { return PrimaryKey != null; }
        }

        public bool HasIdentityColumn
        {
            get { return IdentityColumn != null; }
        }

        public IReadOnlyCollection<UniqueConstraint> UniqueConstraints { get; internal set; }

        public IReadOnlyCollection<ForeignKeyConstraint> ForeignKeys { get; internal set; }

        public IReadOnlyCollection<TableConstraint> Constraints { get; internal set; }

        public TableColumnCollection Columns { get; internal set; }

        public override SchemaObjectType Type
        {
            get { return SchemaObjectType.Table; }
        }

        IReadOnlyList<IColumn> ITable.Columns
        {
            get { return Columns; }
        }

        IReadOnlyCollection<ITableConstraint> ITable.Constraints
        {
            get { return Constraints; }
        }

        IReadOnlyCollection<IUniqueConstraint> ITable.UniqueConstraints
        {
            get { return UniqueConstraints; }
        }

        IReadOnlyCollection<IForeignKeyConstraint> ITable.ForeignKeys
        {
            get { return ForeignKeys; }
        }

        IColumn ITable.IdentityColumn
        {
            get { return IdentityColumn; }
        }

        IUniqueConstraint ITable.PrimaryKey
        {
            get { return PrimaryKey; }
        }

        IColumn ITable.GetColumnByName(string name)
        {
            return Columns.GetColumnOrNull(name);
        }
    }
}
