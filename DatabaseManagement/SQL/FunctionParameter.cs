using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseManagement.SQL
{
    public class FunctionParameter
    {
        //public string Name { get; internal set; }

        public DbType ParameterType { get; internal set; }

        public bool IsOptional { get; internal set; }

        
    }
}
