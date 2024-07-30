// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Background.cs" company="Microsoft">
//   Copyright Microsoft Corporation, all rights reserved
// </copyright>
// <summary>
//   Represents the Css "background" declaration
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace WebGrease.Css.ImageAssemblyAnalysis.PropertyModel
{
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using Ast;
    using Extensions;

    /// <summary>Represents the Css "background" declaration</summary>
    internal sealed class Background
    {
        /// <summary>Initializes a new instance of the Background class
        /// Example:
        /// #selector
        /// {
        ///   background: url(../../i/02/3118D8F3781159C8341246BBF2B4CA.gif) no-repeat -10px -200px;
        /// }</summary>
        /// <param name="declarationAstNode">The background declaration node</param>
        /// <param name="outputUnit">The output unit.</param>
        /// <param name="outputUnitFactor">The output unit factor.</param>
        internal Background(DeclarationNode declarationAstNode, string outputUnit, double outputUnitFactor)
        {
            Contract.Requires(declarationAstNode != null);

            this.DeclarationAstNode = declarationAstNode;
            this.BackgroundImage = new BackgroundImage();
            this.BackgroundPosition = new BackgroundPosition(outputUnit, outputUnitFactor);
            this.BackgroundRepeat = new BackgroundRepeat();

            var expr = declarationAstNode.ExprNode;
            var termNode = expr.TermNode;

            // Parse term
            this.BackgroundImage.ParseTerm(termNode);
            this.BackgroundPosition.ParseTerm(termNode);
            this.BackgroundRepeat.ParseTerm(termNode);

            // Parse term with operator
            expr.TermsWithOperators.ForEach(termWithOperator =>
            {
                this.BackgroundImage.ParseTermWithOperator(termWithOperator);
                this.BackgroundPosition.ParseTermWithOperator(termWithOperator);
                this.BackgroundRepeat.ParseTermWithOperator(termWithOperator);
            });
        }

        /// <summary>
        /// Gets the declaration node
        /// </summary>
        public DeclarationNode DeclarationAstNode { get; private set; }

        /// <summary>
        /// Gets the background image declaration
        /// </summary>
        internal BackgroundImage BackgroundImage { get; private set; }

        /// <summary>
        /// Gets the background position  declaration
        /// </summary>
        internal BackgroundPosition BackgroundPosition { get; private set; }

        /// <summary>
        /// Gets the background repeat  declaration
        /// </summary>
        internal BackgroundRepeat BackgroundRepeat { get; private set; }

        /// <summary>
        /// Gets the url of background image
        /// </summary>
        internal string Url
        {
            get
            {
                return this.BackgroundImage.Url;
            }
        }

        /// <summary>Updates the background node with new url, x, y</summary>
        /// <example>The coordinates need to be expanded for long declaration:
        /// #selector
        /// {
        ///     background: url x y;
        /// }</example>
        /// <param name="updatedUrl">The updated url</param>
        /// <param name="updatedX">The updated x</param>
        /// <param name="updatedY">The updated y</param>
        /// <param name="webGreaseBackgroundDpi">The webgrease background dpi to use</param>
        /// <returns>The new declaration node with updated values</returns>
        internal DeclarationNode UpdateBackgroundNode(string updatedUrl, int? updatedX, int? updatedY, float webGreaseBackgroundDpi)
        {
            var isUrlUpdated = false;
            var isXUpdated = false;
            var isYUpdated = false;
            var indexX = 0;
            var indexY = 0;

            var updatedTermsWithOperators = new List<TermWithOperatorNode>();
            foreach (var termWithOperatorNode in this.DeclarationAstNode.DeclarationEnumerator())
            {
                TermNode updatedTermNode;

                // Try updating url
                if (!isUrlUpdated)
                {
                    isUrlUpdated = this.BackgroundImage.UpdateTermForUrl(termWithOperatorNode.TermNode, out updatedTermNode, updatedUrl);

                    if (isUrlUpdated)
                    {
                        updatedTermsWithOperators.Add(new TermWithOperatorNode(termWithOperatorNode.Operator, updatedTermNode.CopyTerm()));
                        continue;
                    }
                }

                // Try updating X
                if (!isXUpdated)
                {
                    isXUpdated = this.BackgroundPosition.UpdateTermForX(termWithOperatorNode.TermNode, out updatedTermNode, updatedX, webGreaseBackgroundDpi);

                    if (isXUpdated)
                    {
                        if (isYUpdated)
                        {
                            // Insert just before Y (consider a scenario of top center to center 0)
                            updatedTermsWithOperators.Insert(indexX, new TermWithOperatorNode(termWithOperatorNode.Operator, updatedTermNode.CopyTerm()));
                        }
                        else
                        {
                            // Insert at the end
                            updatedTermsWithOperators.Add(new TermWithOperatorNode(termWithOperatorNode.Operator, updatedTermNode.CopyTerm()));
                            indexY = updatedTermsWithOperators.Count;
                        }

                        continue;
                    }
                }

                // Try updating Y
                if (!isYUpdated)
                {
                    isYUpdated = this.BackgroundPosition.UpdateTermForY(termWithOperatorNode.TermNode, out updatedTermNode, updatedY, webGreaseBackgroundDpi);

                    if (isYUpdated)
                    {
                        if (isXUpdated)
                        {
                            // Insert just after X (consider a scenario of top center to center 0)
                            updatedTermsWithOperators.Insert(indexY, new TermWithOperatorNode(termWithOperatorNode.Operator, updatedTermNode.CopyTerm()));
                        }
                        else
                        {
                            // Insert at the end
                            updatedTermsWithOperators.Add(new TermWithOperatorNode(termWithOperatorNode.Operator, updatedTermNode.CopyTerm()));
                            indexX = updatedTermsWithOperators.Count - 1;
                        }

                        continue;
                    }
                }

                // Save the original term with operator (such as no-repeat)
                updatedTermsWithOperators.Add(termWithOperatorNode);
            }

            // Add any missing X or Y
            BackgroundPosition.AddingMissingXAndY(updatedX, updatedY, isXUpdated, isYUpdated, indexX, indexY, updatedTermsWithOperators, webGreaseBackgroundDpi);

            return this.DeclarationAstNode.CreateDeclarationNode(updatedTermsWithOperators);
        }
    }
}