using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BB
{
    internal interface IObjectRepository// : IQueryProvider
    {
        void Commit(IList<IGrouping<Type, object>> objectsToInsert, IList<IGrouping<Type, object>> objectsToDelete, IList<IGrouping<Type, ObjectChangeTracker>> objectsToUpdate);
    }
}
