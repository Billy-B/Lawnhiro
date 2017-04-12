using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseManagement
{
    internal class ReadOnlyList<T> : ReadOnlyCollection<T>
    {
        internal ReadOnlyList() : base(new List<T>()) { }

        internal void Add(T item)
        {
            Items.Add(item);
        }
    }
}
