using System.Collections.Generic;
using System.Linq;

namespace ImpEx
{
    public static class ExtensionMethods
    {
        internal static string FromCollectionToString<T>(this IEnumerable<T> collection, string separator = ", ")
        {
            return string.Join(separator, collection.Select(x => x.ToString()));
        }
    }
}
