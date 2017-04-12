using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseManagement.SqlServer
{
    public class View : Table, IView
    {
        public ViewCheckOption CheckOption { get; internal set; }

        internal View() { }

        bool IView.IsUpdatable
        {
            get { return false; }
        }
    }
}
