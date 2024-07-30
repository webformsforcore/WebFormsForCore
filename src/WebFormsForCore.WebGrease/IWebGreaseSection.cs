// ---------------------------------------------------------------------
// <copyright file="IWebGreaseSection.cs" company="Microsoft">
//    Copyright Microsoft Corporation, all rights reserved
// </copyright>
// ---------------------------------------------------------------------
namespace WebGrease
{
    using System;

    using WebGrease.Configuration;

    /// <summary>The Section interface.</summary>
    public interface IWebGreaseSection
    {
        /// <summary>Executes the action for the section.</summary>
        /// <param name="action">The action to execute.</param>
        void Execute(Action action);

        /// <summary>Executes the action for the section and returns the result.</summary>
        /// <param name="action">The action to execute.</param>
        /// <typeparam name="T">The type fo the result of the action.</typeparam>
        /// <returns>The result of type T.</returns>
        T Execute<T>(Func<T> action);

        /// <summary>Makes the section cachable.</summary>
        /// <param name="varBySettings">The settings to var by.</param>
        /// <param name="isSkipable">Determines if the cache is skipable.</param>
        /// <param name="infiniteWaitForLock">Should the lock wait infinitely (not use a timeout)</param>
        /// <returns>The <see cref="ICachableWebGreaseSection"/>.</returns>
        ICachableWebGreaseSection MakeCachable(object varBySettings, bool isSkipable = false, bool infiniteWaitForLock = false);

        /// <summary>Makes the section cachable.</summary>
        /// <param name="varByContentItem">The content item to vary by.</param>
        /// <param name="varBySettings">The settings to var by.</param>
        /// <param name="isSkipable">Determines if the cache is skipable.</param>
        /// <param name="infiniteWaitForLock">Should the lock wait infinitely (not use a timeout)</param>
        /// <returns>The <see cref="ICachableWebGreaseSection"/>.</returns>
        ICachableWebGreaseSection MakeCachable(ContentItem varByContentItem, object varBySettings = null, bool isSkipable = false, bool infiniteWaitForLock = false);

        /// <summary>Makes the section cachable.</summary>
        /// <param name="varByFileSet">The var By File Set.</param>
        /// <param name="varBySettings">The settings to var by.</param>
        /// <param name="isSkipable">Determines if the cache is skipable.</param>
        /// <param name="infiniteWaitForLock">Should the lock wait infinitely (not use a timeout)</param>
        /// <returns>The <see cref="ICachableWebGreaseSection"/>.</returns>
        ICachableWebGreaseSection MakeCachable(IFileSet varByFileSet, object varBySettings = null, bool isSkipable = false, bool infiniteWaitForLock = false);
    }
}