
using Microsoft.Reporting.WebForms.Internal.Soap.ReportingServices2005.Execution;
using Microsoft.SqlServer.ReportingServices;
using System;
using System.Runtime.Serialization;
using System.Security;
using System.Security.Permissions;
using System.Web.Services.Protocols;
using System.Xml;

#nullable disable
namespace Microsoft.Reporting.WebForms
{
  [Serializable]
  [StrongNameIdentityPermission(SecurityAction.InheritanceDemand, PublicKey = "0024000004800000940000000602000000240000525341310004000001000100272736ad6e5f9586bac2d531eabc3acc666c2f8ec879fa94f8f7b0327d2ff2ed523448f83c3d5c5dd2dfc7bc99c5286b2c125117bf5cbe242b9d41750732b2bdffe649c6efb8e5526d526fdd130095ecdb7bf210809c6cdad8824faa9ac0310ac3cba2aa0523567b2dfa7fe250b30facbd62d4ec99b94ac47c7d3b28f1f6e4c8")]
  public class ReportServerException : ReportViewerException
  {
    private const string SoapErrorNamespace = "http://www.microsoft.com/sql/reportingservices";
    private string m_errorCode = "";

    protected ReportServerException(string message, string errorCode, Exception innerException)
      : base(message, innerException)
    {
      if (errorCode == null)
        return;
      this.m_errorCode = errorCode;
    }

    protected ReportServerException(SerializationInfo info, StreamingContext context)
      : base(info, context)
    {
      info.AddValue("ReportServerErrorCode", (object) this.m_errorCode);
    }

    [SecurityCritical]
    [SecurityTreatAsSafe]
    [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.SerializationFormatter)]
    public override void GetObjectData(SerializationInfo info, StreamingContext context)
    {
      base.GetObjectData(info, context);
      this.m_errorCode = info.GetValue("ReportServerErrorCode", typeof (string)) as string;
    }

    public string ErrorCode => this.m_errorCode;

    internal static ReportServerException FromException(Exception e)
    {
      switch (e)
      {
        case ReportServerException reportServerException2:
          return reportServerException2;
        case SoapException soapException:
          ReportServerException reportServerException1 = ReportServerException.FromMoreInformationNode(ReportServerException.GetNestedMoreInformationNode(soapException.Detail));
          if (reportServerException1 != null)
            return reportServerException1;
          break;
        case RSExecutionConnection.MissingEndpointException _:
          return new ReportServerException(e.Message, (string) null, (Exception) new MissingEndpointException(e.Message, e.InnerException));
        case RSExecutionConnection.SoapVersionMismatchException _:
          return new ReportServerException(e.Message, (string) null, (Exception) new SoapVersionMismatchException(e.Message, e.InnerException));
      }
      return new ReportServerException(e.Message, (string) null, e);
    }

    internal static ReportServerException FromMoreInformationNode(XmlNode moreInfoNode)
    {
      if (moreInfoNode == null)
        return (ReportServerException) null;
      XmlNamespaceManager nsmgr = new XmlNamespaceManager(moreInfoNode.OwnerDocument.NameTable);
      nsmgr.AddNamespace("rs", "http://www.microsoft.com/sql/reportingservices");
      string message = (string) null;
      string errorCode = (string) null;
      XmlNode xmlNode = moreInfoNode.SelectSingleNode("rs:Message", nsmgr);
      if (xmlNode != null)
      {
        string innerText = xmlNode.InnerText;
        XmlNode namedItem = xmlNode.Attributes.GetNamedItem("ErrorCode", "http://www.microsoft.com/sql/reportingservices");
        if (namedItem != null)
          errorCode = namedItem.Value;
        message = !string.IsNullOrEmpty(errorCode) ? SoapExceptionStrings.RSSoapMessageFormat(innerText, errorCode) : innerText;
      }
      ReportServerException innerException = ReportServerException.FromMoreInformationNode(ReportServerException.GetNestedMoreInformationNode(moreInfoNode));
      return string.IsNullOrEmpty(message) ? innerException : new ReportServerException(message, errorCode, (Exception) innerException);
    }

    private static XmlNode GetNestedMoreInformationNode(XmlNode node)
    {
      if (node == null)
        return (XmlNode) null;
      XmlNamespaceManager nsmgr = new XmlNamespaceManager(node.OwnerDocument.NameTable);
      nsmgr.AddNamespace("rs", "http://www.microsoft.com/sql/reportingservices");
      return node.SelectSingleNode("rs:MoreInformation", nsmgr);
    }
  }
}
