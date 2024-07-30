// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ResourcePivotGroupCollection.cs" company="Microsoft">
//   Copyright Microsoft Corporation, all rights reserved
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace WebGrease.Configuration
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>The resource pivot group collection.</summary>
    public class ResourcePivotGroupCollection : IEnumerable<ResourcePivotGroup>
    {
        /// <summary>The resource pivots.</summary>
        private readonly IDictionary<string, ResourcePivotGroup> resourcePivots = new Dictionary<string, ResourcePivotGroup>();

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.IEnumerator"/> object that can be used to iterate through the collection.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        /// <summary>The this.</summary>
        /// <param name="groupKey">The group key.</param>
        /// <returns>The <see cref="ResourcePivotGroup"/>.</returns>
        public ResourcePivotGroup this[string groupKey]
        {
            get
            {
                ResourcePivotGroup resourcePivotGroup;
                if (this.resourcePivots.TryGetValue(groupKey, out resourcePivotGroup))
                {
                    return resourcePivotGroup;
                }

                return null;
            }
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Collections.Generic.IEnumerator`1"/> that can be used to iterate through the collection.
        /// </returns>
        /// <filterpriority>1</filterpriority>
        public IEnumerator<ResourcePivotGroup> GetEnumerator()
        {
            return this.resourcePivots.Values.GetEnumerator();
        }

        /// <summary>Clears all the items.</summary>
        /// <param name="groupKey">The group key.</param>
        internal void Clear(string groupKey)
        {
            var resourcePivotGroup = this[groupKey];
            if (resourcePivotGroup != null)
            {
                resourcePivotGroup.Keys.Clear();
            }
        }

        /// <summary>Sets the groupkey applymode and keys.</summary>
        /// <param name="groupKey">The group key.</param>
        /// <param name="applyMode">The apply mode.</param>
        /// <param name="keys">The keys.</param>
        internal void Set(string groupKey, ResourcePivotApplyMode? applyMode, IEnumerable<string> keys)
        {
            var resourcePivotGroup = this[groupKey];
            if (resourcePivotGroup != null)
            {
                resourcePivotGroup = new ResourcePivotGroup(groupKey, applyMode ?? resourcePivotGroup.ApplyMode, resourcePivotGroup.Keys.Concat(keys));
            }
            else
            {
                resourcePivotGroup = new ResourcePivotGroup(groupKey, applyMode ?? ResourcePivotApplyMode.ApplyAsStringReplace, keys);
            }

            this.resourcePivots[groupKey] = resourcePivotGroup;
        }
    }
}
