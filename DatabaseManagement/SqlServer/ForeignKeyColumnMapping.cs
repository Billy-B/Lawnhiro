using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseManagement.SqlServer
{
    internal class ForeignKeyColumnMapping
    {
        internal ForeignKeyConstraint Constraint { get; set; }

        internal Column ForeignKeyColumn { get; set; }

        internal Column ReferencedColumn { get; set; }

        internal int Position { get; set; }
    }
}
