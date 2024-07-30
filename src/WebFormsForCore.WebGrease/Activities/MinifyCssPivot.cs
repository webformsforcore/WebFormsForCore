// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MinifyCssPivot.cs" company="Microsoft">
//   Copyright Microsoft Corporation, all rights reserved
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace WebGrease.Activities
{
    using System.Collections.Generic;
    using System.Linq;

    using WebGrease.Configuration;

    /// <summary>The minify css pivot.</summary>
    internal class MinifyCssPivot
    {
        /// <summary>The string value.</summary>
        private readonly string stringValue;

        /// <summary>Initializes a new instance of the <see cref="MinifyCssPivot"/> class.</summary>
        /// <param name="mergedResource">The merged resource.</param>
        /// <param name="newContentResourcePivotKeys">The new content resource pivot keys.</param>
        /// <param name="dpi">The dpi.</param>
        public MinifyCssPivot(IEnumerable<IDictionary<string, string>> mergedResource, ResourcePivotKey[] newContentResourcePivotKeys, float dpi)
        {
            this.MergedResource = mergedResource;
            this.NewContentResourcePivotKeys = newContentResourcePivotKeys;
            this.Dpi = dpi;
            this.stringValue = string.Join("-", this.NewContentResourcePivotKeys.Select(p => p.Key));
        }

        /// <summary>Gets the merged resource.</summary>
        public IEnumerable<IDictionary<string, string>> MergedResource { get; private set; }

        /// <summary>Gets the new content resource pivot keys.</summary>
        public ResourcePivotKey[] NewContentResourcePivotKeys { get; private set; }

        /// <summary>Gets the dpi.</summary>
        public float Dpi { get; private set; }

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>
        /// A string that represents the current object.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public override string ToString()
        {
            return this.stringValue;
        }

        /// <summary>
        /// Serves as a hash function for a particular type. 
        /// </summary>
        /// <returns>
        /// A hash code for the current <see cref="T:System.Object"/>.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public override int GetHashCode()
        {
            return this.stringValue.GetHashCode();
        }
    }
}