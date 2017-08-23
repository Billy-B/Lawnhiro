using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BB;

namespace Lawnhiro.API
{
    [Table("CouponCodes")]
    public class Coupon
    {
        [Column]
        public string Code { get; }

        [Column]
        public int TimesUsed { get; set; }

        [Column]
        public decimal Discount { get; }

        [Column]
        public bool Enabled { get; }

        public override string ToString()
        {
            return Code;
        }
    }
}
