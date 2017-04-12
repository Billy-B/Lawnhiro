using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseManagement
{
    public enum SchemaObjectType
    {
        CheckConstraint,
        ForeignKeyConstraint,
        Table,
        UniqueConstraint,
        DefaultConstraint,
        PrimaryKeyConstraint,
        View
    }
}
