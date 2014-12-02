using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelperFunctions
{
    public static class StringExtentions
    {
        public static bool IsNullOrWhiteSpace(this string str)
        {
            if (string.IsNullOrWhiteSpace(str))
                return true;

            return false;
        }

        public static bool IsNullOrEmpty(this string str)
        {
            if (string.IsNullOrEmpty(str))
                return true;

            return false;
        }
    }
}
