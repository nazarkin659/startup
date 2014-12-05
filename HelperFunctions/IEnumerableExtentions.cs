using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelperFunctions
{
    public static class IEnumerableExtentions
    {
        public static bool IsNullOrEmpty<TSource>(this IEnumerable<TSource> o)
        {
            if (o == null || o.Count() == 0)
                return true;

            return false;
        }

        public static void AddSafely<T, K>(this Dictionary<T, K> dict, T key, K value)
        {
            if (!dict.ContainsKey(key))
                dict.Add(key, value);
        }
    }
}
