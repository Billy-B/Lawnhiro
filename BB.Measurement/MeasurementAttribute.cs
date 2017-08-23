using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BB.Measure
{
    [AttributeUsage(AttributeTargets.Struct, AllowMultiple = false)]
    public class MeasurementAttribute : Attribute
    {
        public MeasurementAttribute(int dimMass, int dimLength, int dimTime)
        {
            DimLength = dimLength;
            DimTime = dimTime;
            DimMass = dimMass;
        }

        public int DimLength { get; private set; }
        public int DimTime { get; private set; }
        public int DimMass { get; private set; }
    }
}
