// ----------------------------------------------------------------------------------------------------
// <copyright file="WebGreaseMeasureItem.cs" company="Microsoft Corporation">
//   Copyright Microsoft Corporation, all rights reserved.
// </copyright>
// ----------------------------------------------------------------------------------------------------
namespace WebGrease
{
    using System;

    /// <summary>The web grease measure item.</summary>
    public class TimeMeasureItem
    {
        #region Constructors and Destructors

        /// <summary>Initializes a new instance of the <see cref="TimeMeasureItem"/> class.</summary>
        /// <param name="id">The id.</param>
        /// <param name="value">The value.</param>
        public TimeMeasureItem(string id, DateTime value)
        {
            this.Id = id;
            this.Value = value;
        }

        #endregion

        #region Public Properties

        /// <summary>Gets or sets the id.</summary>
        public string Id { get; set; }

        /// <summary>Gets or sets the value.</summary>
        public DateTime Value { get; set; }

        #endregion
    }
}