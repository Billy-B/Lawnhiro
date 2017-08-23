using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BB
{
    public abstract class ObjectRepository
    {
        public IQueryable<T> Query<T>()
        {
            throw new NotImplementedException();
        }

        public abstract void Add(object obj);

        public abstract void Delete(object obj);

        public abstract void CommitChanges();

        public abstract void DiscardChanges();
    }
}
