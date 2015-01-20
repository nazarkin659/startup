using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelperFunctions
{
    public static class IEnumerableExtentions
    {
        private static Random random = new Random((int)DateTime.Now.Ticks);

        public static bool IsNullOrEmpty<TSource>(this IEnumerable<TSource> o)
        {
            if (o == null || o.Count() == 0)
                return true;

            return false;
        }

        public static bool IsNullOrEmpty<TSource>(this ICollection<TSource> o)
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

        public static TSource Random<TSource>(this List<TSource> o)
        {
            if (!o.IsNullOrEmpty())
            {
                int index = random.Next(0, o.Count());
                return o[index];
            }

            return default(TSource);
        }
    }
}
