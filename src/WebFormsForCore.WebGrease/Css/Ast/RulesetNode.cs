// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RulesetNode.cs" company="Microsoft">
//   Copyright Microsoft Corporation, all rights reserved
// </copyright>
// <summary>
//   ruleset
//   : selectors_group
//   '{' S* declaration? [ ';' S* declaration? ]* '}' S*
//   ;
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace WebGrease.Css.Ast
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;    
    using System.Collections.Specialized;
    using System.Diagnostics.Contracts;
    using System.Linq;
    using Selectors;
    using Visitor;

    /// <summary>ruleset
    /// : selectors_group
    /// '{' S* declaration? [ ';' S* declaration? ]* '}' S*
    /// ;</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Ruleset")]
    public sealed class RulesetNode : StyleSheetRuleNode
    {
        /// <summary>Initializes a new instance of the RulesetNode class</summary>
        /// <param name="selectorsGroupNode">Selectors group node.</param>
        /// <param name="declarations">The list of declarations.</param>
        public RulesetNode(SelectorsGroupNode selectorsGroupNode, ReadOnlyCollection<DeclarationNode> declarations, ReadOnlyCollection<ImportantCommentNode> importantComments)
        {
            Contract.Requires(selectorsGroupNode != null);

            // Member Initialization
            this.SelectorsGroupNode = selectorsGroupNode;
            this.Declarations = declarations ?? new List<DeclarationNode>(0).AsReadOnly();
            this.ImportantComments = importantComments ?? new List<ImportantCommentNode>(0).AsReadOnly();
        }

        /// <summary>
        /// Gets the important comments
        /// </summary>
        /// <value>The ImportantCommentNodes</value>
        public ReadOnlyCollection<ImportantCommentNode> ImportantComments { get; private set; }


        /// <summary>
        /// Gets the selectors group node.
        /// </summary>
        /// <value>The selectors group node.</value>
        public SelectorsGroupNode SelectorsGroupNode { get; private set; }

        /// <summary>
        /// Gets Declarations
        /// </summary>
        /// <value>Declarations dictionary</value>
        public ReadOnlyCollection<DeclarationNode> Declarations { get; private set; }
        /// <summary>
        /// Check membership of each declaration in the dictionary.
        /// </summary>
        /// <param name="declarationDictionary"></param>
        /// <returns></returns>
        public bool HasConflictingDeclaration(OrderedDictionary declarationDictionary)
        {
            foreach (var declaration in Declarations)
            {
                if (declarationDictionary.Contains(declaration.Property))
                {
                    return !((DeclarationNode)declarationDictionary[declaration.Property]).Equals(declaration);
                }                
            }
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rulesetNode"></param>
        /// <returns></returns>
        public bool ShouldMergeWith(RulesetNode rulesetNode)
        {
            int intersection=0;
            foreach (var myDeclaration in this.Declarations)
            {
                foreach (var otherDeclaration in rulesetNode.Declarations)
                {
                    if(myDeclaration.Equals(otherDeclaration))
                    {
                        intersection++;
                        break;
                    }
                }

                if (intersection > 1)
                {
                    break;
                }
            }

            return intersection > 1 ||(intersection==1 &&(Declarations.Count==1 || rulesetNode.Declarations.Count==1));
            //return intersection > 1;
        }

        /// <summary>
        /// Gets merged RuleseteNode from the two RulesetNode
        /// </summary>
        /// <param name="otherRulesetNode"> another ruleset node</param>
        /// <returns> A new merged Ruleset Node.</returns>
        public RulesetNode GetMergedRulesetNode(RulesetNode otherRulesetNode)
        {
            List<SelectorNode> mySelectors = new List<SelectorNode>(this.SelectorsGroupNode.SelectorNodes);
            List<SelectorNode> otherSelectors = new List<SelectorNode>(otherRulesetNode.SelectorsGroupNode.SelectorNodes);
            ReadOnlyCollection<SelectorNode> unionList = mySelectors.Union(otherSelectors).ToList().AsReadOnly();

            List<DeclarationNode> myDeclarations = new List<DeclarationNode>(this.Declarations);
            List<DeclarationNode> otherDeclarations = new List<DeclarationNode>(otherRulesetNode.Declarations);
            List<DeclarationNode> mergedNewDeclarations = new List<DeclarationNode>();

            foreach (var myDeclaration in this.Declarations)
            {
                bool unique = true;
                foreach (var otherDeclaration in otherRulesetNode.Declarations)
                {
                    if (myDeclaration.Equals(otherDeclaration))
                    {
                        unique = false;
                        otherDeclarations.Remove(otherDeclaration);
                        break;
                    }
                } 

                if (!unique)
                {
                    myDeclarations.Remove(myDeclaration);
                    mergedNewDeclarations.Add(myDeclaration);
                }
            }

            this.Declarations = myDeclarations.AsReadOnly();
            otherRulesetNode.Declarations = otherDeclarations.AsReadOnly();
            return new RulesetNode(new SelectorsGroupNode(unionList), mergedNewDeclarations.AsReadOnly(), this.ImportantComments);
        }

        /// <summary>Defines an accept operation</summary>
        /// <param name="nodeVisitor">The visitor to invoke</param>
        /// <returns>The modified AST node if modified otherwise the original node</returns>
        public override AstNode Accept(NodeVisitor nodeVisitor)
        {
            return nodeVisitor.VisitRulesetNode(this);
        } 
    }
}