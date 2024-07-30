// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExcludePropertyVisitor.cs" company="Microsoft">
//   Copyright Microsoft Corporation, all rights reserved
// </copyright>
// <summary>
//   Provides the implementation of CSS property exclusion visitor. It
//   visits all AST nodes for CSS files and excludes properties that
//   have a "Excluded" value in them.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace WebGrease.Css.Visitor
{
    using System;
    using Ast;
    using Extensions;

    /// <summary>Provides the implementation of CSS property exclusion visitor. It
    /// visits all AST nodes for CSS files and excludes properties that
    /// have a "Excluded" value in them.</summary>
    public sealed class ExcludePropertyVisitor : NodeTransformVisitor
    {
        /// <summary>
        /// The substring that indicates the exclusion of property form the final CSS.
        /// </summary>
        private const string ExcludedSubstring = "Exclude";

        /// <summary>Updates declaration based on property keys/values. If property has a key or a value that contains
        /// "Excluded", then such a property will be excluded from the updated declaration.</summary>
        /// <example>The "background-image" will be excluded from the following CSS selector:
        /// #selector
        /// {
        ///   background-position: -10px  -200px;
        ///   background-image: Excluded;
        ///   background-image: url(../../i/02/3118D8F3781159C8341246BBF2B4CA.gif);
        /// }</example>
        /// <example>The "Excluded-right" will be excluded from the following CSS selector:
        /// #selector
        /// {
        ///   Excluded-right: -10px;
        ///   background-image: url(../../i/02/3118D8F3781159C8341246BBF2B4CA.gif);
        /// }</example>
        /// <param name="declarationNode">The declaration node</param>
        /// <returns>The updated declaration node</returns>
        public override AstNode VisitDeclarationNode(DeclarationNode declarationNode)
        {
            if (declarationNode == null)
            {
                throw new ArgumentNullException("declarationNode");
            }

            return declarationNode.MinifyPrint().Contains(ExcludedSubstring) ? null : declarationNode;
        }
    }
}