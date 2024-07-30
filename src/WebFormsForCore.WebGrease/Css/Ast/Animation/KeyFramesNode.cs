// --------------------------------------------------------------------------------------------------------------------
// <copyright file="KeyFramesNode.cs" company="Microsoft">
//   Copyright Microsoft Corporation, all rights reserved
// </copyright>
// <summary>
//   The key frames node.
//   keyframes-rule: '@keyframes' IDENT '{' keyframes-blocks '}';
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace WebGrease.Css.Ast.Animation
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Diagnostics.Contracts;
    using Visitor;

    /// <summary>The key frames node.
    /// keyframes-rule: '@keyframes' IDENT '{' keyframes-blocks '}';</summary>
    public sealed class KeyFramesNode : StyleSheetRuleNode
    {
        /// <summary>Initializes a new instance of the <see cref="KeyFramesNode"/> class.</summary>
        /// <param name="keyFramesSymbol">The key frames symbol.</param>
        /// <param name="identValue">The ident value.</param>
        /// <param name="stringValue">The str value.</param>
        /// <param name="keyFramesBlockNodes">The key frames block nodes.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "string", Justification = "Css keyword")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "ident", Justification = "Css keyword")]
        public KeyFramesNode(string keyFramesSymbol, string identValue, string stringValue, ReadOnlyCollection<KeyFramesBlockNode> keyFramesBlockNodes)
        {
            Contract.Requires(!string.IsNullOrWhiteSpace(keyFramesSymbol));
            Contract.Requires(!string.IsNullOrWhiteSpace(keyFramesSymbol) || !string.IsNullOrWhiteSpace(stringValue));

            this.KeyFramesSymbol = keyFramesSymbol;
            this.IdentValue = identValue;
            this.StringValue = stringValue;
            this.KeyFramesBlockNodes = keyFramesBlockNodes ?? new List<KeyFramesBlockNode>(0).AsReadOnly();
        }

        /// <summary>Gets the key frames symbol.</summary>
        public string KeyFramesSymbol { get; private set; }

        /// <summary>Gets the ident value.</summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Ident")]
        public string IdentValue { get; private set; }

        /// <summary>Gets the str value.</summary>
        public string StringValue { get; private set; }

        /// <summary>Gets the key frames block nodes.</summary>
        public ReadOnlyCollection<KeyFramesBlockNode> KeyFramesBlockNodes { get; private set; }

        /// <summary>Defines an accept operation</summary>
        /// <param name="nodeVisitor">The visitor to invoke</param>
        /// <returns>The modified AST node if modified otherwise the original node</returns>
        public override AstNode Accept(NodeVisitor nodeVisitor)
        {
            return nodeVisitor.VisitKeyFramesNode(this);
        }
    }
}
