using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InitVent.Common.Extensions
{
    public static class CollectionExtensions
    {
        public static void AddAll<T>(this ICollection<T> collection, params T[] items)
        {
            AddAll(collection, items.AsEnumerable());
        }

        public static void AddAll<T>(this ICollection<T> collection, IEnumerable<T> items)
        {
            foreach (var item in items)
                collection.Add(item);
        }
    }
}
