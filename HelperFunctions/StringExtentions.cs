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

        public static string AppendString(this string sourse, string append)
        {
            return string.Format("{0}{1}", sourse, append);
        }

        public static string ToLowerSafely(this string sourse)
        {
            return sourse ?? "";
        }

        public static string ReplaceSafely(this string sourse, string replace, string replaceTo)
        {
            if (sourse == null)
                return "";

            return sourse.Replace(replace, replaceTo);
        }
    }
}
