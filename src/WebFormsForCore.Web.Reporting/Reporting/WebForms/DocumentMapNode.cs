// Decompiled with JetBrains decompiler
// Type: Microsoft.Reporting.WebForms.DocumentMapNode
// Assembly: Microsoft.ReportViewer.WebForms, Version=12.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91
// MVID: F82ADCE6-59A1-4E44-AF2B-7E8AD7E2F93B
// Assembly location: C:\Users\simon\Downloads\Microsoft.ReportViewer.WebForms.dll

using Microsoft.ReportingServices.ReportProcessing;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

#nullable disable
namespace Microsoft.Reporting.WebForms
{
  public sealed class DocumentMapNode
  {
    private string m_label;
    private string m_id;
    private IList<DocumentMapNode> m_children;

    internal DocumentMapNode(string label, string id, DocumentMapNode[] children)
    {
      this.m_label = label;
      this.m_id = id;
      if (children != null)
        this.m_children = (IList<DocumentMapNode>) new ReadOnlyCollection<DocumentMapNode>((IList<DocumentMapNode>) children);
      else
        this.m_children = (IList<DocumentMapNode>) new ReadOnlyCollection<DocumentMapNode>((IList<DocumentMapNode>) new DocumentMapNode[0]);
    }

    internal static DocumentMapNode CreateTree(IDocumentMap docMap, string rootName)
    {
      DocumentMapNode node = DocumentMapNode.CreateNode(docMap);
      if (node != null)
        node.m_label = rootName;
      return node;
    }

    internal static DocumentMapNode CreateTree(Microsoft.Reporting.WebForms.Internal.Soap.ReportingServices2005.Execution.DocumentMapNode serverNode, string rootName)
    {
      DocumentMapNode node = DocumentMapNode.CreateNode(serverNode);
      if (node != null)
        node.m_label = rootName;
      return node;
    }

    internal static DocumentMapNode CreateNode(IDocumentMap docMap)
    {
      if (docMap == null)
        return (DocumentMapNode) null;
      Stack<DocumentMapNode.NodeStackEntry> nodeStack = new Stack<DocumentMapNode.NodeStackEntry>();
      List<DocumentMapNode> workspace = new List<DocumentMapNode>();
      while (((IEnumerator) docMap).MoveNext())
      {
        OnDemandDocumentMapNode current = ((IEnumerator<OnDemandDocumentMapNode>) docMap).Current;
        DocumentMapNode.NodeStackEntry nodeStackEntry;
        nodeStackEntry.Node = DocumentMapNode.FromOnDemandNode(current);
        nodeStackEntry.Level = current.Level;
        while (nodeStack.Count > 0 && current.Level < nodeStack.Peek().Level)
          DocumentMapNode.CollapseTopLevel(nodeStack, workspace);
        nodeStack.Push(nodeStackEntry);
      }
      while (nodeStack.Count > 1)
        DocumentMapNode.CollapseTopLevel(nodeStack, workspace);
      return nodeStack.Pop().Node;
    }

    private static void CollapseTopLevel(
      Stack<DocumentMapNode.NodeStackEntry> nodeStack,
      List<DocumentMapNode> workspace)
    {
      if (nodeStack == null || nodeStack.Count <= 1)
        return;
      int level = nodeStack.Peek().Level;
      workspace.Clear();
      while (nodeStack.Peek().Level == level)
        workspace.Add(nodeStack.Pop().Node);
      DocumentMapNode node = nodeStack.Peek().Node;
      node.SetNodeChildren((IList<DocumentMapNode>) new DocumentMapNode[workspace.Count]);
      for (int index = workspace.Count - 1; index >= 0; --index)
        node.Children[workspace.Count - index - 1] = workspace[index];
    }

    private static DocumentMapNode FromOnDemandNode(OnDemandDocumentMapNode node)
    {
      return new DocumentMapNode(node.Label, node.Id, (DocumentMapNode[]) null);
    }

    internal static DocumentMapNode CreateNode(Microsoft.Reporting.WebForms.Internal.Soap.ReportingServices2005.Execution.DocumentMapNode serverNode)
    {
      if (serverNode == null)
        return (DocumentMapNode) null;
      int length = 0;
      if (serverNode.Children != null)
        length = serverNode.Children.Length;
      DocumentMapNode[] children = new DocumentMapNode[length];
      for (int index = 0; index < length; ++index)
        children[index] = DocumentMapNode.CreateNode(serverNode.Children[index]);
      return new DocumentMapNode(serverNode.Label, serverNode.UniqueName, children);
    }

    public string Label => this.m_label;

    public string Id => this.m_id;

    public IList<DocumentMapNode> Children => this.m_children;

    private void SetNodeChildren(IList<DocumentMapNode> children) => this.m_children = children;

    private struct NodeStackEntry
    {
      public DocumentMapNode Node;
      public int Level;
    }
  }
}
