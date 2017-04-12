using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseManagement.SqlServer
{
    internal class UniqueConstraintColumnMapping
    {
        internal UniqueConstraint Constraint { get; set; }

        internal Column Column { get; set; }

        internal int Position { get; set; }
    }
}
