using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Microsoft.Reporting.WebForms.Internal.Soap.ReportingServices2005.Execution;
using Microsoft.ReportingServices.ReportProcessing;

namespace Microsoft.Reporting.WebForms;

public sealed class DocumentMapNode
{
	private struct NodeStackEntry
	{
		public DocumentMapNode Node;

		public int Level;
	}

	private string m_label;

	private string m_id;

	private IList<DocumentMapNode> m_children;

	public string Label => m_label;

	public string Id => m_id;

	public IList<DocumentMapNode> Children => m_children;

	internal DocumentMapNode(string label, string id, DocumentMapNode[] children)
	{
		m_label = label;
		m_id = id;
		if (children != null)
		{
			m_children = new ReadOnlyCollection<DocumentMapNode>(children);
		}
		else
		{
			m_children = new ReadOnlyCollection<DocumentMapNode>(new DocumentMapNode[0]);
		}
	}

	internal static DocumentMapNode CreateTree(IDocumentMap docMap, string rootName)
	{
		DocumentMapNode documentMapNode = CreateNode(docMap);
		if (documentMapNode != null)
		{
			documentMapNode.m_label = rootName;
		}
		return documentMapNode;
	}

	internal static DocumentMapNode CreateTree(Microsoft.Reporting.WebForms.Internal.Soap.ReportingServices2005.Execution.DocumentMapNode serverNode, string rootName)
	{
		DocumentMapNode documentMapNode = CreateNode(serverNode);
		if (documentMapNode != null)
		{
			documentMapNode.m_label = rootName;
		}
		return documentMapNode;
	}

	internal static DocumentMapNode CreateNode(IDocumentMap docMap)
	{
		if (docMap == null)
		{
			return null;
		}
		Stack<NodeStackEntry> stack = new Stack<NodeStackEntry>();
		List<DocumentMapNode> workspace = new List<DocumentMapNode>();
		OnDemandDocumentMapNode val = null;
		NodeStackEntry item = default(NodeStackEntry);
		while (((IEnumerator)docMap).MoveNext())
		{
			val = ((IEnumerator<OnDemandDocumentMapNode>)docMap).Current;
			item.Node = FromOnDemandNode(val);
			item.Level = val.Level;
			while (stack.Count > 0 && val.Level < stack.Peek().Level)
			{
				CollapseTopLevel(stack, workspace);
			}
			stack.Push(item);
		}
		while (stack.Count > 1)
		{
			CollapseTopLevel(stack, workspace);
		}
		return stack.Pop().Node;
	}

	private static void CollapseTopLevel(Stack<NodeStackEntry> nodeStack, List<DocumentMapNode> workspace)
	{
		if (nodeStack != null && nodeStack.Count > 1)
		{
			int level = nodeStack.Peek().Level;
			workspace.Clear();
			while (nodeStack.Peek().Level == level)
			{
				workspace.Add(nodeStack.Pop().Node);
			}
			DocumentMapNode node = nodeStack.Peek().Node;
			node.SetNodeChildren(new DocumentMapNode[workspace.Count]);
			for (int num = workspace.Count - 1; num >= 0; num--)
			{
				node.Children[workspace.Count - num - 1] = workspace[num];
			}
		}
	}

	private static DocumentMapNode FromOnDemandNode(OnDemandDocumentMapNode node)
	{
		return new DocumentMapNode(node.Label, node.Id, null);
	}

	internal static DocumentMapNode CreateNode(Microsoft.Reporting.WebForms.Internal.Soap.ReportingServices2005.Execution.DocumentMapNode serverNode)
	{
		if (serverNode == null)
		{
			return null;
		}
		int num = 0;
		if (serverNode.Children != null)
		{
			num = serverNode.Children.Length;
		}
		DocumentMapNode[] array = new DocumentMapNode[num];
		for (int i = 0; i < num; i++)
		{
			array[i] = CreateNode(serverNode.Children[i]);
		}
		return new DocumentMapNode(serverNode.Label, serverNode.UniqueName, array);
	}

	private void SetNodeChildren(IList<DocumentMapNode> children)
	{
		m_children = children;
	}
}
