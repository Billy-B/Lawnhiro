using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BB
{
    internal enum ObjectState
    {
        New = 0,
        AwaitingInsert = 1,
        Attached = 2,
        Deleted = 3
    }
}
