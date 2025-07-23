using System;
using System.Collections;
using Microsoft.Reporting.WebForms.Internal.Soap.ReportingServices2005.Execution;
using Microsoft.ReportingServices.ReportProcessing;

namespace Microsoft.Reporting.WebForms;

public sealed class Warning
{
	public string Code { get; private set; }

	public string Message { get; private set; }

	public string ObjectName { get; private set; }

	public string ObjectType { get; private set; }

	public Severity Severity { get; private set; }

	internal Warning(string code, string message, string objectName, string objectType, string severity)
	{
		Code = code;
		Message = message;
		ObjectName = objectName;
		ObjectType = objectType;
		if (string.Compare(severity, "Warning", StringComparison.OrdinalIgnoreCase) == 0)
		{
			Severity = Severity.Warning;
		}
		else
		{
			Severity = Severity.Error;
		}
	}

	internal static Warning[] FromSoapWarnings(Microsoft.Reporting.WebForms.Internal.Soap.ReportingServices2005.Execution.Warning[] soapWarnings)
	{
		if (soapWarnings == null)
		{
			return new Warning[0];
		}
		Warning[] array = new Warning[soapWarnings.Length];
		for (int i = 0; i < soapWarnings.Length; i++)
		{
			array[i] = new Warning(soapWarnings[i].Code, soapWarnings[i].Message, soapWarnings[i].ObjectName, soapWarnings[i].ObjectType, soapWarnings[i].Severity);
		}
		return array;
	}

	internal static Warning[] FromProcessingMessageList(ProcessingMessageList processingWarnings)
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		if (processingWarnings == null)
		{
			return new Warning[0];
		}
		Warning[] array = new Warning[((ArrayList)(object)processingWarnings).Count];
		for (int i = 0; i < ((ArrayList)(object)processingWarnings).Count; i++)
		{
			array[i] = new Warning(((object)processingWarnings[i].Code).ToString(), processingWarnings[i].Message, processingWarnings[i].ObjectName, ((object)processingWarnings[i].ObjectType).ToString(), ((object)processingWarnings[i].Severity).ToString());
		}
		return array;
	}
}
