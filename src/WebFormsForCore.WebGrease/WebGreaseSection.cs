// ---------------------------------------------------------------------
// <copyright file="WebGreaseSection.cs" company="Microsoft">
//    Copyright Microsoft Corporation, all rights reserved
// </copyright>
// ---------------------------------------------------------------------
namespace WebGrease
{
    using System;
    using System.Collections.Concurrent;
    using WebGrease.Configuration;

    /// <summary>The section.</summary>
    public class WebGreaseSection : IWebGreaseSection, ICachableWebGreaseSection
    {
        /// <summary>The thread locks.</summary>
        private static readonly ConcurrentDictionary<string, object> SectionLocks = new ConcurrentDictionary<string, object>();

        /// <summary>The is group.</summary>
        private readonly bool isGroup;

        /// <summary>The context.</summary>
        private readonly IWebGreaseContext context;

        /// <summary>The id parts.</summary>
        private readonly string[] idParts;

        /// <summary>The cache var by setting.</summary>
        private object cacheVarBySetting;

        /// <summary>The cache is skipable.</summary>
        private bool cacheIsSkipable;

        /// <summary>If the lock around the cache should wait indefinately and not use a timeout.</summary>
        private bool cacheInfiniteWaitForLock;

        /// <summary>The cache var by content item.</summary>
        private ContentItem cacheVarByContentItem;

        /// <summary>The cachevar by file set.</summary>
        private IFileSet cacheVarByFileSet;

        /// <summary>The restore from cache action.</summary>
        private Func<ICacheSection, bool> restoreFromCacheAction;

        /// <summary>The when skipped action.</summary>
        private Action<ICacheSection> whenSkippedAction;

        /// <summary>Initializes a new instance of the <see cref="WebGreaseSection"/> class.</summary>
        /// <param name="context">The context.</param>
        /// <param name="idParts">The id parts.</param>
        /// <param name="isGroup">The is group.</param>
        private WebGreaseSection(IWebGreaseContext context, string[] idParts, bool isGroup)
        {
            this.context = context;
            this.idParts = idParts;
            this.isGroup = isGroup;
        }

        /// <summary>The create.</summary>
        /// <param name="context">The context.</param>
        /// <param name="idParts">The id parts.</param>
        /// <param name="isGroup">The is group.</param>
        /// <returns>The <see cref="IWebGreaseSection"/>.</returns>
        public static IWebGreaseSection Create(IWebGreaseContext context, string[] idParts, bool isGroup)
        {
            return new WebGreaseSection(context, idParts, isGroup);
        }

        /// <summary>Executes the action for the section.</summary>
        /// <param name="action">The action to execute.</param>
        public void Execute(Action action)
        {
            this.context.Measure.Start(this.isGroup, this.idParts);
            try
            {
                action();
            }
            finally
            {
                this.context.Measure.End(this.isGroup, this.idParts);
            }
        }

        /// <summary>Executes the action for the section and returns the result, the parameter of the action is the id of the section.</summary>
        /// <param name="action">The action to execute.</param>
        /// <typeparam name="T">The type fo the result of the action.</typeparam>
        /// <returns>The result of type T.</returns>
        public T Execute<T>(Func<T> action)
        {
            this.context.Measure.Start(this.isGroup, this.idParts);
            try
            {
                return action();
            }
            finally
            {
                this.context.Measure.End(this.isGroup, this.idParts);
            }
        }

        /// <summary>Makes the section cachable.</summary>
        /// <param name="varBySettings">The settings to var by.</param>
        /// <param name="isSkipable">Determines if the cache is skipable.</param>
        /// <param name="infiniteWaitForLock">Should the lock wait infinitely (not use a timeout)</param>
        /// <returns>The <see cref="ICachableWebGreaseSection"/>.</returns>
        public ICachableWebGreaseSection MakeCachable(object varBySettings, bool isSkipable = false, bool infiniteWaitForLock = false)
        {
            this.MakeCachable(null as IFileSet, varBySettings, isSkipable, infiniteWaitForLock);
            return this;
        }

        /// <summary>Makes the section cachable.</summary>
        /// <param name="varByContentItem">The content item to vary by.</param>
        /// <param name="varBySettings">The settings to var by.</param>
        /// <param name="isSkipable">Determines if the cache is skipable.</param>
        /// <param name="infiniteWaitForLock">Should the lock wait infinitely (not use a timeout)</param>
        /// <returns>The <see cref="ICachableWebGreaseSection"/>.</returns>
        public ICachableWebGreaseSection MakeCachable(ContentItem varByContentItem, object varBySettings = null, bool isSkipable = false, bool infiniteWaitForLock = false)
        {
            this.cacheVarByContentItem = varByContentItem;
            this.cacheVarBySetting = varBySettings;
            this.cacheIsSkipable = isSkipable;
            this.cacheInfiniteWaitForLock = infiniteWaitForLock;
            return this;
        }

