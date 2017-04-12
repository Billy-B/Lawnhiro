using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseManagement
{
    public interface IView : ITable
    {
        bool IsUpdatable { get; }
        ViewCheckOption CheckOption { get; }
    }
}
