// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CacheSourceDependency.cs" company="Microsoft">
//   Copyright Microsoft Corporation, all rights reserved
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace WebGrease
{
    using System;
    using System.IO;
    using System.Linq;

    using WebGrease.Configuration;
    using WebGrease.Extensions;

    /// <summary>The cache source dependency.</summary>
    public class CacheSourceDependency
    {
        #region Public Properties

        /// <summary>Gets the input spec.</summary>
        public InputSpec InputSpec { get; private set; }

        /// <summary>Gets the input spec hash.</summary>
        public string InputSpecHash { get; private set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>Creates a source dependency.</summary>
        /// <param name="context">The context.</param>
        /// <param name="inputSpec">The input spec.</param>
        /// <returns>The <see cref="CacheSourceDependency"/>.</returns>
        internal static CacheSourceDependency Create(IWebGreaseContext context, InputSpec inputSpec)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }
            if (inputSpec == null)
            {
                throw new ArgumentNullException("inputSpec");
            }

            var csd = new CacheSourceDependency();
            if (Directory.Exists(inputSpec.Path))
            {
                inputSpec.Path.EnsureEndSeparator();
            }

            csd.InputSpecHash = GetInputSpecHash(context, inputSpec);
            inputSpec.Path = inputSpec.Path.MakeRelativeToDirectory(context.Configuration.SourceDirectory);
            csd.InputSpec = inputSpec;

            return csd;
        }

        /// <summary>The has changed.</summary>
        /// <param name="context">The context.</param>
        /// <returns>The <see cref="bool"/>.</returns>
        internal bool HasChanged(IWebGreaseContext context)
        {
            return !this.InputSpecHash.Equals(GetInputSpecHash(context, InputSpec), StringComparison.Ordinal);
        }

        #endregion

        #region Methods

        /// <summary>Gets a unique hash for the input spec.</summary>
        /// <param name="context">The context.</param>
        /// <param name="inputSpec">The input spec.</param>
        /// <returns>The unique hash>.</returns>
        private static string GetInputSpecHash(IWebGreaseContext context, InputSpec inputSpec)
        {
            return
                inputSpec.GetFiles(context.Configuration.SourceDirectory)
                         .ToDictionary(f => f.MakeRelativeToDirectory(context.Configuration.SourceDirectory), context.GetFileHash)
                         .ToJson();
        }

        #endregion
    }
}