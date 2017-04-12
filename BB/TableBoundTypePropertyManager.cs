using DatabaseManagement;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BB
{
    internal abstract class TableBoundTypePropertyManager : PropertyManager
    {
        public ITable Table { get; set; }

        public abstract void AppendUpdate(IList<KeyValuePair<IColumn, object>> updateValues, object newPropValue);
        public abstract void AppendInsert(IList<KeyValuePair<IColumn, object>> insertValues, object propValue);
    }
}
