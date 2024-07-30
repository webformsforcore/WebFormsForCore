// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ImportNode.cs" company="Microsoft">
//   Copyright Microsoft Corporation, all rights reserved
// </copyright>
// <summary>
//   import
//   : IMPORT_SYM S*
//   [STRING|URI] S* media_query_list? ';' S*
//   ;
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace WebGrease.Css.Ast
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Diagnostics.Contracts;
    using MediaQuery;
    using Visitor;

    /// <summary>import
    ///  : IMPORT_SYM S*
    ///    [STRING|URI] S* media_query_list? ';' S*
    /// ;</summary>
    public sealed class ImportNode : AstNode
    {
        /// <summary>Initializes a new instance of the ImportNode class</summary>
        /// <param name="allowedImportDataType">ImportNode Data type</param>
        /// <param name="importDataValue">ImportNode Data value</param>
        /// <param name="mediaQueries">ImportNode mediums</param>
        public ImportNode(AllowedImportData allowedImportDataType, string importDataValue, ReadOnlyCollection<MediaQueryNode> mediaQueries)
        {
            Contract.Requires(allowedImportDataType != AllowedImportData.None);
            Contract.Requires(!string.IsNullOrWhiteSpace(importDataValue));
            
            this.AllowedImportDataType = allowedImportDataType;
            this.ImportDataValue = importDataValue;
            this.MediaQueries = mediaQueries ?? new List<MediaQueryNode>(0).AsReadOnly();
        }

        /// <summary>
        /// Gets ImportNode Datatype
        /// </summary>
        /// <value>ImportNode Datatype</value>
        public AllowedImportData AllowedImportDataType { get; private set; }

        /// <summary>
        /// Gets ImportNode Data value
        /// </summary>
        /// <value>ImportNode Data value</value>
        public string ImportDataValue { get; private set; }

        /// <summary>
        /// Gets Imported MediaQueries
        /// </summary>
        /// <value>Imported MediaQueries</value>
        public ReadOnlyCollection<MediaQueryNode> MediaQueries { get; private set; }

        /// <summary>Defines an accept operation</summary>
        /// <param name="nodeVisitor">The visitor to invoke</param>
        /// <returns>The modified AST node if modified otherwise the original node</returns>
        public override AstNode Accept(NodeVisitor nodeVisitor)
        {
            return nodeVisitor.VisitImportNode(this);
        }
    }
}
