using BB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lawnhiro.API
{
    [Table]
    public class HeardAboutUsSource
    {
        [Column]
        public int Id { get; }

        [Column]
        public string Name { get; }

        public override string ToString()
        {
            return Name;
        }
    }
}
