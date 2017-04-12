using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseManagement.SqlServer
{
    public sealed class ForeignKeyConstraint : TableConstraint, IForeignKeyConstraint
    {
        public Table ReferencedTable { get; internal set; }

        public IReadOnlyList<Column> ForeignKeyColumns { get; internal set; }

        public IReadOnlyList<Column> ReferencedColumns { get; internal set; }

        public ForeignKeyRule UpdateRule { get; internal set; }

        public ForeignKeyRule DeleteRule { get; internal set; }

        IReadOnlyList<IColumn> IForeignKeyConstraint.ForeignKeyColumns
        {
            get { return ForeignKeyColumns; }
        }

        IReadOnlyList<IColumn> IForeignKeyConstraint.ReferencedColumns
        {
            get { return ReferencedColumns; }
        }

        ITable IForeignKeyConstraint.ReferencedTable
        {
            get { return ReferencedTable; }
        }

        public override SchemaObjectType Type
        {
            get { return SchemaObjectType.ForeignKeyConstraint; }
        }

        internal ForeignKeyConstraint() { }
    }
}
