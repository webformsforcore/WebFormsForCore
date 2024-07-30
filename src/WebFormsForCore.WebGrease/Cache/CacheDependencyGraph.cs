// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CacheDependencyGraph.cs" company="Microsoft">
//   Copyright Microsoft Corporation, all rights reserved
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace WebGrease
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Xml.Linq;

    /// <summary>The cache dependency graph.</summary>
    internal class CacheDependencyGraph
    {
        #region Fields

        /// <summary>The links.</summary>
        private readonly List<KeyValuePair<Guid, Guid>> links = new List<KeyValuePair<Guid, Guid>>();

        /// <summary>The nodes.</summary>
        private readonly IDictionary<string, Guid> nodes = new Dictionary<string, Guid>();

        #endregion

        #region Public Methods and Operators

        /// <summary>The add dependency link.</summary>
        /// <param name="label1">The label 1.</param>
        /// <param name="label2">The label 2.</param>
        internal void AddDependencyLink(string label1, string label2)
        {
            var guid1 = this.AddDependencyNode(label1);
            var guid2 = this.AddDependencyNode(label2);
            var link = new KeyValuePair<Guid, Guid>(guid1, guid2);
            if (!this.links.Contains(link))
            {
                this.links.Add(link);
            }
        }

        /// <summary>The save.</summary>
        /// <param name="path">The path.</param>
        internal void Save(string path)
        {
            var xmlns = XNamespace.Get("http://schemas.microsoft.com/vs/2009/dgml");
            var directedGraphDoc = new XDocument(
                new XDeclaration("1.0", "utf-8", "no"), 
                new XElement(
                    xmlns + "DirectedGraph", 
                    new XAttribute("GraphDirection", "TopToBottom"), 
                    new XAttribute("Layout", "Sugiyama"), 
                    new XElement(
                        xmlns + "Nodes", 
                        this.nodes.Select(kvp => new XElement(xmlns + "Node", new XAttribute("Id", kvp.Value), new XAttribute("Label", kvp.Key)))), 
                    new XElement(
                        xmlns + "Links", 
                        this.links.Select(kvp => new XElement(xmlns + "Link", new XAttribute("Source", kvp.Key), new XAttribute("Target", kvp.Value)))), 
                    new XElement(
                        xmlns + "Properties", 
                        new XElement(
                            xmlns + "Property", 
                            new XAttribute("Id", "GraphDirection"), 
                            new XAttribute("DataType", "Microsoft.VisualStudio.Diagrams.Layout.LayoutOrientation")), 
                        new XElement(xmlns + "Property", new XAttribute("Id", "Layout"), new XAttribute("DataType", "System.String")), 
                        new XElement(xmlns + "Property", new XAttribute("Id", "Bounds"), new XAttribute("DataType", "System.String")), 
                        new XElement(
                            xmlns + "Property", 
                            new XAttribute("Id", "Label"), 
                            new XAttribute("Label", "Label"), 
                            new XAttribute("Description", "Displayable label of an Annotatable object"), 
                            new XAttribute("DataType", "System.String")))));

            directedGraphDoc.Save(path);
        }

        /// <summary>The add dependency node.</summary>
        /// <param name="label">The label.</param>
        /// <returns>The <see cref="Guid"/>.</returns>
        [SuppressMessage("Microsoft.Globalization", "CA1308:NormalizeStringsToUppercase", Justification = "Needs lowercase")]
        private Guid AddDependencyNode(string label)
        {
            var key = label.ToLowerInvariant();
            if (!this.nodes.ContainsKey(key))
            {
                this.nodes.Add(key, Guid.NewGuid());
            }

            return this.nodes[key];
        }

        #endregion
    }
}