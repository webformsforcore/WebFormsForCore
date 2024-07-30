// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ListExtensions.cs" company="Microsoft">
//   Copyright Microsoft Corporation, all rights reserved
// </copyright>
// <summary>
//   ListExtensions Class - Provides the extension on List
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace WebGrease.Css.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;

    /// <summary>ListExtensions Class - Provides the extension on List</summary>
    public static class ListExtensions
    {
        /// <summary>The generic version of AsReadOnly with null check</summary>
        /// <typeparam name="T">The type of items in collection</typeparam>
        /// <param name="list">The list of items</param>
        /// <returns>The safe readonly collection with null check</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1002:DoNotExposeGenericLists", Justification = "This is by design.")]
        public static ReadOnlyCollection<T> AsSafeReadOnly<T>(this List<T> list)
        {
            return list != null ? list.AsReadOnly() : null;
        }

        /// <summary>For each extension method for IEnumerable</summary>
        /// <typeparam name="T">The type of items in collection</typeparam>
        /// <param name="list">The list of items</param>
        /// <param name="action">The action to perform on items</param>
        public static void ForEach<T>(this IEnumerable<T> list, Action<T> action)
        {
            if (list == null || action == null)
            {
                return;
            }

            foreach (var item in list)
            {
                action(item);
            }
        }

        /// <summary>For each extension which has an index</summary>
        /// <typeparam name="T">The type of <paramref name="action"/> elements.</typeparam>
        /// <param name="list">The enumerable</param>
        /// <param name="action">The action with type and index</param>
        public static void ForEach<T>(this IEnumerable<T> list, Action<T, int> action)
        {
            if (list == null || action == null)
            {
                return;
            }

            var count = 0;
            foreach (var item in list)
            {
                action(item, count);
                count++;
            }
        }

        /// <summary>For each extension which has a boolean which indicates the last index</summary>
        /// <typeparam name="T">The type of <paramref name="action"/> elements.</typeparam>
        /// <param name="list">The list of items</param>
        /// <param name="action">The action with type and bool</param>
        public static void ForEach<T>(this IList<T> list, Action<T, bool> action)
        {
            if (list == null || action == null)
            {
                return;
            }

            var count = 0;
            foreach (var item in list)
            {
                action(item, count < list.Count - 1 ? false : true);
                count++;
            }
        }

        /// <summary>Walks the enumerable and converts to read only collection.</summary>
        /// <param name="enumerable">The enumerable.</param>
        /// <typeparam name="T">The type of items in list.</typeparam>
        /// <returns>The read only collection.</returns>
        public static ReadOnlyCollection<T> ToSafeReadOnlyCollection<T>(this IEnumerable<T> enumerable)
            where T : class
        {
            if (enumerable == null)
            {
                return null;
            }

            var collection = new List<T>(enumerable.Where(_ => _ != null));
            return collection.AsReadOnly();
        }
    }
}
