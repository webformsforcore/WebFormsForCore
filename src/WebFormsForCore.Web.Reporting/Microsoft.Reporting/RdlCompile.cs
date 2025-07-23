using System;
using System.Collections;
using System.Globalization;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using Microsoft.Build.Framework;
using Microsoft.ReportingServices.Diagnostics;
using Microsoft.ReportingServices.Library;
using Microsoft.ReportingServices.ReportProcessing;

namespace Microsoft.Reporting;

internal sealed class RdlCompile : ITask
{
	[Serializable]
	private class StateCache : Hashtable
	{
		public DateTime this[string key]
		{
			get
			{
				if (base[key] != null)
				{
					return (DateTime)base[key];
				}
				return DateTime.MinValue;
			}
			set
			{
				base[key] = value;
			}
		}

		public StateCache()
			: base(StringComparer.OrdinalIgnoreCase)
		{
		}

		protected StateCache(SerializationInfo serializationInfo, StreamingContext context)
			: base(serializationInfo, context)
		{
		}
	}

	private string[] m_sources;

	private string m_stateFile;

	private string m_timeStampFile;

	private StateCache m_stateCache;

	private StateCache m_newState;

	private IBuildEngine m_buildEngine;

	public string[] Sources
	{
		get
		{
			return m_sources;
		}
		set
		{
			m_sources = value;
		}
	}

	[Output]
	public string StateFile
	{
		get
		{
			return m_stateFile;
		}
		set
		{
			m_stateFile = value;
		}
	}

	[Output]
	public string TimeStampFile
	{
		get
		{
			return m_timeStampFile;
		}
		set
		{
			m_timeStampFile = value;
		}
	}

	public IBuildEngine BuildEngine
	{
		get
		{
			return m_buildEngine;
		}
		set
		{
			m_buildEngine = value;
		}
	}

	public ITaskHost HostObject
	{
		get
		{
			return null;
		}
		set
		{
		}
	}

	public bool Execute()
	{
		bool flag = true;
		ReadStateFile();
		m_newState = new StateCache();
		for (int i = 0; i < m_sources.Length; i++)
		{
			if (!CompileReport(m_sources[i]))
			{
				flag = false;
			}
		}
		WriteStateFile();
		if (flag)
		{
			WriteTimeStampFile();
		}
		return flag;
	}

	private bool CompileReport(string fileName)
	{
		//IL_0095: Expected O, but got Unknown
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Expected O, but got Unknown
		if (string.IsNullOrEmpty(fileName))
		{
			return true;
		}
		DateTime lastWriteTime;
		byte[] array;
		try
		{
			lastWriteTime = File.GetLastWriteTime(fileName);
			if (m_stateCache != null && lastWriteTime <= m_stateCache[fileName])
			{
				m_newState[fileName] = lastWriteTime;
				return true;
			}
			array = File.ReadAllBytes(fileName);
		}
		catch (Exception ex)
		{
			LogError(ProcessingStrings.RdlCompile_CouldNotOpenFile, new object[2] { fileName, ex.Message });
			return false;
		}
		try
		{
			ControlSnapshot val2 = default(ControlSnapshot);
			PublishingResult val = ReportCompiler.CompileReport((ICatalogItemContext)new PreviewItemContext(), array, false, ref val2);
			LogMessages(fileName, (ICollection)val.Warnings);
		}
		catch (DefinitionInvalidException ex2)
		{
			DefinitionInvalidException ex3 = ex2;
			Exception innerException = ((Exception)(object)ex3).InnerException;
			ReportProcessingException ex4 = (ReportProcessingException)(object)((innerException is ReportProcessingException) ? innerException : null);
			if (ex4 != null)
			{
				LogMessages(fileName, (ICollection)ex4.ProcessingMessages);
			}
			return false;
		}
		catch (Exception exception)
		{
			LogErrorFromException(exception);
			return false;
		}
		m_newState[fileName] = lastWriteTime;
		return true;
	}

