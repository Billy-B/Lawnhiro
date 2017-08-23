using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BB.Measure
{
    internal static class FormatHelpers
    {
        private const char SUPERSCRIPT_MINUS = '⁻';

        private static readonly char[] _superscriptLUT =
        {
            '⁰',
            '¹',
            '²',
            '³',
            '⁴',
            '⁵',
            '⁶',
            '⁷',
            '⁸',
            '⁹'
        };

        public static string FormatPowerNotation(IEnumerable<KeyValuePair<string, int>> unitsAndExponents)
        {
            KeyValuePair<string, int>[] nonZeroUnits = unitsAndExponents.Where(kvp => kvp.Value != 0).ToArray();
            if (nonZeroUnits.Length == 2)
            {
                KeyValuePair<string, int> dimPositive = nonZeroUnits.FirstOrDefault(kvp => kvp.Value > 0);
                KeyValuePair<string, int> dimNegative = nonZeroUnits.FirstOrDefault(kvp => kvp.Value < 0);
                if (dimPositive.Value != 0 && dimNegative.Value != 0)
                {
                    return getPowerNotation(dimPositive.Key, dimPositive.Value) + "/" + getPowerNotation(dimNegative.Key, -dimNegative.Value);
                }
            }
            return string.Join("*", nonZeroUnits.Select(kvp => getPowerNotation(kvp.Key, kvp.Value)));
        }
        private static string getPowerNotation(string @base, int exp)
        {
            switch (exp)
            {
                case 0:
                    return "1";
                case 1:
                    return @base;
                default:
                    return @base + getSuperscript(exp);
            }
        }
        private static string getSuperscript(int exp)
        {
            StringBuilder sb = new StringBuilder(exp.ToString());
            int start;
            if (exp < 0)
            {
                start = 1;
                sb[0] = SUPERSCRIPT_MINUS;
            }
            else
            {
                start = 0;
            }
            for (int i = start; i < sb.Length; i++)
            {
                sb[i] = _superscriptLUT[sb[i] - '0'];
            }
            return sb.ToString();
        }

    }
}
