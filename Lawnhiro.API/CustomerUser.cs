using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lawnhiro.API
{
    public class CustomerUser
    {
        public int Id { get; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string EmailAddress { get; set; }
        public int PayPalId { get; set; }
        internal string PasswordHash { get; set; }
        public DateTime Created { get; set; }
        public DateTime LastModified { get; set; }
        public bool Active { get; set; }
        public ICollection<Order> Orders { get; }
        public ICollection<Residence> Residences { get; }
        public override string ToString()
        {
            return FirstName + " " + LastName;
        }
    }
}
