using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseManagement.SQL
{
    public abstract class ScalarExpression : Expression
    {
        public abstract DbType DbType { get; }
    }
}
