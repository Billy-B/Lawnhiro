using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseManagement
{
    public enum ForeignKeyRule
    {
        None = 0,
        Cascade = 1,
        SetNull = 2,
        SetDefault = 3,
        Restrict = 4
    }
}