        /// <summary>Makes the section cachable.</summary>
        /// <param name="varByFileSet">The var By File Set.</param>
        /// <param name="varBySettings">The settings to var by.</param>
        /// <param name="isSkipable">Determines if the cache is skipable.</param>
        /// <param name="infiniteWaitForLock">Should the lock wait infinitely (not use a timeout)</param>
        /// <returns>The <see cref="ICachableWebGreaseSection"/>.</returns>
        public ICachableWebGreaseSection MakeCachable(IFileSet varByFileSet, object varBySettings = null, bool isSkipable = false, bool infiniteWaitForLock = false)
        {
            this.cacheVarByFileSet = varByFileSet;
            this.cacheVarBySetting = varBySettings;
            this.cacheIsSkipable = isSkipable;
            this.cacheInfiniteWaitForLock = infiniteWaitForLock;
            return this;
        }

        /// <summary>Sets the restore action</summary>
        /// <param name="action">The restore section action.</param>
        /// <returns>The <see cref="ICachableWebGreaseSection"/>.</returns>
        public ICachableWebGreaseSection RestoreFromCacheAction(Func<ICacheSection, bool> action)
        {
            this.restoreFromCacheAction = action;
            return this;
        }

        /// <summary>The when skipped.</summary>
        /// <param name="action">The action.</param>
        /// <returns>The <see cref="ICachableWebGreaseSection"/>.</returns>
        public ICachableWebGreaseSection WhenSkipped(Action<ICacheSection> action)
        {
            this.whenSkippedAction = action;
            return this;
        }

        /// <summary>
        /// Executes the action for the cached section
        /// the boolean/success result of the action will determine if the action will be stored in cache.
        /// </summary>
        /// <param name="cachableSectionAction">The section action.</param>
        /// <returns>If all was successfull (being passed from within the actions).</returns>
        public bool Execute(Func<ICacheSection, bool> cachableSectionAction)
        {
            var id = WebGreaseContext.ToStringId(this.idParts);
            var webGreaseSectionKey = new WebGreaseSectionKey(this.context, id, this.cacheVarByContentItem, this.cacheVarBySetting, this.cacheVarByFileSet);
            var sectionLock = SectionLocks.GetOrAdd(webGreaseSectionKey.Value, new object());

            return Safe.Lock(
                sectionLock, 
                this.cacheInfiniteWaitForLock ? Safe.MaxLockTimeout : Safe.DefaultLockTimeout, 
                () =>
                {
                    var errorHasOccurred = false;
                    EventHandler logOnErrorOccurred = delegate { errorHasOccurred = true; };
                    this.context.Log.ErrorOccurred += logOnErrorOccurred;
                    var cacheSection = this.context.Cache.BeginSection(webGreaseSectionKey);

                    try
                    {
                        if (this.context.TemporaryIgnore(this.cacheVarByFileSet, this.cacheVarByContentItem) && !errorHasOccurred)
                        {
                            cacheSection.Save();
                            return true;
                        }

                        cacheSection.Load();
                        if (this.cacheIsSkipable && cacheSection.CanBeSkipped())
                        {
                            if (this.whenSkippedAction != null)
                            {
                                this.whenSkippedAction(cacheSection);
                            }

                            if (!errorHasOccurred)
                            {
                                return true;
                            }
                        }

                        if (this.restoreFromCacheAction != null && cacheSection.CanBeRestoredFromCache())
                        {
                            if (this.restoreFromCacheAction(cacheSection) && !errorHasOccurred)
                            {
                                return true;
                            }
                        }

                        this.context.Measure.Start(this.isGroup, this.idParts);
                        try
                        {
                            if (!cachableSectionAction(cacheSection) || errorHasOccurred)
                            {
                                return false;
                            }

                            cacheSection.Save();
                            return true;
                        }
                        finally
                        {
                            this.context.Measure.End(this.isGroup, this.idParts);
                        }
                    }
                    finally
                    {
                        this.context.Log.ErrorOccurred -= logOnErrorOccurred;
                        cacheSection.EndSection();
                    }
                });
        }
    }
}