	private void LogMessages(string fileName, ICollection messages)
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Expected O, but got Unknown
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Invalid comparison between Unknown and I4
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		if (messages == null)
		{
			return;
		}
		foreach (ProcessingMessage message in messages)
		{
			ProcessingMessage val = message;
			Severity severity = val.Severity;
			if ((int)severity == 1)
			{
				LogWarning(null, ((object)val.Code).ToString(), null, fileName, 0, 0, 0, 0, val.Message, null);
			}
			else
			{
				LogError(null, ((object)val.Code).ToString(), null, fileName, 0, 0, 0, 0, val.Message, null);
			}
		}
	}

	private void ReadStateFile()
	{
		try
		{
			if (!string.IsNullOrEmpty(m_stateFile) && File.Exists(m_stateFile))
			{
				using (FileStream serializationStream = new FileStream(m_stateFile, FileMode.Open))
				{
					BinaryFormatter binaryFormatter = new BinaryFormatter();
					m_stateCache = binaryFormatter.Deserialize(serializationStream) as StateCache;
					return;
				}
			}
		}
		catch (Exception)
		{
		}
	}

	private void WriteStateFile()
	{
		try
		{
			if (!string.IsNullOrEmpty(m_stateFile))
			{
				if (File.Exists(m_stateFile))
				{
					File.Delete(m_stateFile);
				}
				using FileStream serializationStream = new FileStream(m_stateFile, FileMode.CreateNew);
				BinaryFormatter binaryFormatter = new BinaryFormatter();
				binaryFormatter.Serialize(serializationStream, m_newState);
				return;
			}
		}
		catch (Exception ex)
		{
			LogWarning(ProcessingStrings.RdlCompile_CouldNotWriteStateFile, new object[2] { m_stateFile, ex.Message });
		}
	}

	private void WriteTimeStampFile()
	{
		try
		{
			if (!string.IsNullOrEmpty(m_timeStampFile))
			{
				if (File.Exists(m_timeStampFile))
				{
					File.Delete(m_timeStampFile);
				}
				File.Create(m_timeStampFile).Close();
			}
		}
		catch (Exception ex)
		{
			LogWarning(ProcessingStrings.RdlCompile_CouldNotWriteStateFile, new object[2] { m_stateFile, ex.Message });
		}
	}

	private static string FormatString(string unformatted, params object[] args)
	{
		string result = unformatted;
		if (args != null && args.Length > 0)
		{
			result = string.Format(CultureInfo.CurrentCulture, unformatted, args);
		}
		return result;
	}

	private void LogWarning(string message, object[] messageArgs)
	{
		LogWarning(null, null, null, null, 0, 0, 0, 0, message, messageArgs);
	}

	private void LogWarning(string subcategory, string warningCode, string helpKeyword, string file, int lineNumber, int columnNumber, int endLineNumber, int endColumnNumber, string message, object[] messageArgs)
	{
		if (message == null)
		{
			throw new ArgumentNullException("message");
		}
		if (file == null || file.Length == 0)
		{
			file = BuildEngine.ProjectFileOfTaskNode;
		}
		BuildWarningEventArgs e = new BuildWarningEventArgs(subcategory, warningCode, file, lineNumber, columnNumber, endLineNumber, endColumnNumber, FormatString(message, messageArgs), helpKeyword, GetType().Name);
		BuildEngine.LogWarningEvent(e);
	}

	public void LogErrorFromException(Exception exception)
	{
		LogError(exception.Message, new object[0]);
	}

	private void LogError(string message, object[] messageArgs)
	{
		LogError(null, null, null, null, 0, 0, 0, 0, message, messageArgs);
	}

	private void LogError(string subcategory, string errorCode, string helpKeyword, string file, int lineNumber, int columnNumber, int endLineNumber, int endColumnNumber, string message, object[] messageArgs)
	{
		if (message == null)
		{
			throw new ArgumentNullException("message");
		}
		if ((file == null || file.Length == 0) && lineNumber == 0 && columnNumber == 0 && !BuildEngine.ContinueOnError)
		{
			file = BuildEngine.ProjectFileOfTaskNode;
			lineNumber = BuildEngine.LineNumberOfTaskNode;
			columnNumber = BuildEngine.ColumnNumberOfTaskNode;
		}
		BuildErrorEventArgs e = new BuildErrorEventArgs(subcategory, errorCode, file, lineNumber, columnNumber, endLineNumber, endColumnNumber, FormatString(message, messageArgs), helpKeyword, GetType().Name);
		BuildEngine.LogErrorEvent(e);
	}
}
