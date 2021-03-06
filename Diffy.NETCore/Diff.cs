﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Diffy
{
    /// <summary>
    /// Contains methods to compute the difference between two lists.
    /// </summary>
    public static class Diff
    {
        /// <summary>
        /// Computes the difference between two lists.
        /// </summary>
        /// <remarks>
        /// The diff data will be computed lazily via yield. You need to enumerate the enumerable
        /// to do the whole processing.
        /// </remarks>
        /// <typeparam name="T">The type of element inside the lists.</typeparam>
        /// <param name="firstCollection">The source collection.</param>
        /// <param name="secondCollection">The destination collection.</param>
        /// <returns>The diff data.</returns>
        public static IEnumerable<DiffSection> Compute<T>(IList<T> firstCollection, IList<T> secondCollection)
        {
            return Compute(firstCollection, secondCollection, EqualityComparer<T>.Default);
        }

        /// <summary>
        /// Computes the difference between two lists.
        /// </summary>
        /// <remarks>
        /// The diff data will be computed lazily via yield. You need to enumerate the enumerable
        /// to do the whole processing.
        /// </remarks>
        /// <typeparam name="T">The type of element inside the lists.</typeparam>
        /// <param name="firstCollection">The source collection.</param>
        /// <param name="secondCollection">The destination collection.</param>
        /// <param name="comparer">The <see cref="IEqualityComparer{T}"/> used to test for equality.</param>
        /// <returns>The diff data.</returns>
        public static IEnumerable<DiffSection> Compute<T>(IList<T> firstCollection, IList<T> secondCollection, IEqualityComparer<T> comparer)
        {
            Requires.NotNull(firstCollection, nameof(firstCollection));
            Requires.NotNull(secondCollection, nameof(secondCollection));

            return Compute(firstCollection, 0, firstCollection.Count, secondCollection, 0, secondCollection.Count, comparer);
        }

        /// <summary>
        /// Computes the difference between two lists.
        /// </summary>
        /// <remarks>
        /// The diff data will be computed lazily via yield. You need to enumerate the enumerable
        /// to do the whole processing.
        /// </remarks>
        /// <typeparam name="T">The type of element inside the lists.</typeparam>
        /// <param name="firstCollection">The source collection.</param>
        /// <param name="firstStart">The starting index inside the first collection.</param>
        /// <param name="firstEnd">The ending index inside the first collection.</param>
        /// <param name="secondCollection">The destination collection.</param>
        /// <param name="secondStart">The starting index inside the second collection.</param>
        /// <param name="secondEnd">The ending index inside the second collection.</param>
        /// <param name="equalityComparer">The <see cref="IEqualityComparer{T}"/> used to test for equality.</param>
        /// <returns>The diff data.</returns>
        public static IEnumerable<DiffSection> Compute<T>(
            IList<T> firstCollection, int firstStart, int firstEnd,
            IList<T> secondCollection, int secondStart, int secondEnd,
            IEqualityComparer<T> equalityComparer)
        {
            Requires.NotNull(firstCollection, nameof(firstCollection));
            Requires.Range(firstStart >= 0, nameof(firstStart), nameof(firstStart) + " must be greater than zero!");
            Requires.Range(firstEnd >= 0, nameof(firstEnd), nameof(firstStart) + " must be greater than zero!");
            Requires.NotNull(secondCollection, nameof(secondCollection));
            Requires.Range(secondStart >= 0, nameof(secondStart), nameof(secondStart) + " must be greater than zero!");
            Requires.Range(secondEnd >= 0, nameof(secondEnd), nameof(secondEnd) + " must be greater than zero!");
            Requires.NotNull(equalityComparer, nameof(equalityComparer));

            LongestCommonSubsequenceResult lcs = FindLongestCommonSubsequence(
                firstCollection, firstStart, firstEnd,
                secondCollection, secondStart, secondEnd,
                equalityComparer
            );

            if (lcs.IsSuccess) {
                // deal with the section before
                IEnumerable<DiffSection> sectionsBefore = Compute(
                    firstCollection, firstStart, lcs.PositionInFirstCollection,
                    secondCollection, secondStart, lcs.PositionInSecondCollection,
                    equalityComparer
                );
                foreach (DiffSection section in sectionsBefore) {
                    yield return section;
                }

                // output the copy operation
                yield return new DiffSection(DiffSectionType.Copy, lcs.Length);

                // deal with the section after
                IEnumerable<DiffSection> sectionsAfter = Compute(
                    firstCollection, lcs.PositionInFirstCollection + lcs.Length, firstEnd,
                    secondCollection, lcs.PositionInSecondCollection + lcs.Length, secondEnd,
                    equalityComparer
                );
                foreach (DiffSection section in sectionsAfter) {
                    yield return section;
                }
            } else {
                // if we get here, no LCS

                if (firstStart < firstEnd) {
                    // we got content from first collection --> deleted
                    yield return new DiffSection(DiffSectionType.Delete, firstEnd - firstStart);
                }
                if (secondStart < secondEnd) {
                    // we got content from second collection --> inserted
                    yield return new DiffSection(DiffSectionType.Insert, secondEnd - secondStart);
                }
            }
        }

        /// <summary>
        /// Finds the longest common subsequence in two lists.
        /// </summary>
        /// <typeparam name="T">The type of element inside the lists.</typeparam>
        /// <param name="firstCollection">The source collection.</param>
        /// <param name="firstStart">The starting index inside the first collection.</param>
        /// <param name="firstEnd">The ending index inside the first collection.</param>
        /// <param name="secondCollection">The destination collection.</param>
        /// <param name="secondStart">The starting index inside the second collection.</param>
        /// <param name="secondEnd">The ending index inside the second collection.</param>
        /// <param name="equalityComparer">The <see cref="IEqualityComparer{T}"/> used to test for equality.</param>
        /// <returns>A <see cref="LongestCommonSubsequenceResult"/>.</returns>
        public static LongestCommonSubsequenceResult FindLongestCommonSubsequence<T>(
            IList<T> firstCollection, int firstStart, int firstEnd,
            IList<T> secondCollection, int secondStart, int secondEnd,
            IEqualityComparer<T> equalityComparer)
        {
            Requires.NotNull(firstCollection, nameof(firstCollection));
            Requires.NotNull(secondCollection, nameof(secondCollection));
            Requires.NotNull(equalityComparer, nameof(equalityComparer));
            Requires.Range(firstEnd <= firstCollection.Count, nameof(firstEnd), "Range end must be inside of the first collection.");
            Requires.Range(secondEnd <= secondCollection.Count, nameof(secondEnd), "Range end must be inside of the second collection.");
            Requires.NotNull(equalityComparer, nameof(equalityComparer));

            // default result, if we can't find anything
            LongestCommonSubsequenceResult result = new LongestCommonSubsequenceResult();

            for (int index1 = firstStart; index1 < firstEnd; index1++) {
                for (int index2 = secondStart; index2 < secondEnd; index2++) {
                    if (equalityComparer.Equals(firstCollection[index1], secondCollection[index2])) {
                        int length = CountEqual(
                            firstCollection, index1, firstEnd,
                            secondCollection, index2, secondEnd,
                            equalityComparer
                        );

                        // Is longer than what we already have --> record new LCS
                        if (length > result.Length) {
                            result = new LongestCommonSubsequenceResult(
                                index1,
                                index2,
                                length
                            );
                        }
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Counts how many elements are equal in both lists starting at the given positions.
        /// </summary>
        /// <typeparam name="T">The type of element inside the lists.</typeparam>
        /// <param name="firstCollection">The source collection.</param>
        /// <param name="firstPosition">The starting index inside the first collection.</param>
        /// <param name="firstEnd">The ending index inside the first collection.</param>
        /// <param name="secondCollection">The destination collection.</param>
        /// <param name="secondPosition">The starting index inside the second collection.</param>
        /// <param name="secondEnd">The ending index inside the second collection.</param>
        /// <param name="equalityComparer">The <see cref="IEqualityComparer{T}"/> used to test for equality.</param>
        /// <returns>The length of the equal sequence.</returns>
        public static int CountEqual<T>(
            IList<T> firstCollection, int firstPosition, int firstEnd,
            IList<T> secondCollection, int secondPosition, int secondEnd,
            IEqualityComparer<T> equalityComparer)
        {
            Requires.NotNull(firstCollection, nameof(firstCollection));
            Requires.NotNull(secondCollection, nameof(secondCollection));
            Requires.NotNull(equalityComparer, nameof(equalityComparer));
            Requires.Range(firstEnd <= firstCollection.Count, nameof(firstEnd), "Range end must be inside of the first collection.");
            Requires.Range(secondEnd <= secondCollection.Count, nameof(secondEnd), "Range end must be inside of the second collection.");
            Requires.NotNull(equalityComparer, nameof(equalityComparer));

            int length = 0;
            while (firstPosition < firstEnd && secondPosition < secondEnd) {
                if (!equalityComparer.Equals(firstCollection[firstPosition], secondCollection[secondPosition])) {
                    break;
                }

                firstPosition++;
                secondPosition++;
                length++;
            }
            return length;
        }
    }
}
