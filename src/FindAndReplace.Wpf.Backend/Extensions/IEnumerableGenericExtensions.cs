using System.Collections.Generic;
using System.Linq;

namespace FindAndReplace.Wpf.Backend.Extensions
{
    public static class IEnumerableGenericExtensions
    {
        public static bool IsNullOrEmpty<T>(this IEnumerable<T> collection)
        {
            if (collection == null)
                return true;

            var isEmpty = !collection.Any();
            return isEmpty;
        }

    }
}
