using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseManagement.SQL
{
    public abstract class FieldAccessExpression : ScalarExpression
    {
        public new TableValuedExpression Table { get; internal set; }

        internal sealed override string ToCommandString()
        {
            return ToString();
        }
    }
}
