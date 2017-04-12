using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseManagement.SqlServer
{
    public class UniqueConstraint : TableConstraint, IUniqueConstraint
    {
        public bool IsPrimaryKey { get; internal set; }

        public IReadOnlyList<Column> ConstrainedColumns { get; internal set; }

        internal UniqueConstraint() { }

        IReadOnlyList<IColumn> IUniqueConstraint.ConstrainedColumns
        {
            get { return ConstrainedColumns; }
        }
        public override SchemaObjectType Type
        {
            get { return IsPrimaryKey ? SchemaObjectType.PrimaryKeyConstraint : SchemaObjectType.UniqueConstraint; }
        }
    }
}
