// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TermNode.cs" company="Microsoft">
//   Copyright Microsoft Corporation, all rights reserved
// </copyright>
// <summary>
//   term
//   unary_operator?
//   [ NUMBER S* | PERCENTAGE S* | LENGTH S* | EMS S* | EXS S* | ANGLE S* | TIME S* | FREQ S* ]
//   | STRING S* | IDENT S* | URI S* | hexcolor | function
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace WebGrease.Css.Ast
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using Visitor;


    /// <summary>term
    /// unary_operator?
    /// [ NUMBER S* | PERCENTAGE S* | LENGTH S* | EMS S* | EXS S* | ANGLE S* | TIME S* | FREQ S* ]
    /// | STRING S* | IDENT S* | URI S* | hexcolor | function</summary>
    public sealed class TermNode : AstNode
    {
        /// <summary>Initializes a new instance of the TermNode class</summary>
        /// <param name="unaryOperator">Unary Operator</param>
        /// <param name="numberBasedValue">Number based value</param>
        /// <param name="stringBasedValue">String based value</param>
        /// <param name="hexColor">Hexadecimal color code</param>
        /// <param name="functionNode">Function object</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "string", Justification = "Css value description.")]
        public TermNode(string unaryOperator, string numberBasedValue, string stringBasedValue,
            string hexColor, FunctionNode functionNode, ReadOnlyCollection<ImportantCommentNode> importantComments, string replacementTokenBasedValue = null)
        {
            // Validity Checks
            // Besides the optional unary_operator, only one value can exist for the remaing arguments the rest need to be null
            var isArgumentPopulated = false;
            var hasError = false;

            // Check for: [ NUMBER S* | PERCENTAGE S* | LENGTH S* | EMS S* | EXS S* | ANGLE S* | TIME S* | FREQ S* ]
            if (!string.IsNullOrWhiteSpace(numberBasedValue))
            {
                isArgumentPopulated = true;
            }

            // check for: STRING | IDENT | URI
            if (!string.IsNullOrWhiteSpace(stringBasedValue))
            {
                if (isArgumentPopulated)
                {
                    hasError = true;
                }
                else
                {
                    isArgumentPopulated = true;
                }
            }

            // check for: hexcolor
            if (!string.IsNullOrWhiteSpace(hexColor))
            {
                if (isArgumentPopulated)
                {
                    hasError = true;
                }
                else
                {
                    isArgumentPopulated = true;
                }
            }

            // functionNode
            if (functionNode != null)
            {
                if (isArgumentPopulated)
                {
                    hasError = true;
                }
            }

            if (hasError)
            {
                throw new AstException(CssStrings.ExpectedSingleValue);
            }


            // Member Initialization
            this.UnaryOperator = unaryOperator;
            this.NumberBasedValue = numberBasedValue;
            this.StringBasedValue = stringBasedValue;
            this.Hexcolor = hexColor;
            this.FunctionNode = functionNode;
            this.ImportantComments = importantComments ?? (new List<ImportantCommentNode>()).AsReadOnly();
            this.IsBinary = false;            
            this.ReplacementTokenBasedValue = replacementTokenBasedValue;
        }

        /// <summary>Gets or sets the replacement token based value.</summary>
        public string ReplacementTokenBasedValue { get; set; }

        /// <summary>
        /// Gets The Comments
        /// </summary>
        /// <value> The list of Important Comment Nodes</value>
        public ReadOnlyCollection<ImportantCommentNode> ImportantComments { get; private set; }

        /// <summary>
        /// Gets the Binary value
        /// Determines Whether this term is inside Binary Expression.
        /// </summary>
        public bool IsBinary { get; set; }

        /// <summary>
        /// Gets Unary Operatior
        /// </summary>
        /// <value>Unary Operator</value>
        public string UnaryOperator { get; private set; }            

        /// <summary>
        /// Gets Number base value
        /// </summary>
        /// <value>Number base value</value>
        public string NumberBasedValue { get; private set; }

        /// <summary>
        /// Gets String base value
        /// </summary>
        /// <value>String base value</value>
        public string StringBasedValue { get; private set; }

        /// <summary>
        /// Gets Hexa color 
        /// </summary>
        /// <value>Hexa Colot value</value>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Hexcolor")]
        public string Hexcolor { get; private set; }

        /// <summary>
        /// Gets Funtion
        /// </summary>
        /// <value>FunctionNode Property</value>
        public FunctionNode FunctionNode { get; private set; }

        /// <summary>
        /// Determines if the node is equal to another node
        /// </summary>
        /// <param name="termNode"> another term node</param>
        /// <returns> Equal or not.</returns>
        public bool Equals(TermNode termNode)
        {
            bool equals = termNode.IsBinary == this.IsBinary
                && termNode.UnaryOperator == this.UnaryOperator
                && termNode.NumberBasedValue == this.NumberBasedValue
                && termNode.StringBasedValue == this.StringBasedValue
                && termNode.Hexcolor == this.Hexcolor;
            if (this.FunctionNode != null && termNode.FunctionNode != null)
            {
                return equals && termNode.FunctionNode.Equals(this.FunctionNode);
            }
            else if (this.FunctionNode == null && termNode.FunctionNode == null)
            {
                return equals;
            }
            else
            {
                return false;
            }
        }

        /// <summary>Defines an accept operation</summary>
        /// <param name="nodeVisitor">The visitor to invoke</param>
        /// <returns>The modified AST node if modified otherwise the original node</returns>
        public override AstNode Accept(NodeVisitor nodeVisitor)
        {
            return nodeVisitor.VisitTermNode(this);
        }
    }
}
