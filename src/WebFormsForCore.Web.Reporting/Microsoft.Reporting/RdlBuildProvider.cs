using System;
using System.Collections;
using System.IO;
using System.Web;
using System.Web.Compilation;
using System.Web.Hosting;
using Microsoft.ReportingServices.Diagnostics;
using Microsoft.ReportingServices.Library;
using Microsoft.ReportingServices.ReportProcessing;

namespace Microsoft.Reporting;

internal sealed class RdlBuildProvider : BuildProvider
{
	public override CompilerType CodeCompilerType
	{
		get
		{
			CompileReport();
			return null;
		}
	}

	private void CompileReport()
	{
		//IL_0037: Expected O, but got Unknown
		//IL_00ba: Expected O, but got Unknown
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Expected O, but got Unknown
		HttpContext current = HttpContext.Current;
		if (current != null)
		{
			return;
		}
		byte[] streamBytes = GetStreamBytes(OpenStream());
		HostingEnvironment.MapPath(base.VirtualPath);
		try
		{
			ControlSnapshot val = default(ControlSnapshot);
			ReportCompiler.CompileReport((ICatalogItemContext)new PreviewItemContext(), streamBytes, false, ref val);
		}
		catch (DefinitionInvalidException ex)
		{
			DefinitionInvalidException ex2 = ex;
			Exception innerException = ((Exception)(object)ex2).InnerException;
			ReportProcessingException ex3 = (ReportProcessingException)(object)((innerException is ReportProcessingException) ? innerException : null);
			if (ex3 != null && ex3.ProcessingMessages != null)
			{
				HttpParseException ex4 = new HttpParseException(((Exception)(object)ex3).Message, null, base.VirtualPath, null, 0);
				for (int i = 1; i < ((ArrayList)(object)ex3.ProcessingMessages).Count; i++)
				{
					ProcessingMessage val2 = ex3.ProcessingMessages[i];
					ex4.ParserErrors.Add(new ParserError(val2.Message, base.VirtualPath, 0));
				}
				throw ex4;
			}
		}
		catch (ReportProcessingException ex5)
		{
			ReportProcessingException ex6 = ex5;
			throw new HttpParseException(((Exception)(object)ex6).Message);
		}
	}

	public static byte[] GetStreamBytes(Stream stream)
	{
		byte[] array;
		if (stream.CanSeek)
		{
			int num = (int)stream.Length;
			array = new byte[num];
			stream.Read(array, 0, num);
			return array;
		}
		array = new byte[256];
		int num2 = 0;
		int num3;
		do
		{
			if (array.Length < num2 + 256)
			{
				byte[] array2 = new byte[array.Length * 2];
				Array.Copy(array, array2, array.Length);
				array = array2;
			}
			num3 = stream.Read(array, num2, 256);
			num2 += num3;
		}
		while (num3 != 0);
		return array;
	}
}
