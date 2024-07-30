// --------------------------------------------------------------------------------------------------------------------
// <copyright file="INamedConfig.cs" company="Microsoft">
//   Copyright Microsoft Corporation, all rights reserved
// </copyright>
namespace WebGrease.Configuration
{
    /// <summary>The NamedConfig interface.</summary>
    public interface INamedConfig
    {
        #region Public Properties

        /// <summary>
        /// Gets the name of this configuration
        /// </summary>
        string Name { get; }

        #endregion
    }
}