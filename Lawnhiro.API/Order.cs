using BB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lawnhiro.API
{
    [Table("Orders")]
    public class Order
    {
        [Column]
        public int Id { get; }
        [ForeignKey]
        public Residence Residence { get; set; }
        [ForeignKey]
        public Customer Customer { get; set; }
        [Column]
        public DateTimeOffset Placed { get; set; }
        [Column]
        public string CustomerNotes { get; set; }
        [Column]
        public decimal Price { get; set; }
        [ForeignKey]
        public Provider ClaimedBy { get; set; }
        [Column]
        public DateTime? ETA { get; set; }
        [Column]
        public string ProviderNotes { get; set; }
        [Column]
        public bool Complete { get; set; }
        [Column]
        public string PayPalOrderId { get; set; }
        [Column]
        public bool Completed { get; set; }
    }
}
