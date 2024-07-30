// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CommonTreeExtensions.cs" company="Microsoft">
//   Copyright Microsoft Corporation, all rights reserved
// </copyright>
// <summary>
//   CommonTreeExtensions Class - Provides the extension on CommonTree types
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace WebGrease.Css.Extensions
{
    using System.Collections.Generic;
    using System.Linq;
    using Antlr.Runtime.Tree;

    /// <summary>CommonTreeExtensions Class - Provides the extension on CommonTree types</summary>
    public static class CommonTreeExtensions
    {
        /// <summary>Gets the children of common tree.</summary>
        /// <param name="commonTree">The common tree.</param>
        /// <param name="childFilterText">The immediate child filter text.</param>
        /// <returns>The enumerable of common tree.</returns>
        public static IEnumerable<CommonTree> Children(this CommonTree commonTree, string childFilterText = null)
        {
            if (commonTree == null || commonTree.Children == null)
            {
                yield break;
            }

            if (!string.IsNullOrWhiteSpace(childFilterText))
            {
                foreach (var child in commonTree.Children.OfType<CommonTree>().Where(_ => _.Text == childFilterText))
                {
                    yield return child;
                }
            }
            else
            {
                foreach (var child in commonTree.Children.OfType<CommonTree>())
                {
                    yield return child;
                }
            }
        }

        /// <summary>Gets the grand child enumerable with immediate child filter text.</summary>
        /// <param name="commonTree">The common tree.</param>
        /// <param name="childFilterText">The immediate child filter text.</param>
        /// <returns>The list of grand children.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", MessageId = "GrandChildren", Justification = "This is by design.")]
        public static IEnumerable<CommonTree> GrandChildren(this CommonTree commonTree, string childFilterText)
        {
            if (commonTree == null || commonTree.Children == null)
            {
                yield break;
            }

            foreach (var granchChild in commonTree.Children(childFilterText).SelectMany(_ => _.Children()))
            {
                yield return granchChild;
            }
        }

        /// <summary>Gets the text represented by common tree.</summary>
        /// <param name="commonTree">The common tree.</param>
        /// <param name="defaultText">The default text.</param>
        /// <returns>The text represented by common tree.</returns>
        public static string TextOrDefault(this CommonTree commonTree, string defaultText = null)
        {
            return commonTree != null ? commonTree.ToString() : defaultText;
        }

        /// <summary>Gets the text of first child or default value.</summary>
        /// <param name="commonTree">The common tree.</param>
        /// <returns>The first child text.</returns>
        public static string FirstChildText(this CommonTree commonTree)
        {
            return FirstChildTextOrDefault(commonTree);
        }

        /// <summary>Gets the text of first child or default value.</summary>
        /// <param name="commonTree">The common tree.</param>
        /// <param name="defaultText">The default text.</param>
        /// <returns>The first child text.</returns>
        public static string FirstChildTextOrDefault(this CommonTree commonTree, string defaultText = null)
        {
            if (commonTree != null)
            {
                var firstChild = commonTree.Children().FirstOrDefault();
                if (firstChild != null)
                {
                    return firstChild.TextOrDefault(defaultText);
                }
            }

            return defaultText;
        }

        /// <summary>Gets the text of first child or default value.</summary>
        /// <param name="commonTree">The common tree.</param>
        /// <returns>The first child text.</returns>
        public static string FirstChildText(this IEnumerable<CommonTree> commonTree)
        {
            return commonTree.FirstChildTextOrDefault();
        }

        /// <summary>Gets the text of first child or default value.</summary>
        /// <param name="commonTree">The common tree.</param>
        /// <param name="defaultText">The default text.</param>
        /// <returns>The first child text.</returns>
        public static string FirstChildTextOrDefault(this IEnumerable<CommonTree> commonTree, string defaultText = null)
        {
            if (commonTree != null)
            {
                var first = commonTree.FirstOrDefault();
                if (first != null)
                {
                    var firstChild = first.Children().FirstOrDefault();
                    if (firstChild != null)
                    {
                        return firstChild.TextOrDefault(defaultText);
                    }
                }
            }

            return defaultText;
        }
    }
}
