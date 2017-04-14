using BB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lawnhiro.API
{
    [Table]
    public class Residence
    {
        [Column]
        public int Id { get; }
        [Column]
        public string GooglePlaceId { get; set; }
        [Column("AddressLine1")]
        public string Address { get; set; }
        [Column]
        public string Zip { get; set; }
        [Column]
        public string City { get; set; }
        [Column]
        public string State { get; set; }
        [ForeignKey]
        public HeardAboutUsSource Source { get; set; }
        [Column]
        public string ProviderCode { get; set; }
        [Column]
        public decimal MowableSqFt { get; set; }
    }
}
