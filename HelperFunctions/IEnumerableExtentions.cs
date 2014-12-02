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
            if (o == null || o.IsNullOrEmpty())
                return true;

            return false;
        }
    }
}
