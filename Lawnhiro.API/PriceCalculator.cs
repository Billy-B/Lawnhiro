using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lawnhiro.API
{
    public static class PriceCalculator
    {
        public static decimal CalculateMowableSqFt(ZillowResidenceInfo info)
        {
            if (info == null)
            {
                throw new ArgumentNullException("info");
            }

            decimal estimatedFootprint;
            decimal lotSqFt;
            if (info.LotSizeSqFt == null)
            {
                lotSqFt = info.FinishedSqFt * 3;
            }
            else
            {
                lotSqFt = (decimal)info.LotSizeSqFt;
            }
            if (info.NumberOfFloors == null) // actual number of floors not available, must estimate.
            {
                estimatedFootprint = Math.Min(info.FinishedSqFt, lotSqFt / 2);
            }
            else
            {
                estimatedFootprint = (info.FinishedSqFt / (decimal)info.NumberOfFloors);
            }
            return lotSqFt - estimatedFootprint;
        }

        public static decimal CalculatePrice(ServiceArea area, decimal mowableSqFt)
        {
            decimal ret = Math.Floor(area.BasePrice + (mowableSqFt * area.PricePerSqFt));
            return ret;
        }
    }
}
