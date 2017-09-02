using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BB.Measure
{
    public abstract class UnitSystem : IFormatProvider
    {
        internal UnitSystem() { }

        private Dictionary<string, List<Unit>> _allUnitsIndexedByAbbreviation = new Dictionary<string, List<Unit>>();
        private Dictionary<Type, MeasurementUnits> _indexedByType = new Dictionary<Type, MeasurementUnits>();

        private static readonly Unit[] _empty = new Unit[0];

        public Unit GetUnit(string unit)
        {
            Unit[] units = GetUnits(unit);
            switch (units.Length)
            {
                case 1:
                    return units[1];
                case 0:
                    throw new ArgumentException("Unit is undefined.", "unit");
                default:
                    throw new ArgumentException("Ambiguous match found.", "unit");
            }
        }

        public void AddUnit(Unit unit, Type measurementType)
        {
            assertValidMeasurementType(measurementType);
        }

        public void SetDefault(Unit unit, Type measurementType)
        {
            assertValidMeasurementType(measurementType);
            MeasurementDimensions unitDim = unit.Dimensions;
            MeasurementDimensions typeDim = Measurement.GetDimensions(measurementType);
            if (unitDim != typeDim)
            {
                throw new InvalidOperationException("Unit " + unit + " does not have the proper dimensions for measurement type " + measurementType);
            }
            GetUnitsByMeasurement(measurementType).StandardUnit = unit;
        }

        public void AssignAbbreviation(Unit unit, string abbreviation)
        {
            throw new NotImplementedException();
        }

        internal Unit[] GetUnits(string unit)
        {
            List<Unit> ret;
            if (_allUnitsIndexedByAbbreviation.TryGetValue(unit, out ret))
            {
                return ret.ToArray();
            }
            else
            {
                return _empty;
            }
        }


        public class MeasurementUnits
        {
            public Type MeasurementType { get; internal set; }
            public Unit StandardUnit { get; internal set; }
            public ICollection<Unit> Units { get; internal set; }
            internal readonly MeasurementDimensions _dimensions;

            private Dictionary<string, Unit> _indexedByAbbreviation;
        }

        public MeasurementUnits Mass
        {
            get { return GetUnitsByMeasurement(typeof(Mass)); }
        }
        public MeasurementUnits Length
        {
            get { return GetUnitsByMeasurement(typeof(Length)); }
        }
        public MeasurementUnits Area
        {
            get { return GetUnitsByMeasurement(typeof(Area)); }
        }
        public MeasurementUnits Time
        {
            get { return GetUnitsByMeasurement(typeof(Time)); }
        }
        public MeasurementUnits Volume
        {
            get { return GetUnitsByMeasurement(typeof(Volume)); }
        }
        public MeasurementUnits Force
        {
            get { return GetUnitsByMeasurement(typeof(Force)); }
        }
        public MeasurementUnits Energy
        {
            get { return GetUnitsByMeasurement(typeof(Energy)); }
        }

        public MeasurementUnits GetUnitsByMeasurement(Type measurementType)
        {
            assertValidMeasurementType(measurementType);
            MeasurementUnits ret;
            lock (_indexedByType)
            {
                if (!_indexedByType.TryGetValue(measurementType, out ret))
                {
                    ret = new MeasurementUnits();
                    _indexedByType.Add(measurementType, ret);
                }
            }
            return ret;
        }

        private static void assertValidMeasurementType(Type measurementType)
        {
            if (measurementType == null)
            {
                throw new ArgumentNullException("measurementType");
            }
        }

        public object GetFormat(Type formatType)
        {
            throw new NotImplementedException();
        }

        public class SIUnitSystem : UnitSystem
        {
            public readonly Unit Second;
            
        }
    }
}
