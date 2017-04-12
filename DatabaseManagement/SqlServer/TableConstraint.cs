using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseManagement.SqlServer
{
    public abstract class TableConstraint : SchemaObject, ITableConstraint
    {
        public Table Table { get; internal set; }

        ITable ITableConstraint.Table
        {
            get { return Table; }
        }
    }
}
