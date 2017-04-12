using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BB
{
    internal interface ICacheIndex<T>
    {
        void Add(T item);
        void Remove(T item);
    }
}
