// ----------------------------------------------------------------------------------------------------
// <copyright file="TimeMeasureResult.cs" company="Microsoft Corporation">
//   Copyright Microsoft Corporation, all rights reserved.
// </copyright>
// ----------------------------------------------------------------------------------------------------
namespace WebGrease
{
    using System.Collections.Generic;

    /// <summary>The web grease measure result.</summary>
    public class TimeMeasureResult
    {
        #region Public Properties

        /// <summary>Gets or sets the count.</summary>
        public int Count { get; set; }

        /// <summary>Gets or sets the duration.</summary>
        public double Duration { get; set; }

        /// <summary>Gets or sets the id.</summary>
        public IEnumerable<string> IdParts { get; set; }

        /// <summary>Gets the name.</summary>
        public string Name
        {
            get
            {
                return WebGreaseContext.ToStringId(this.IdParts);
            }
        }

        #endregion
    }
}