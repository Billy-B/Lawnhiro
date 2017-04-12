using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BB;

namespace Lawnhiro.API
{
    [Table]
    public class Customer
    {
        [Column]
        public int Id { get; }
        [Column]
        public string Email { get; set; }
    }
}
