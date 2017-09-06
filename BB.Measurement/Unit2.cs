using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BB.Measure
{
    public sealed class Unit2
    {
        private string _name;

        private Dictionary<Unit2, int> _construction;

        private double _scaleFactor;
        internal MeasurementDimensions _dimensions;

        private Unit2 _scaledUnit;

        private Dictionary<Unit2, int> Construction
        {
            get
            {
                Dictionary<Unit2, int> ret = _construction;
                if (ret == null)
                {
                    ret = new Dictionary<Unit2, int>
                    {
                        { this, 1 }
                    };
                    _construction = ret;
                }
                return ret;
            }
        }

        public string Name
        {
            get
            {
                string ret = _name;
                if (ret == null)
                {
                    ret = buildName(Construction);
                    _name = ret;
                }
                return ret;
            }
        }
        private static string buildName(Dictionary<Unit2, int> construction)
        {
            if (construction.Count == 1)
            {
                var kvp = construction.Single();
                if (kvp.Key._dimensions == MeasurementDimensions.Length)
                {
                    if (kvp.Value == 2)
                    {
                        return "square " + kvp.Key.Name;
                    }
                    else if (kvp.Value == 3)
                    {
                        return "cubic " + kvp.Key.Name;
                    }
                }
            }
            else if (construction.Count == 2)
            {
                var kvps = construction.OrderByDescending(kvp => kvp.Value).ToArray();
                var kvp1 = kvps[0];
                var kvp2 = kvps[1];
                if (kvp1.Value > 0 && kvp2.Value < 0)
                {
                    if (kvp1.Value == 1 && kvp2.Value == -1)
                    {
                        return kvp1.Key.Name + " per " + kvp1.Key.Name;
                    }
                }
            }
            return FormatHelpers.FormatPowerNotation(construction.Select(kvp => new KeyValuePair<string, int>(kvp.Key.Name, kvp.Value)));
        }
    }
}
