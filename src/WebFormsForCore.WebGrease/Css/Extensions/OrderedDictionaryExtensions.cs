// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OrderedDictionaryExtensions.cs" company="Microsoft">
//   Copyright Microsoft Corporation, all rights reserved
// </copyright>
// <summary>
//   OrderedDictionaryExtensions Class - Provides the extension on OrderedDictionary
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace WebGrease.Css.Extensions
{
    using System;
    using System.Collections.Specialized;

    /// <summary>OrderedDictionaryExtensions Class - Provides the extension on OrderedDictionaryExtensions</summary>
    public static class OrderedDictionaryExtensions
    {
        /// <summary>
        /// Appends an item to an ordered dictionary with the given key. If there is already an item in the ordered
        /// dictionary with the same key, it is removed first so that the item appears at the end of the list, not at
        /// the original location.
        /// </summary>
        /// <typeparam name="TItem">item type</typeparam>
        /// <param name="dictionary">ordered dictionary of items</param>
        /// <param name="item">item to insert</param>
        /// <param name="key">key to use</param>
        public static void AppendWithOverride<TItem>(this OrderedDictionary dictionary, TItem item, Func<TItem, object> key)
        {
            // dictionary can't be null or we'd throw errors. But item can be null -- the caller may want to insert a null
            // object in the dictionary for a given key (although the key function will need to be able to handle that).
            if (dictionary != null)
            {
                // if the dictionary already has this key, remove it from its original location.
                var keyValue = key(item);
                if (dictionary.Contains(keyValue))
                {
                    dictionary.Remove(keyValue);
                }

                // add the property to the end of the ordered dictionary
                dictionary.Add(keyValue, item);
            }
        }
    }
}
