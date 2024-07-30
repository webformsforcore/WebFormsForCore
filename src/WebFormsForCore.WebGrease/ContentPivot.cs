// ----------------------------------------------------------------------------------------------------
// <copyright file="ContentPivot.cs" company="Microsoft Corporation">
//   Copyright Microsoft Corporation, all rights reserved.
// </copyright>
// ----------------------------------------------------------------------------------------------------
namespace WebGrease
{
    using System.Collections.Generic;
    using System.Linq;

    using WebGrease.Configuration;
    using WebGrease.Extensions;

    /// <summary>
    /// The content pivot class is used for pivots on locales/theme to use in with resources for each content item.
    /// In the future this class will also support other pivots needed than locale/theme.
    /// </summary>
    public class ContentPivot
    {
        /// <summary>Initializes a new instance of the <see cref="ContentPivot"/> class.</summary>
        /// <param name="pivotKeys">The pivot Keys.</param>
        public ContentPivot(params ResourcePivotKey[] pivotKeys)
        {
            this.PivotKeys = pivotKeys;
        }

        #region Public Properties

        /// <summary>Gets the pivots.</summary>
        public IEnumerable<ResourcePivotKey> PivotKeys { get; private set; }

        /// <summary>The this.</summary>
        /// <param name="groupKey">The pivot key.</param>
        /// <returns>The <see cref="string"/>.</returns>
        public string this[string groupKey]
        {
            get
            {
                return this.PivotKeys
                           .Where(pk => pk.GroupKey.Equals(groupKey))
                           .Select(pk => pk.Key)
                           .FirstOrDefault();
            }
        }

        #endregion

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>
        /// A string that represents the current object.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public override string ToString()
        {
            return "{0}".InvariantFormat(string.Join("-", this.PivotKeys.Select(p => p.Key).Where(i => !i.IsNullOrWhitespace())));
        }
    }
}