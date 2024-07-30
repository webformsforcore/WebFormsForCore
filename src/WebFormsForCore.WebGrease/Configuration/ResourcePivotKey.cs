// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ResourcePivotKey.cs" company="Microsoft">
//   Copyright Microsoft Corporation, all rights reserved
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace WebGrease.Configuration
{
    using WebGrease.Extensions;

    /// <summary>The resource pivot key.</summary>
    public class ResourcePivotKey
    {
        #region Constructors and Destructors

        /// <summary>Initializes a new instance of the <see cref="ResourcePivotKey"/> class.</summary>
        /// <param name="groupKey">The group key.</param>
        /// <param name="key">The key.</param>
        public ResourcePivotKey(string groupKey, string key)
        {
            this.GroupKey = groupKey;
            this.Key = key;
        }

        #endregion

        #region Public Properties

        /// <summary>Gets the group key.</summary>
        public string GroupKey { get; private set; }

        /// <summary>Gets the key.</summary>
        public string Key { get; private set; }

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>
        /// A string that represents the current object.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public override string ToString()
        {
            return "[{0}:{1}]".InvariantFormat(this.GroupKey, this.Key);
        }

        #endregion

        #region Internal Methods

        /// <summary>
        /// To string with specific format for concating the GroupKey and Key.
        /// </summary>
        /// <returns>The formated string for a file name, e.g. locale.generic-generic for ToString("{0}.{1}")</returns>
        internal string ToString(string format)
        {
            return format.InvariantFormat(this.GroupKey, this.Key);
        }

        #endregion
    }
}