// ----------------------------------------------------------------------------------------------------
// <copyright file="PreprocessingManager.cs" company="Microsoft Corporation">
//   Copyright Microsoft Corporation, all rights reserved.
// </copyright>
// ----------------------------------------------------------------------------------------------------

namespace WebGrease.Preprocessing
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using System.ComponentModel.Composition.Hosting;
    using System.IO;
    using System.Linq;
    using System.Reflection;

    using WebGrease.Configuration;
    using WebGrease.Css.Extensions;
    using WebGrease.Extensions;

    /// <summary>
    /// This is the preprocessing manager, it manages all the loaded preprocessors, and the calls to them
    /// </summary>
    public class PreprocessingManager
    {
        #region Fields

        /// <summary>The registered preprocessing engines.</summary>
        [ImportMany(typeof(IPreprocessingEngine))]
        private readonly IList<IPreprocessingEngine> registeredPreprocessingEngines = new List<IPreprocessingEngine>();

        /// <summary>The context.</summary>
        private IWebGreaseContext context;

        #endregion

        #region Constructors and Destructors

        /// <summary>Initializes a new instance of the <see cref="PreprocessingManager"/> class. 
        /// Initializes a new instance of the <see cref="PreprocessingManager"/>.
        /// Can only be called from within the class by the Singleton construction.</summary>
        /// <param name="webGreaseConfiguration">The web Grease Configuration.</param>
        /// <param name="logManager">The log Manager.</param>
        /// <param name="timeMeasure">The time Measure.</param>
        internal PreprocessingManager(WebGreaseConfiguration webGreaseConfiguration, LogManager logManager, ITimeMeasure timeMeasure)
        {
            if (webGreaseConfiguration == null)
            {
                throw new ArgumentNullException("webGreaseConfiguration");
            }

            if (logManager == null)
            {
                throw new ArgumentNullException("logManager");
            }

            if (timeMeasure == null)
            {
                throw new ArgumentNullException("timeMeasure");
            }

            this.Initialize(webGreaseConfiguration.PreprocessingPluginPath, logManager, timeMeasure);
        }

        /// <summary>Initializes a new instance of the <see cref="PreprocessingManager"/> class.</summary>
        /// <param name="preprocessingManager">The preprocessing manager.</param>
        internal PreprocessingManager(PreprocessingManager preprocessingManager)
        {
            preprocessingManager.registeredPreprocessingEngines.ForEach(rp => this.registeredPreprocessingEngines.Add(rp));
        }

        /// <summary>Set the current context.</summary>
        /// <param name="webGreaseContext">The web grease context.</param>
        internal void SetContext(IWebGreaseContext webGreaseContext)
        {
            this.context = webGreaseContext;
        }

        /// <summary>This will call any of the registered preprocessor plugins, that are named in the provided preprocessing config, in the order they are configured.
        /// It will only call the preprocessor if it reports it can handle the filetype. (Using Canprocess).
        /// It will loop through all of them and then return the processed file.
        /// A null value returned by the preprocessors indicates an exceptipon has occurred, and we should break of processing.
        /// The plugins themselves will report the detailed error through the logError and logExtendedError actions.</summary>
        /// <param name="contentItem">The content Item.</param>
        /// <param name="preprocessConfig">The preprocessing config.</param>
        /// <param name="minimalOutput"></param>
        /// <returns>If no preprocessors are found, the passed in file contents.
        /// Or the result of the pre processors, or null if there was an error while calling the preprocessors.</returns>
        internal ContentItem Process(ContentItem contentItem, PreprocessingConfig preprocessConfig, bool minimalOutput = false)
        {
            // Select all the registered preprocessors that are named in the configguration in the order in which they appear in the config and are valid for this file type.
            this.context.Log.Information("Registered preprocessors to use: {0}".InvariantFormat(string.Join(";", preprocessConfig.PreprocessingEngines)));
            var preprocessorsToUse = this.GetProcessors(contentItem, preprocessConfig);
            if (!preprocessorsToUse.Any())
            {
                return contentItem;
            }

            this.context.SectionedAction(SectionIdParts.Preprocessing)
             .MakeCachable(contentItem, new { relativePath = Path.GetDirectoryName(contentItem.RelativeContentPath), preprocessConfig, pptu = preprocessorsToUse.Select(pptu => pptu.Name) })
             .RestoreFromCacheAction(cacheSection =>
             {
                 contentItem = cacheSection.GetCachedContentItem(CacheFileCategories.PreprocessingResult);
                 return contentItem != null;
             })
             .Execute(cacheSection =>
             {
                 // Loop through each available engine that was also configured
                 // And check if the engine can process the file
                 foreach (var preprocessingEngine in preprocessorsToUse)
                 {
                     this.context.Log.Information("preprocessing with: {0}".InvariantFormat(preprocessingEngine.Name));

                     // Get the new content
                     contentItem = preprocessingEngine.Process(this.context, contentItem, preprocessConfig, minimalOutput);

                     if (contentItem == null)
                     {
                         return false;
                     }
                 }

                 cacheSection.AddResult(contentItem, CacheFileCategories.PreprocessingResult);
                 return true;
             });

            return contentItem;
        }

        /// <summary>The get processors.</summary>
        /// <param name="contentItem">The content item.</param>
        /// <param name="preprocessConfig">The preprocess config.</param>
        /// <returns>The list of preprocessors that applies to the content item.</returns>
        internal IPreprocessingEngine[] GetProcessors(ContentItem contentItem, PreprocessingConfig preprocessConfig)
        {
            return preprocessConfig.PreprocessingEngines
                                   .SelectMany(ppe => this.registeredPreprocessingEngines.Where(rppe => rppe.Name.Equals(ppe, StringComparison.OrdinalIgnoreCase)))
                                   .Where(pptu => pptu.CanProcess(this.context, contentItem, preprocessConfig))
                                   .ToArray();
        }

        #endregion

        /// <summary>Initialized method to be called once in each application session by the caller planning to use the preprocessors.
        /// This will try to load the plugins from the plugin folder, and try to initialize each of them.
        /// Will report progess to the log information action.</summary>
        /// <param name="pluginPath">The plugin path</param>
        /// <param name="logManager">The log Manager.</param>
        /// <param name="timeMeasure">The time Measure.</param>
        private void Initialize(string pluginPath, LogManager logManager, ITimeMeasure timeMeasure)
        {
            timeMeasure.Start(false, SectionIdParts.Preprocessing, SectionIdParts.Initialize);
            logManager.Information(ResourceStrings.PreprocessingInitializeStart.InvariantFormat(pluginPath));

            // If no plugin path was provided, we use the assembly path.
            if (string.IsNullOrWhiteSpace(pluginPath))
            {
                var assemblyFilInfo = new FileInfo(Assembly.GetCallingAssembly().FullName);
                pluginPath = assemblyFilInfo.DirectoryName;
            }

            if (!string.IsNullOrWhiteSpace(pluginPath))
            {
                if (!Directory.Exists(pluginPath))
                {
                    logManager.Error(
                        new DirectoryNotFoundException(pluginPath),
                        ResourceStrings.PreprocessingCouldNotFindThePluginPath.InvariantFormat(pluginPath));
                    return;
                }

                // And now use MEF to load all possible plugins.
                logManager.Information(ResourceStrings.PreprocessingPluginPath.InvariantFormat(pluginPath));
                using (var addInCatalog = new AggregateCatalog())
                {
                    addInCatalog.Catalogs.Add(new DirectoryCatalog(pluginPath));

                    using (var addInContainer = new CompositionContainer(addInCatalog))
                    {
                        // Fill the imports of this object
                        try
                        {
                            addInContainer.ComposeParts(this);
                        }
                        catch (CompositionException compositionException)
                        {
                            logManager.Error(compositionException, ResourceStrings.PreprocessingLoadingError);
                        }

                        foreach (var registeredPreprocessingEngine in this.registeredPreprocessingEngines)
                        {
                            logManager.Information(
                                ResourceStrings.PreprocessingEngineFound.InvariantFormat(registeredPreprocessingEngine.Name));
                        }
                    }
                }
            }

            logManager.Information(ResourceStrings.PreprocessingInitializeEnd);
            timeMeasure.End(false, SectionIdParts.Preprocessing, SectionIdParts.Initialize);
        }
    }
}