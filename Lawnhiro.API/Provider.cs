using BB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Lawnhiro.API
{
    [Table]
    public class Provider
    {
        [Column]
        public int Id { get; }
        [Column]
        public string FirstName { get; set; }
        [Column]
        public string LastName { get; set; }
        [Column]
        public string Email { get; set; }
        [Column]
        public string AddressLine1 { get; set; }
        [Column]
        public string AddressLine2 { get; set; }

        [Column]
        public string City { get; set; }
        [Column]
        public string State { get; set; }
        [Column]
        public string Zip { get; set; }

        [Column]
        public System.Data.SqlTypes.SqlBinary PasswordHash { get; set; }
        [Column]
        public System.Data.SqlTypes.SqlBinary PasswordSalt { get; set; }
        [Column]
        public string PhoneNumber { get; set; }

        [Column]
        public bool Active { get; set; }
        [Column]
        public bool Admin { get; set; }

        public override string ToString()
        {
            return FirstName + " " + LastName;
        }
    }
}
