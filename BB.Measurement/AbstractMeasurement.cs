using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BB.Measure
{
    internal struct AbstractMeasurement
    {
        internal double _value;
        internal MeasurementDimensions _dim;

        internal AbstractMeasurement(double value, MeasurementDimensions dim)
        {
            _value = value;
            _dim = dim;
        }

        public override bool Equals(object obj)
        {
            if (obj is AbstractMeasurement)
            {
                AbstractMeasurement other = (AbstractMeasurement)obj;
                return _dim == other._dim && _value == other._value;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return _value.GetHashCode() ^ _dim.GetHashCode();
        }

        public override string ToString()
        {
            return Measurement.Format(_value, _dim);
        }
    }
}
