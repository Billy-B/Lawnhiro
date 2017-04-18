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
        public string City { get; set; }
        [Column]
        public string State { get; set; }
        [Column]
        public decimal BasePrice { get; set; }
        [Column]
        public decimal PricePerSqFt { get; set; }
        [Column]
        public DateTime StartDate { get; set; }
    }
}
