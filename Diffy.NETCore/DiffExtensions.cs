using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diffy
{
    /// <summary>
    /// A delegate used to represent a method inserting a group of items into a collection.
    /// </summary>
    /// <typeparam name="T">The type of item inside the collection.</typeparam>
    /// <param name="index">The index to insert at.</param>
    /// <param name="items">The items to insert.</param>
    public delegate void InsertRangeDelegate<T>(int index, IEnumerable<T> items);

    /// <summary>
    /// A delegate used to represent a method removing ranges from a collection.
    /// </summary>
    /// <param name="index">The index to start removing at.</param>
    /// <param name="count">The amount of elements to remove.</param>
    public delegate void RemoveRangeDelegate(int index, int count);

    /// <summary>
    /// Contains extensions to <see cref="IEnumerable{T}"/> and <see cref="IList{T}"/>.
    /// </summary>
    public static class DiffExtensions
    {
        /// <summary>
        /// Computes the diff between two sequences.
        /// </summary>
        /// <remarks>
        /// Since computing the diff requires an <see cref="IList{T}"/>, this method executes the <see cref="IEnumerable{T}"/>.
        /// </remarks>
        /// <typeparam name="T">The <see cref="Type"/> of element.</typeparam>
        /// <param name="source">The source sequence.</param>
        /// <param name="destination">The sequence, <paramref name="source"/> shall be transformed into.</param>
        /// <returns>The diff data.</returns>
        public static IEnumerable<DiffSection> Diff<T>(this IEnumerable<T> source, IEnumerable<T> destination)
        {
            return Diff(source, destination, EqualityComparer<T>.Default);
        }

        /// <summary>
        /// Computes the diff between two sequences.
        /// </summary>
        /// <remarks>
        /// Since computing the diff requires an <see cref="IList{T}"/>, this method executes the <see cref="IEnumerable{T}"/>.
        /// </remarks>
        /// <typeparam name="T">The <see cref="Type"/> of element.</typeparam>
        /// <param name="source">The source sequence.</param>
        /// <param name="destination">The sequence, <paramref name="source"/> shall be transformed into.</param>
        /// <param name="comparer">An <see cref="IEqualityComparer{T}"/> to test for equality.</param>
        /// <returns>The diff data.</returns>
        public static IEnumerable<DiffSection> Diff<T>(this IEnumerable<T> source, IEnumerable<T> destination, IEqualityComparer<T> comparer)
        {
            Requires.NotNull(source, nameof(source));
            Requires.NotNull(destination, nameof(destination));

            return Diff(source.ToList(), destination.ToList(), comparer);
        }

        /// <summary>
        /// Computes the diff between two sequences.
        /// </summary>
        /// <remarks>
        /// Since computing the diff requires an <see cref="IList{T}"/>, this method executes the <see cref="IEnumerable{T}"/>.
        /// </remarks>
        /// <typeparam name="T">The <see cref="Type"/> of element.</typeparam>
        /// <typeparam name="TComparand">The <see cref="Type"/> of element to be compared.</typeparam>
        /// <param name="source">The source sequence.</param>
        /// <param name="destination">The sequence, <paramref name="source"/> shall be transformed into.</param>
        /// <param name="selector">A selector to determine the value to use to compare the elements for equality.</param>
        /// <returns>The diff data.</returns>
        public static IEnumerable<DiffSection> Diff<T, TComparand>(this IEnumerable<T> source, IEnumerable<T> destination, Func<T, TComparand> selector)
            where TComparand : IComparable<TComparand>
        {
            return Diff(source, destination, selector, EqualityComparer<TComparand>.Default);
        }

        /// <summary>
        /// Computes the diff between two sequences.
        /// </summary>
        /// <remarks>
        /// Since computing the diff requires an <see cref="IList{T}"/>, this method executes the <see cref="IEnumerable{T}"/>.
        /// </remarks>
        /// <typeparam name="T">The <see cref="Type"/> of element.</typeparam>
        /// <typeparam name="TComparand">The <see cref="Type"/> of element to be compared.</typeparam>
        /// <param name="source">The source sequence.</param>
        /// <param name="destination">The sequence, <paramref name="source"/> shall be transformed into.</param>
        /// <param name="selector">A selector to determine the value to use to compare the elements for equality.</param>
        /// <param name="comparer">An <see cref="IEqualityComparer{T}"/> to test for equality.</param>
        /// <returns>The diff data.</returns>
        public static IEnumerable<DiffSection> Diff<T, TComparand>(this IEnumerable<T> source, IEnumerable<T> destination, Func<T, TComparand> selector, IEqualityComparer<TComparand> comparer)
            where TComparand : IComparable<TComparand>
        {
            Requires.NotNull(source, nameof(source));
            Requires.NotNull(destination, nameof(destination));

            return Diff(source.ToList(), destination.ToList(), selector, comparer);
        }

        /// <summary>
        /// Computes the diff between two sequences.
        /// </summary>
        /// <typeparam name="T">The <see cref="Type"/> of element.</typeparam>
        /// <param name="source">The source sequence.</param>
        /// <param name="destination">The sequence, <paramref name="source"/> shall be transformed into.</param>
        /// <returns>The diff data.</returns>
        public static IEnumerable<DiffSection> Diff<T>(this IList<T> source, IList<T> destination)
        {
            return Diffy.Diff.Compute(source, destination);
        }

        /// <summary>
        /// Computes the diff between two sequences.
        /// </summary>
        /// <typeparam name="T">The <see cref="Type"/> of element.</typeparam>
        /// <param name="source">The source sequence.</param>
        /// <param name="destination">The sequence, <paramref name="source"/> shall be transformed into.</param>
        /// <param name="comparer">An <see cref="IEqualityComparer{T}"/> to test for equality.</param>
        /// <returns>The diff data.</returns>
        public static IEnumerable<DiffSection> Diff<T>(this IList<T> source, IList<T> destination, IEqualityComparer<T> comparer)
        {
            return Diffy.Diff.Compute(source, destination, comparer);
        }

        /// <summary>
        /// Computes the diff between two sequences.
        /// </summary>
        /// <typeparam name="T">The <see cref="Type"/> of element.</typeparam>
        /// <typeparam name="TComparand">The <see cref="Type"/> of element to be compared.</typeparam>
        /// <param name="source">The source sequence.</param>
        /// <param name="destination">The sequence, <paramref name="source"/> shall be transformed into.</param>
        /// <param name="selector">A selector to determine the value to use to compare the elements for equality.</param>
        /// <returns>The diff data.</returns>
        public static IEnumerable<DiffSection> Diff<T, TComparand>(this IList<T> source, IList<T> destination, Func<T, TComparand> selector)
            where TComparand : IComparable<TComparand>
        {
            return Diff(source, destination, selector, EqualityComparer<TComparand>.Default);
        }

        /// <summary>
        /// Computes the diff between two sequences.
        /// </summary>
        /// <typeparam name="T">The <see cref="Type"/> of element.</typeparam>
        /// <typeparam name="TComparand">The <see cref="Type"/> of element to be compared.</typeparam>
        /// <param name="source">The source sequence.</param>
        /// <param name="destination">The sequence, <paramref name="source"/> shall be transformed into.</param>
        /// <param name="selector">A selector to determine the value to use to compare the elements for equality.</param>
        /// <param name="comparer">An <see cref="IEqualityComparer{T}"/> to test for equality.</param>
        /// <returns>The diff data.</returns>
        public static IEnumerable<DiffSection> Diff<T, TComparand>(this IList<T> source, IList<T> destination, Func<T, TComparand> selector, IEqualityComparer<TComparand> comparer)
            where TComparand : IComparable<TComparand>
        {
            Requires.NotNull(source, nameof(source));
            Requires.NotNull(destination, nameof(destination));
            Requires.NotNull(selector, nameof(selector));

            List<TComparand> newSource = source.Select(selector).ToList();
            List<TComparand> newDestination = destination.Select(selector).ToList();
            return Diffy.Diff.Compute(newSource, newDestination, comparer);
        }

        /// <summary>
        /// Transforms the <paramref name="source"/> into the <paramref name="destination"/>.
        /// </summary>
        /// <remarks>
        /// This method is optimized for <see cref="List{T}"/> as it makes use of 
        /// <see cref="List{T}.InsertRange(int, IEnumerable{T})"/> and <see cref="List{T}.RemoveRange(int, int)"/>
        /// as often as possible to save method calls.
        /// </remarks>
        /// <typeparam name="T">The <see cref="Type"/> of element.</typeparam>
        /// <param name="source">The source sequence.</param>
        /// <param name="destination">The sequence, <paramref name="source"/> shall be transformed into.</param>
        public static void TransformTo<T>(this IList<T> source, IList<T> destination)
        {
            TransformTo(source, destination, EqualityComparer<T>.Default);
        }

        /// <summary>
        /// Transforms the <paramref name="source"/> into the <paramref name="destination"/>.
        /// </summary>
        /// <remarks>
        /// This method is optimized for <see cref="List{T}"/> as it makes use of 
        /// <see cref="List{T}.InsertRange(int, IEnumerable{T})"/> and <see cref="List{T}.RemoveRange(int, int)"/>
        /// as often as possible to save method calls.
        /// </remarks>
        /// <typeparam name="T">The <see cref="Type"/> of element.</typeparam>
        /// <param name="source">The source sequence.</param>
        /// <param name="destination">The sequence, <paramref name="source"/> shall be transformed into.</param>
        /// <param name="comparer">An <see cref="IEqualityComparer{T}"/> to test for equality.</param>
        public static void TransformTo<T>(this IList<T> source, IList<T> destination, IEqualityComparer<T> comparer)
        {
            List<T> srcList = source as List<T>;
            TransformTo(
                source, destination, comparer,
                (srcList != null) ? srcList.InsertRange : (InsertRangeDelegate<T>)null,
                (srcList != null) ? srcList.RemoveRange : (RemoveRangeDelegate)null
            );
        }

        /// <summary>
        /// Transforms the <paramref name="source"/> into the <paramref name="destination"/>.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Since lots of calls to .Insert / .Remove may generate lots of events on the UI thread, 
        /// this method allows you to specify the methods for .InsertRange and .RemoveRange. Those methods
        /// will be used instead of the ones for single elements.
        /// </para>
        /// <para>
        /// Use this overload, if your actual list type is not <see cref="List{T}"/>.
        /// </para>
        /// </remarks>
        /// <typeparam name="T">The <see cref="Type"/> of element.</typeparam>
        /// <param name="source">The source sequence.</param>
        /// <param name="destination">The sequence, <paramref name="source"/> shall be transformed into.</param>
        /// <param name="comparer">An <see cref="IEqualityComparer{T}"/> to test for equality.</param>
        /// <param name="sourceInsertRangeDelegate">The InsertRange-method of the source element. May be null.</param>
        /// <param name="sourceRemoveRangeDelegate">The RemoveRange-method of the source element. May be null.</param>
        public static void TransformTo<T>(
            this IList<T> source, IList<T> destination, IEqualityComparer<T> comparer,
            InsertRangeDelegate<T> sourceInsertRangeDelegate, RemoveRangeDelegate sourceRemoveRangeDelegate)
        {
            Requires.NotNull(source, nameof(source));
            Requires.NotNull(destination, nameof(destination));
            Requires.NotNull(comparer, nameof(comparer));

            TransformTo(
                source, 
                destination, 
                Diffy.Diff.Compute(source, destination, comparer), 
                sourceInsertRangeDelegate, 
                sourceRemoveRangeDelegate
            );
        }

        /// <summary>
        /// Applies the given <paramref name="diff"/> and transforms the <paramref name="source"/> into the <paramref name="destination"/>.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Since lots of calls to .Insert / .Remove may generate lots of events on the UI thread, 
        /// this method allows you to specify the methods for .InsertRange and .RemoveRange. Those methods
        /// will be used instead of the ones for single elements.
        /// </para>
        /// <para>
        /// Use this overload, if your actual list type is not <see cref="List{T}"/>.
        /// </para>
        /// </remarks>
        /// <typeparam name="T">The <see cref="Type"/> of element.</typeparam>
        /// <param name="source">The source sequence.</param>
        /// <param name="destination">The sequence, <paramref name="source"/> shall be transformed into.</param>
        /// <param name="diff">The diff to apply.</param>
        /// <param name="sourceInsertRangeDelegate">The InsertRange-method of the source element. May be null.</param>
        /// <param name="sourceRemoveRangeDelegate">The RemoveRange-method of the source element. May be null.</param>
        public static void TransformTo<T>(
            this IList<T> source, IList<T> destination, IEnumerable<DiffSection> diff,
            InsertRangeDelegate<T> sourceInsertRangeDelegate, RemoveRangeDelegate sourceRemoveRangeDelegate)
        {
            Requires.NotNull(source, nameof(source));
            Requires.NotNull(destination, nameof(destination));
            Requires.NotNull(diff, nameof(diff));
            
            int destIndex = 0;
            int sourceIndex = 0;
            foreach (DiffSection section in diff) {
                // Try to use the *Range-methods to reduce events on GUI thread
                switch (section.Type) {
                    case DiffSectionType.Copy:
                        destIndex += section.Length;
                        sourceIndex += section.Length;
                        break;
                    case DiffSectionType.Insert:
                        if (section.Length > 1 && sourceInsertRangeDelegate != null) {
                            IEnumerable<T> subset = destination.Skip(destIndex).Take(section.Length);
                            sourceInsertRangeDelegate(sourceIndex, subset);
                            destIndex += section.Length;
                            sourceIndex += section.Length;
                        } else {
                            for (int i = 0; i < section.Length; i++, sourceIndex++, destIndex++) {
                                source.Insert(sourceIndex, destination[destIndex]);
                            }
                        }
                        break;
                    case DiffSectionType.Delete:
                        if (section.Length > 1 && sourceRemoveRangeDelegate != null) {
                            sourceRemoveRangeDelegate(sourceIndex, section.Length);
                        } else {
                            for (int i = 0; i < section.Length; i++) {
                                source.RemoveAt(sourceIndex);
                            }
                        }
                        break;
                    default:
                        throw new InvalidOperationException(nameof(DiffSectionType) + "-enum value was invalid! " + section.Type);
                }
            }
        }
    }
}
