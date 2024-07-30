// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ResourcePivotGroup.cs" company="Microsoft">
//   Copyright Microsoft Corporation, all rights reserved
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace WebGrease.Configuration
{
    using System.Collections.Generic;

    /// <summary>The resource pivot group.</summary>
    public class ResourcePivotGroup
    {
        #region Constructors and Destructors

        /// <summary>Initializes a new instance of the <see cref="ResourcePivotGroup"/> class.</summary>
        /// <param name="key">The key.</param>
        /// <param name="applyMode">The apply mode.</param>
        /// <param name="keys">The keys.</param>
        public ResourcePivotGroup(string key, ResourcePivotApplyMode applyMode, IEnumerable<string> keys)
        {
            this.Key = key;
            this.ApplyMode = applyMode;
            this.Keys = new HashSet<string>(keys);
        }

        #endregion

        #region Public Properties

        /// <summary>Gets the apply mode.</summary>
        public ResourcePivotApplyMode ApplyMode { get; private set; }

        /// <summary>Gets the key.</summary>
        public string Key { get; private set; }

        /// <summary>Gets the keys.</summary>
        public HashSet<string> Keys { get; private set; }

        #endregion
    }
}