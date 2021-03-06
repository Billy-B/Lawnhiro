﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseManagement.SQL
{
    public abstract class TableValuedExpression : Expression
    {
        public string Alias { get; internal set; }

        public override Expression DispatchAny(ExpressionVisitor visitor)
        {
            return visitor.VisitTable(this);
        }

        internal abstract TableValuedExpression Dispatch(ExpressionVisitor visitor);
    }
}
