// --------------------------------------------------------------------------------------------------------------------
// <copyright file="KeyFramesBlockNode.cs" company="Microsoft">
//   Copyright Microsoft Corporation, all rights reserved
// </copyright>
// <summary>
//   The key frames block node.
//   keyframes-blocks: [ keyframe-selectors block ]* ;
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace WebGrease.Css.Ast.Animation
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Diagnostics.Contracts;
    using Visitor;

    /// <summary>The key frames block node.
    /// keyframes-blocks: [ keyframe-selectors block ]* ;</summary>
    public sealed class KeyFramesBlockNode : AstNode
    {
        /// <summary>Initializes a new instance of the <see cref="KeyFramesBlockNode"/> class.</summary>
        /// <param name="keyFramesSelectors">The key frames selectors.</param>
        /// <param name="declarationNodes">The declaration nodes.</param>
        public KeyFramesBlockNode(ReadOnlyCollection<string> keyFramesSelectors, ReadOnlyCollection<DeclarationNode> declarationNodes)
        {
            Contract.Requires(keyFramesSelectors != null && keyFramesSelectors.Count > 0);

            this.KeyFramesSelectors = keyFramesSelectors;
            this.DeclarationNodes = declarationNodes ?? new List<DeclarationNode>(0).AsReadOnly();
        }

        /// <summary>Gets the key frames selectors.</summary>
        public ReadOnlyCollection<string> KeyFramesSelectors { get; private set; }

        /// <summary>Gets the declaration nodes.</summary>
        public ReadOnlyCollection<DeclarationNode> DeclarationNodes { get; private set; }

        /// <summary>Defines an accept operation</summary>
        /// <param name="nodeVisitor">The visitor to invoke</param>
        /// <returns>The modified AST node if modified otherwise the original node</returns>
        public override AstNode Accept(NodeVisitor nodeVisitor)
        {
            return nodeVisitor.VisitKeyFramesBlockNode(this);
        }
    }
}