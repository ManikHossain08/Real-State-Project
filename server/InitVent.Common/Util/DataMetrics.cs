using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InitVent.Common.Util
{
    public static class DataMetrics
    {
        /// <summary>
        /// Computes the Levenshtein distance between two sequences, counting the total
        /// number of insertions, deletions, and substitutions required to transform one
        /// sequence into the other.
        /// </summary>
        /// <typeparam name="T">The type of the elements of the sequences</typeparam>
        /// <param name="first">The first sequence</param>
        /// <param name="second">The second sequence</param>
        /// <param name="comparer">The comparer to use to identify matching elements of the sequences, or none to use the default comparer</param>
        /// <returns>The Levenshtein distance between the two given sequences</returns>
        /// <remarks>
        /// See http://en.wikipedia.org/wiki/Levenshtein_distance .  This implementation
        /// was taken directly from the aforementioned page; note that numerous
        /// optimizations are possible.
        /// </remarks>
        public static int LevenshteinDistance<T>(IEnumerable<T> first, IEnumerable<T> second, IEqualityComparer<T> comparer = null)
        {
            if (first == null)
                throw new ArgumentNullException("first");
            if (second == null)
                throw new ArgumentNullException("second");

            T[] a = first.ToArray(), b = second.ToArray();

            if (comparer == null)
                comparer = EqualityComparer<T>.Default;

            // For all i and j, distance[i, j] will hold the Levenshtein distance between
            // the first i characters of a and the first j characters of b;
            var distance = new int[a.Length + 1, b.Length + 1];

            // Populate the trivial values (the distances to an empty string)
            for (int i = 0; i <= a.Length; i++)
                distance[i, 0] = i;
            for (int j = 0; j <= b.Length; j++)
                distance[0, j] = j;

            for (int i = 1; i <= a.Length; i++)
            {
                for (int j = 1; j <= b.Length; j++)
                {
                    if (comparer.Equals(a[i - 1], b[j - 1]))
                    {
                        distance[i, j] = distance[i - 1, j - 1];  // No operation required
                    }
                    else
                    {
                        distance[i, j] = new[] {
                                distance[i - 1, j],     // Deletion
                                distance[i, j - 1],     // Insertion
                                distance[i - 1, j - 1], // Substitution
                            }.Min() + 1;
                    }
                }
            }

            return distance[a.Length, b.Length];
        }
    }
}
