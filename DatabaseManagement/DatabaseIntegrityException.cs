using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseManagement
{
    public class DatabaseIntegrityException : Exception
    {
        public DatabaseIntegrityException() : base() { }
        public DatabaseIntegrityException(string message) : base(message) { }
        public DatabaseIntegrityException(string message, Exception innerException) : base(message, innerException) { }
    }
}
