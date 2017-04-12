using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseManagement
{
    public interface IUniqueConstraint : ITableConstraint
    {
        bool IsPrimaryKey { get; }
        IReadOnlyList<IColumn> ConstrainedColumns { get; }
    }
}
