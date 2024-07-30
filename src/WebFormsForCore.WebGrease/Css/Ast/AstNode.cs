// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AstNode.cs" company="Microsoft">
//   Copyright Microsoft Corporation, all rights reserved
// </copyright>
// <summary>
//   This is the base class of the Abstract Syntax Tree (AST)
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace WebGrease.Css.Ast
{
    using Visitor;

    /// <summary>This is the base class of the Abstract Syntax Tree (AST)</summary>
    public abstract class AstNode
    {
        /// <summary>Defines an accept operation</summary>
        /// <param name="nodeVisitor">The visitor to invoke</param>
        /// <returns>The modified AST node if modified otherwise the original node</returns>
        public virtual AstNode Accept(NodeVisitor nodeVisitor)
        {
            return this;
        }
    }
}
