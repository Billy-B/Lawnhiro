using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseManagement
{
    public static class Extensions
    {
        internal static string TrimBracketNotation(this string self)
        {
            self = self.Trim();
            if (self.StartsWith("[") && self.EndsWith("]"))
            {
                return self.Substring(0, self.Length - 2).Trim();
            }
            return self;
        }

        internal static string[] SplitQualifiedNameIntoParts(this string self)
        {
            string[] ret = self.Split('.');
            for (int i = 0; i < ret.Length; i++)
            {
                ret[i] = ret[i].TrimBracketNotation().ToLowerInvariant();
            }
            return ret;
        }
    }
}
