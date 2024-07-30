// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AttribOperatorAndValueNode.cs" company="Microsoft">
//   Copyright Microsoft Corporation, all rights reserved
// </copyright>
// <summary>
//   [[ PREFIXMATCH |
//   SUFFIXMATCH |
//   SUBSTRINGMATCH |
//   '=' |
//   INCLUDES |
//   DASHMATCH ] S* [ IDENT | STRING ] S*
//   ]?
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace WebGrease.Css.Ast.Selectors
{
    using Visitor;

    /// <summary>[
    ///   [
    ///     PREFIXMATCH |
    ///     SUFFIXMATCH |
    ///     SUBSTRINGMATCH |
    ///     '=' |
    ///     INCLUDES |
    ///     DASHMATCH 
    ///   ] S* 
    ///   [
    ///     IDENT | STRING
    ///   ] S*
    /// ]?</summary>
    public sealed class AttribOperatorAndValueNode : AstNode
    {
        /// <summary>Initializes a new instance of the AttribOperatorAndValueNode class</summary>
        /// <param name="operatorKind">Operator Kind</param>
        /// <param name="identityOrString">Identity Or String</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "string", Justification = "Operator is a css description not a csharp one.")]
        public AttribOperatorAndValueNode(AttribOperatorKind operatorKind, string identityOrString)
        {
            // Error checking
            if (string.IsNullOrWhiteSpace(identityOrString))
            {
                if (operatorKind != AttribOperatorKind.None)
                {
                    throw new AstException(CssStrings.ExpectedIdentifierOrString);
                }
            }

            // Member Initilization
            this.AttribOperatorKind = operatorKind;
            this.IdentOrString = identityOrString;
        }

        /// <summary>
        /// Gets Attribute Operator type
        /// </summary>
        /// <value>Attribute Operator type</value>
        public AttribOperatorKind AttribOperatorKind { get; private set; }

        /// <summary>
        /// Gets Identity Or String
        /// </summary>
        /// <value>Identity Or String</value>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Ident")]
        public string IdentOrString { get; private set; }

        /// <summary>Defines an accept operation</summary>
        /// <param name="nodeVisitor">The visitor to invoke</param>
        /// <returns>The modified AST node if modified otherwise the original node</returns>
        public override AstNode Accept(NodeVisitor nodeVisitor)
        {
            return nodeVisitor.VisitAttribOperatorAndValueNode(this);
        }
    }
}
