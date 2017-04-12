using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseManagement
{
    public interface IForeignKeyConstraint : ITableConstraint
    {
        ForeignKeyRule UpdateRule { get; }
        ForeignKeyRule DeleteRule { get; }
        IReadOnlyList<IColumn> ForeignKeyColumns { get; }
        IReadOnlyList<IColumn> ReferencedColumns { get; }
        ITable ReferencedTable { get; }
    }
}
