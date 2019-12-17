using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InitVent.Common.Extensions
{
    public static class Comparer
    {
        public static IComparer<T> AsComparer<T>(this Func<T, T, int> compareMethod)
        {
            return new DelegatedComparer<T>(compareMethod);
        }

        public static IComparer<T> Reverse<T>(this IComparer<T> baseComparer)
        {
            return new DelegatedComparer<T>((a, b) => -baseComparer.Compare(a, b));
        }

        public static IComparer<T> Compose<T>(this IComparer<T> baseComparer, IComparer<T> tiebreaker)
        {
            return new DelegatedComparer<T>(delegate(T a, T b)
            {
                int baseResult = baseComparer.Compare(a, b);
                return baseResult != 0 ? baseResult : tiebreaker.Compare(a, b);
            });
        }
    }

    public class DelegatedComparer<T> : Comparer<T>
    {
        private readonly Func<T, T, int> CompareMethod;

        public DelegatedComparer(Func<T, T, int> compareMethod)
        {
            CompareMethod = compareMethod;
        }

        public override int Compare(T a, T b)
        {
            return CompareMethod(a, b);
        }
    }

    public class DelegatedEqualityComparer<T> : EqualityComparer<T>
    {
        private readonly Func<T, T, bool> EqualityMethod;
        private readonly Func<T, int> HashCodeMethod;

        public DelegatedEqualityComparer(Func<T, T, bool> equalityMethod, Func<T, int> hashCodeMethod)
        {
            EqualityMethod = equalityMethod;
            HashCodeMethod = hashCodeMethod;
        }

        public override bool Equals(T x, T y)
        {
            return EqualityMethod(x, y);
        }

        public override int GetHashCode(T obj)
        {
            return HashCodeMethod(obj);
        }
    }
}
