using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InitVent.Common.Extensions
{
    public static class EnumerableExtensions
    {
        public static bool IsNullOrEmpty<T>(this IEnumerable<T> e)
        {
            return e == null || !e.Any();
        }

        public static bool IsNonEmpty<T>(this IEnumerable<T> e)
        {
            return !IsNullOrEmpty(e);
        }

        public static bool IsNullOrEmpty(this IEnumerable e)
        {
            if (e == null)
                return false;

            // Although testing e.GetEnumerator().MoveNext() might be faster,
            // the enumerator might nto get properly disposed of.  Foreach
            // handles this.
            foreach (var i in e)
                return false;

            return true;
        }

        public static bool IsNonEmpty(this IEnumerable e)
        {
            return !IsNullOrEmpty(e);
        }

        public static bool IsNullOrEmpty<T>(this ICollection<T> c)
        {
            return c == null || c.Count == 0;
        }

        public static bool IsNonEmpty<T>(this ICollection<T> c)
        {
            return !IsNullOrEmpty(c);
        }

        /*
        [Obsolete("Since some classes implement both ICollection and ICollection<T>, this method creates irreconcilable ambiguity.")]
        public static bool IsNullOrEmpty(this ICollection c)
        {
            return c == null || c.Count == 0;
        }

        [Obsolete("Since some classes implement both ICollection and ICollection<T>, this method creates irreconcilable ambiguity.")]
        public static bool IsNonEmpty(this ICollection c)
        {
            return !IsNullOrEmpty(c);
        }
         */

        public static IEnumerable<T> EmptyIfNull<T>(this IEnumerable<T> e)
        {
            return e ?? new T[0];
        }

        public static IEnumerable EmptyIfNull(this IEnumerable e)
        {
            return e ?? new Object[0];
        }

        public static IEnumerable<T> NullIfEmpty<T>(this IEnumerable<T> e)
        {
            return IsNullOrEmpty(e) ? null : e;
        }

        public static IEnumerable NullIfEmpty(this IEnumerable e)
        {
            return IsNullOrEmpty(e) ? null : e;
        }

        public static IEnumerable<T> Prepend<T>(this IEnumerable<T> source, params T[] items)
        {
            return items.Concat(source);
        }

        public static IEnumerable<T> Append<T>(this IEnumerable<T> source, params T[] items)
        {
            return source.Concat(items);
        }

        public static IEnumerable<T> PadEnd<T>(this IEnumerable<T> source, int totalCount, T padding = default(T))
        {
            int i = 0;
            foreach (var element in source)
            {
                yield return element;
                i++;
            }

            for (; i < totalCount; i++)
                yield return padding;
        }

        public static IEnumerable<T> InRange<T>(this IEnumerable<T> source, int? start, int? end, bool inclusive = false, bool oneBased = false)
        {
            if (oneBased)
            {
                start--;
                end--;
            }
            if (inclusive)
            {
                end++;
            }

            if (start.HasValue)
                source = source.Skip(start.Value);
            if (end.HasValue)
                source = source.Take(end.Value - (start ?? 0));

            return source;
        }

        public static IEnumerable<TResult> CrossJoin<TOuter, TInner, TResult>(this IEnumerable<TOuter> outer, IEnumerable<TInner> inner, Func<TOuter, TInner, TResult> resultSelector)
        {
            return from o in outer
                   from i in inner
                   select resultSelector(o, i);
        }

        public static IEnumerable<TResult> LeftOuterJoin<TOuter, TInner, TKey, TResult>(this IEnumerable<TOuter> outer, IEnumerable<TInner> inner,
            Func<TOuter, TKey> outerKeySelector, Func<TInner, TKey> innerKeySelector, Func<TOuter, TInner, TResult> resultSelector)
        {
            return from o in outer
                   join i in inner on outerKeySelector(o) equals innerKeySelector(i) into iValues
                   from i in iValues.DefaultIfEmpty()
                   select resultSelector(o, i);
        }

        public static IEnumerable<TResult> LeftOuterJoin<TOuter, TInner, TKey, TResult>(this IEnumerable<TOuter> outer, IEnumerable<TInner> inner,
            Func<TOuter, TKey> outerKeySelector, Func<TInner, TKey> innerKeySelector, Func<TOuter, TInner, TResult> resultSelector, IEqualityComparer<TKey> comparer)
        {
            //return outer
            //    .GroupJoin(inner, outerKeySelector, innerKeySelector, (o, iValues) => new { o, iValues }, comparer)
            //    .SelectMany(pair => pair.iValues.DefaultIfEmpty().Select(i => resultSelector(pair.o, i)));

            return outer
                .GroupJoin(inner, outerKeySelector, innerKeySelector, (o, iValues) => iValues.DefaultIfEmpty().Select(i => resultSelector(o, i)), comparer)
                .SelectMany(result => result);
        }

        public static IEnumerable<TResult> FullOuterJoin<TOuter, TInner, TKey, TResult>(this IEnumerable<TOuter> outer, IEnumerable<TInner> inner,
            Func<TOuter, TKey> outerKeySelector, Func<TInner, TKey> innerKeySelector, Func<TOuter, TInner, TResult> resultSelector)
        {
            var allKeys = outer.Select(outerKeySelector)
                .Union(inner.Select(innerKeySelector))
                .Distinct();

            return from key in allKeys
                   join o in outer on key equals outerKeySelector(o) into oList
                   from o in oList.DefaultIfEmpty()
                   join i in inner on key equals innerKeySelector(i) into iList
                   from i in iList.DefaultIfEmpty()
                   select resultSelector(o, i);
        }

        public static IEnumerable<TResult> FullOuterJoin<TOuter, TInner, TKey, TResult>(this IEnumerable<TOuter> outer, IEnumerable<TInner> inner,
            Func<TOuter, TKey> outerKeySelector, Func<TInner, TKey> innerKeySelector, Func<TOuter, TInner, TResult> resultSelector, IEqualityComparer<TKey> comparer)
        {
            var allKeys = outer.Select(outerKeySelector)
                .Union(inner.Select(innerKeySelector), comparer)
                .Distinct(comparer);

            return allKeys
                .GroupJoin(outer, key => key, outerKeySelector, (key, oValues) => new { key, oValues }, comparer)
                .GroupJoin(inner, kvp => kvp.key, innerKeySelector, (kvp, iValues) => new { kvp.oValues, iValues }, comparer)
                .SelectMany(pair => CrossJoin(pair.oValues.DefaultIfEmpty(), pair.iValues.DefaultIfEmpty(), resultSelector));
        }

        public static IDictionary<TKey, TValue> ToDictionary<TKey, TValue>(this IEnumerable<KeyValuePair<TKey, TValue>> source)
        {
            return source.ToDictionary(pair => pair.Key, pair => pair.Value);
        }

        public static IDictionary<TKey, TValue> ToDictionary<TKey, TValue>(this IEnumerable<KeyValuePair<TKey, TValue>> source, IEqualityComparer<TKey> comparer)
        {
            return source.ToDictionary(pair => pair.Key, pair => pair.Value, comparer);
        }

    }
}
