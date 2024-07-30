// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StyleSheetRuleNode.cs" company="Microsoft">
//   Copyright Microsoft Corporation, all rights reserved
// </copyright>
// <summary>
//   Empty abstract class that represents RuleSet, MediaNode, PageNode or KeyFrames nodes
//   [ [ ruleset | media | page | keyframes ] [ CDO S* | CDC S* ]* ]*
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace WebGrease.Css.Ast
{
    /// <summary>Empty abstract class that represents RuleSet, MediaNode, PageNode or KeyFrames nodes
    /// [ [ ruleset | media | page | keyframes ] [ CDO S* | CDC S* ]* ]*</summary>
    public abstract class StyleSheetRuleNode : AstNode
    {
    }
}