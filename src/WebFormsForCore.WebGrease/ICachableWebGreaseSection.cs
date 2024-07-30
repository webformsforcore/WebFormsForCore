// ---------------------------------------------------------------------
// <copyright file="ICachableWebGreaseSection.cs" company="Microsoft">
//    Copyright Microsoft Corporation, all rights reserved
// </copyright>
// ---------------------------------------------------------------------
namespace WebGrease
{
    using System;

    /// <summary>
    /// The CachableSection interface provides the method used by a cachable section, this is returned by using isCachable in a "normal web grease section.
    /// </summary>
    public interface ICachableWebGreaseSection
    {
        /// <summary>
        /// Executes the action for the cached section
        /// the boolean/success result of the action will determine if the action will be stored in cache.
        /// </summary>
        /// <param name="cachableSectionAction">The section action.</param>
        /// <returns>If all was successfull (being passed from within the actions).</returns>
        bool Execute(Func<ICacheSection, bool> cachableSectionAction);

        /// <summary>Sets the restore action</summary>
        /// <param name="action">The restore section action.</param>
        /// <returns>The <see cref="ICachableWebGreaseSection"/>.</returns>
        ICachableWebGreaseSection RestoreFromCacheAction(Func<ICacheSection, bool> action);

        /// <summary>The when skipped.</summary>
        /// <param name="action">The action.</param>
        /// <returns>The <see cref="ICachableWebGreaseSection"/>.</returns>
        ICachableWebGreaseSection WhenSkipped(Action<ICacheSection> action);
    }
}