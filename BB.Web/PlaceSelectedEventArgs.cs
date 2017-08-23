using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BB.Web
{
    public class PlaceSelectedEventArgs : EventArgs
    {
        internal PlaceSelectedEventArgs() { }

        public Place SelectedPlace { get; internal set; }
    }
}
