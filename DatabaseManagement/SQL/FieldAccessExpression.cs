using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseManagement.SQL
{
    public abstract class FieldAccessExpression : ScalarExpression
    {
        public TableValuedExpression Table { get; internal set; }
    }
}
