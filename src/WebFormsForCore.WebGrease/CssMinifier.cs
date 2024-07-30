// -----------------------------------------------------------------------
// <copyright file="CssMinifier.cs" company="Microsoft">
// Copyright Microsoft Corporation, all rights reserved
// </copyright>
// -----------------------------------------------------------------------

namespace WebGrease
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Css;
    using WebGrease.Activities;

    /// <summary>
    /// Minifier for css.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Minifier", Justification = "Spelt as desired")]
    public class CssMinifier
    {
        /// <summary>Initializes a new instance of the <see cref="CssMinifier"/> class with default settings.</summary>
        /// <param name="context">The context.</param>
        public CssMinifier(IWebGreaseContext context)
        {
            this.CssActivity = new MinifyCssActivity(context)
                                   {
                                       ShouldMinify = true,
                                       ShouldOptimize = true,
                                       ShouldValidateForLowerCase = false,
                                       ShouldExcludeProperties = false,
                                       ShouldAssembleBackgroundImages = false
                                   };
            this.ShouldMinify = true;
            this.Errors = new List<string>();
        }

        /// <summary>
        /// List of errors causing the minification to fail, if any.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1002:DoNotExposeGenericLists", Justification = "This is not performance critical, and needed for the AddRange method.")]
        public List<string> Errors { get; private set; }

        /// <summary>
        /// Gets or sets a value indicating whether the css content should be minified.
        /// True by default, but set to false to create a "pretty print" version instead.
        /// </summary>
        public bool ShouldMinify { get; set; }

        /// <summary>
        /// Gets or sets the activity to use.
        /// </summary>
        private MinifyCssActivity CssActivity { get; set; }

        /// <summary>
        /// Minifies the given css content.
        /// </summary>
        /// <param name="cssContent">Css to be minified.</param>
        /// <returns>The minifed css. If there are errors, this will be empty.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Catching the exception and storing it for callers to use.")]
        public string Minify(string cssContent)
        {
            this.CssActivity.ShouldMinify = this.ShouldMinify;
            MinifyCssResult cssMinifyResult = null;
            Exception minifyException = null;
            try
            {
                cssMinifyResult = this.CssActivity.Process(ContentItem.FromContent(cssContent));
            }
            catch (Exception ex)
            {
                minifyException = ex;
            }

            // we only expect this if the CssParser bombed on the css, and it should always throw an AggregateEx with it's errors inside
            if (minifyException != null)
            {
                // try to take the friendliest bits of the errors
                var aggEx = minifyException as AggregateException;
                if (aggEx != null)
                {
                    this.Errors.AddRange(ErrorHelper.DedupeCSSErrors(aggEx));
                }
            }

            return cssMinifyResult != null && cssMinifyResult.Css != null && cssMinifyResult.Css.Any()
                        ? cssMinifyResult.Css.FirstOrDefault().Content
                        : null;
        }
    }
}
