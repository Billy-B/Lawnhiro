﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseManagement.SQL
{
    public class TableExpression : TableValuedExpression
    {
        public ITable Table { get; internal set; }

        internal TableExpression() { }

        public override ExpressionType Type
        {
            get { throw new NotImplementedException(); }
        }

        public override string ToString()
        {
            return Table.ToString();
        }

        internal override IEnumerable<Expression> EnumerateSubExpressions()
        {
            return Enumerable.Empty<Expression>();
        }
    }
}