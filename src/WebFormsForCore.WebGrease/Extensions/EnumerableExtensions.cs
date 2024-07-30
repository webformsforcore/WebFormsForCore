// ----------------------------------------------------------------------------------------------------
// <copyright file="EnumerableExtensions.cs" company="Microsoft Corporation">
//   Copyright Microsoft Corporation, all rights reserved.
// </copyright>
// <summary>
//   
// </summary>
// ----------------------------------------------------------------------------------------------------

namespace WebGrease.Extensions
{
    using System;
    using System.Collections;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;

    using WebGrease.Configuration;
    using WebGrease.Css.Extensions;

    /// <summary>Enumerable extensions.</summary>
    internal static class EnumerableExtensions
    {
        #region Methods

        /// <summary>The has at least.</summary>
        /// <param name="source">The source.</param>
        /// <param name="atLeast">The at least.</param>
        /// <typeparam name="T">The type of the item in the enumrable.</typeparam>
        /// <returns>The <see cref="bool"/>.</returns>
        internal static bool HasAtLeast<T>(this IEnumerable<T> source, int atLeast)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }

            var count = 0;
            using (var enumerator = source.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    if (++count == atLeast)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>The distinct by extension method.</summary>
        /// <param name="source">The source.</param>
        /// <param name="keySelector">The key selector.</param>
        /// <typeparam name="TSource">The source type</typeparam>
        /// <typeparam name="TKey">Type of the key</typeparam>
        /// <returns>The <see cref="IEnumerable"/>.</returns>
        internal static IEnumerable<TSource> DistinctBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
        {
            var hash = new HashSet<TKey>();
            return source.Where(p => hash.Add(keySelector(p)));
        }

        /// <summary>Adds a dictionary to another dictionary with the same types.</summary>
        /// <param name="dictionary">The dictionary to add to.</param>
        /// <param name="range">The dictionary to add.</param>
        /// <typeparam name="TKey">The type of Key</typeparam>
        /// <typeparam name="TValue">The type of Value</typeparam>
        internal static void AddRange<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, IEnumerable<KeyValuePair<TKey, TValue>> range)
        {
            range.ForEach(dictionary.Add);
        }

        /// <summary>Adds the AddRange method for the thread safe BlockingCollection object.</summary>
        /// <param name="collection">The blocking collection.</param>
        /// <param name="range">The items to add.</param>
        /// <typeparam name="TValue">The Type of the items in the collection.</typeparam>
        internal static void AddRange<TValue>(this BlockingCollection<TValue> collection, IEnumerable<TValue> range)
        {
            range.ForEach(collection.Add);
        }

        #endregion

        /// <summary>Adds dictionary values of one to another dictionary with int values.</summary>
        /// <param name="dictionary1">The dictionary 1.</param>
        /// <param name="dictionary2">The dictionary 2.</param>
        /// <typeparam name="TKey">The type of Key</typeparam>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "Extension method")]
        internal static void Add<TKey>(this IDictionary<TKey, double> dictionary1, IEnumerable<KeyValuePair<TKey, double>> dictionary2)
        {
            foreach (var kvp2 in dictionary2)
            {
                var key = kvp2.Key;
                if (!dictionary1.ContainsKey(key))
                {
                    dictionary1[key] = 0;
                }

                dictionary1[key] += kvp2.Value;
            }
        }

        /// <summary>Tries to get a value from the dictionary, returns TValue default if not found.</summary>
        /// <param name="dictionary">The dictionary.</param>
        /// <param name="key">The key.</param>
        /// <typeparam name="TKey">The type of the Key</typeparam>
        /// <typeparam name="TValue">The type of the Value</typeparam>
        /// <returns>The value or default(TValue) if not found.</returns>
        internal static TValue TryGetValue<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key)
        {
            if (dictionary == null)
            {
                throw new ArgumentNullException("dictionary");
            }

            TValue value;
            return
                dictionary.TryGetValue(key, out value)
                    ? value
                    : default(TValue);
        }

        /// <summary>Adds dictionary values of one to another dictionary with double values.</summary>
        /// <param name="dictionary1">The dictionary 1.</param>
        /// <param name="dictionary2">The dictionary 2.</param>
        /// <typeparam name="TKey">The type of Key</typeparam>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "Extension method")]
        internal static void Add<TKey>(this IDictionary<TKey, int> dictionary1, IEnumerable<KeyValuePair<TKey, int>> dictionary2)
        {
            foreach (var kvp2 in dictionary2)
            {
                var key = kvp2.Key;
                if (!dictionary1.ContainsKey(key))
                {
                    dictionary1[key] = 0;
                }

                dictionary1[key] += kvp2.Value;
            }
        }

        /// <summary>Adds a named config to a configurations dictionary.</summary>
        /// <param name="configs">The configurations.</param>
        /// <param name="config">The config.</param>
        /// <typeparam name="TConfig">The configuration type</typeparam>
        internal static void AddNamedConfig<TConfig>(this IDictionary<string, TConfig> configs, TConfig config) where TConfig : INamedConfig, new()
        {
            configs[config.Name ?? string.Empty] = config;
        }

        /// <summary>
        /// Gets the named configuration from the dictionary, or the first config if no name is passed or returns a default config if not found.
        /// </summary>
        /// <typeparam name="T">ConfigurationType to retrieve</typeparam>
        /// <param name="configDictionary">Dictionary of config objects</param>
        /// <param name="configName">Named configuration to find</param>
        /// <returns>the configuration object.</returns>
        internal static T GetNamedConfig<T>(this IDictionary<string, T> configDictionary, string configName = null)
            where T : class, INamedConfig, new()
        {
            // no configs return default(T) / null
            if (configDictionary == null || !configDictionary.Any())
            {
                return new T();
            }

            configName = configName.AsNullIfWhiteSpace() ?? string.Empty;

            // Ask for indeterminite, first try and find one without, otherwise return first.
            // try and return name specific, ptherwise return the one with null (no config="" set)
            return configDictionary.TryGetValue(configName)
                   ?? configDictionary.TryGetValue(string.Empty)
                   ?? (configName.IsNullOrWhitespace() ? configDictionary.FirstOrDefault().Value : null)
                   ?? new T();
        }

        /// <summary>The if not null.</summary>
        /// <param name="obj">The obj.</param>
        /// <param name="action">The action.</param>
        /// <typeparam name="TObject">The type of object</typeparam>
        /// <typeparam name="TResult">The type of the result</typeparam>
        /// <returns>The <see cref="TResult"/>.</returns>
        internal static TResult NullSafeAction<TObject, TResult>(this TObject obj, Func<TObject, TResult> action)
        {
            if (action == null)
            {
                throw new ArgumentNullException("action");
            }

            if (obj != null)
            {
                return action(obj);
            }

            return default(TResult);
        }
    }
}