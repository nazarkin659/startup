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

        public static string AppendString(this string source, string append)
        {
            return string.Format("{0}{1}", source, append);
        }

        public static string ToLowerSafely(this string source)
        {
            return source ?? "";
        }

        public static string ReplaceSafely(this string source, string replace, string replaceTo)
        {
            if (source == null)
                return "";

            return source.Replace(replace, replaceTo);
        }

        public static bool ContainsSafely(this string source, string value)
        {
            if (!source.IsNullOrWhiteSpace() && source.Contains(value))
                return true;

            return false;
        }
    }
}
