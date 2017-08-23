using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BB;

namespace Lawnhiro.API
{
    [Table]
    public class ServiceArea
    {
        [Column]
        public string City { get; }
        [Column]
        public string State { get; }
        [Column]
        public decimal BasePrice { get; }
        [Column]
        public decimal PricePerSqFt { get; }
        [Column]
        public bool Active { get; }
    }
}
