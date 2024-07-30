// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ResourcesResolutionActivity.cs" company="Microsoft">
//   Copyright Microsoft Corporation, all rights reserved
// </copyright>
// <summary>
//   The class which is responsible for resolving the resources hierarchy.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace WebGrease.Activities
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;

    /// <summary>The class which is responsible for resolving the resources hierarchy.</summary>
    internal sealed class ResourcesResolutionActivity
    {
        /// <summary>The empty result.</summary>
        private static readonly Dictionary<string, IDictionary<string, string>> EmptyResult = new Dictionary<string, IDictionary<string, string>>();

        /// <summary>The context.</summary>
        private readonly IWebGreaseContext context;

        /// <summary>Initializes a new instance of the <see cref="ResourcesResolutionActivity"/> class.</summary>
        /// <param name="context">The context.</param>
        public ResourcesResolutionActivity(IWebGreaseContext context)
        {
            this.context = context;
            this.ResourceKeys = new List<string>();
        }

        /// <summary>
        /// Gets or sets the paths to the base directory say "Content" which is the root path of application and features resources.
        /// </summary>
        /// <value>The root path of application and features resources.</value>
        internal string SourceDirectory { get; set; }

        /// <summary>
        /// Gets or sets the directory such as "Locales", "Themes"
        /// </summary>
        /// <value>The type of resources to process. Used for directory names.</value>
        internal string ResourceGroupKey { get; set; }

        /// <summary>
        /// Gets or sets the directory App directory which would contain the list of sites.
        /// </summary>
        /// <value>The feature aggregated resources folder path.</value>
        internal string ApplicationDirectoryName { get; set; }

        /// <summary>
        /// Gets or sets the site name which would contain the site overriden resources
        /// </summary>
        /// <value>The site name.</value>
        internal string SiteDirectoryName { get; set; }

        /// <summary>
        /// Gets or sets the destination path where the compressed resources will be
        /// written to the hard drive
        /// </summary>
        /// <value>The output Resources folder path.</value>
        internal string DestinationDirectory { get; set; }

        /// <summary>
        /// Gets the list of locales or themes to generate the resources for.
        /// </summary>
        /// <value>The list of locales or themes.</value>
        internal List<string> ResourceKeys { get; private set; }

        /// <summary>Gets or sets the measure name.</summary>
        internal FileTypes FileType { get; set; }

        /// <summary>The get merged resources.</summary>
        /// <returns>The merged resources.</returns>
        internal IDictionary<string, IDictionary<string, string>> GetMergedResources()
        {
            if (!this.HasSomethingToResolve())
            {
                return EmptyResult;
            }

            return this.context.SectionedAction(SectionIdParts.ResourcesResolutionActivity, this.FileType.ToString(), this.ResourceGroupKey).Execute(() =>
            {
                try
                {
                    var resourcesResolver = ResourcesResolver.Factory(this.context, this.SourceDirectory, this.ResourceGroupKey, this.ApplicationDirectoryName, this.SiteDirectoryName, this.ResourceKeys, this.DestinationDirectory);
                    return resourcesResolver.GetMergedResources();
                }
                catch (ResourceOverrideException resourceOverrideException)
                {
                    // There was a token override in folder path that does not
                    // allow token overriding. For this case, we need to
                    // show a build error.
                    var errorMessage = string.Format(CultureInfo.InvariantCulture, "ResourcesResolutionActivity - {0} has more than one value assigned. Only one value per key name is allowed in libraries and features. Resource key overrides are allowed at the product level only.", resourceOverrideException.TokenKey);
                    throw new WorkflowException(errorMessage, resourceOverrideException);
                }
                catch (Exception exception)
                {
                    throw new WorkflowException("ResourcesResolutionActivity - Error happened while executing the resolve resources activity", exception);
                }
            });
        }

        /// <summary>When overridden in a derived class, executes the task.</summary>
        internal void Execute()
        {
            if (!this.HasSomethingToResolve())
            {
                return;
            }

            this.context.SectionedAction(SectionIdParts.ResourcesResolutionActivity, this.FileType.ToString(), this.ResourceGroupKey).Execute(() =>
            {
                try
                {
                    var resourcesResolver = ResourcesResolver.Factory(this.context, this.SourceDirectory, this.ResourceGroupKey, this.ApplicationDirectoryName, this.SiteDirectoryName, this.ResourceKeys, this.DestinationDirectory);
                    resourcesResolver.ResolveHierarchy();
                }
                catch (ResourceOverrideException resourceOverrideException)
                {
                    // There was a token override in folder path that does not
                    // allow token overriding. For this case, we need to
                    // show a build error.
                    var errorMessage = string.Format(CultureInfo.InvariantCulture, "ResourcesResolutionActivity - {0} has more than one value assigned. Only one value per key name is allowed in libraries and features. Resource key overrides are allowed at the product level only.", resourceOverrideException.TokenKey);
                    throw new WorkflowException(errorMessage, resourceOverrideException);
                }
                catch (Exception exception)
                {
                    throw new WorkflowException("ResourcesResolutionActivity - Error happened while executing the resolve resources activity", exception);
                }
            });
        }

        /// <summary>Check if it has anything to resolve.</summary>
        /// <returns>The <see cref="bool"/>.</returns>
        private bool HasSomethingToResolve()
        {
            return 
                this.ResourceKeys != null 
                && this.ResourceKeys.Any() 
                && !string.IsNullOrWhiteSpace(this.SourceDirectory) 
                && Directory.Exists(this.SourceDirectory);
        }
    }
}
