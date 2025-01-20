
using Microsoft.Build.Framework;
using Microsoft.ReportingServices.Diagnostics;
using Microsoft.ReportingServices.Library;
using Microsoft.ReportingServices.ReportProcessing;
using System;
using System.Collections;
using System.Globalization;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

#nullable disable
namespace Microsoft.Reporting
{
  internal sealed class RdlCompile : ITask
  {
    private string[] m_sources;
    private string m_stateFile;
    private string m_timeStampFile;
    private RdlCompile.StateCache m_stateCache;
    private RdlCompile.StateCache m_newState;
    private IBuildEngine m_buildEngine;

    public string[] Sources
    {
      get => this.m_sources;
      set => this.m_sources = value;
    }

    [Output]
    public string StateFile
    {
      get => this.m_stateFile;
      set => this.m_stateFile = value;
    }

    [Output]
    public string TimeStampFile
    {
      get => this.m_timeStampFile;
      set => this.m_timeStampFile = value;
    }

    public IBuildEngine BuildEngine
    {
      get => this.m_buildEngine;
      set => this.m_buildEngine = value;
    }

    public ITaskHost HostObject
    {
      get => (ITaskHost) null;
      set
      {
      }
    }

    public bool Execute()
    {
      bool flag = true;
      this.ReadStateFile();
      this.m_newState = new RdlCompile.StateCache();
      for (int index = 0; index < this.m_sources.Length; ++index)
      {
        if (!this.CompileReport(this.m_sources[index]))
          flag = false;
      }
      this.WriteStateFile();
      if (flag)
        this.WriteTimeStampFile();
      return flag;
    }

    private bool CompileReport(string fileName)
    {
      if (string.IsNullOrEmpty(fileName))
        return true;
      DateTime lastWriteTime;
      byte[] numArray;
      try
      {
        lastWriteTime = File.GetLastWriteTime(fileName);
        if (this.m_stateCache != null && lastWriteTime <= this.m_stateCache[fileName])
        {
          this.m_newState[fileName] = lastWriteTime;
          return true;
        }
        numArray = File.ReadAllBytes(fileName);
      }
      catch (Exception ex)
      {
        this.LogError(ProcessingStrings.RdlCompile_CouldNotOpenFile, new object[2]
        {
          (object) fileName,
          (object) ex.Message
        });
        return false;
      }
      try
      {
        ControlSnapshot controlSnapshot;
        PublishingResult publishingResult = ReportCompiler.CompileReport((ICatalogItemContext) new PreviewItemContext(), numArray, false, ref controlSnapshot);
        this.LogMessages(fileName, (ICollection) publishingResult.Warnings);
      }
      catch (DefinitionInvalidException ex)
      {
        if (((Exception) ex).InnerException is ReportProcessingException innerException)
          this.LogMessages(fileName, (ICollection) innerException.ProcessingMessages);
        return false;
      }
      catch (Exception ex)
      {
        this.LogErrorFromException(ex);
        return false;
      }
      this.m_newState[fileName] = lastWriteTime;
      return true;
    }

    private void LogMessages(string fileName, ICollection messages)
    {
      if (messages == null)
        return;
      foreach (ProcessingMessage message in (IEnumerable) messages)
      {
        if (message.Severity == 1)
          this.LogWarning((string) null, message.Code.ToString(), (string) null, fileName, 0, 0, 0, 0, message.Message, (object[]) null);
        else
          this.LogError((string) null, message.Code.ToString(), (string) null, fileName, 0, 0, 0, 0, message.Message, (object[]) null);
      }
    }

    private void ReadStateFile()
    {
      try
      {
        if (string.IsNullOrEmpty(this.m_stateFile) || !File.Exists(this.m_stateFile))
          return;
        using (FileStream serializationStream = new FileStream(this.m_stateFile, FileMode.Open))
          this.m_stateCache = new BinaryFormatter().Deserialize((Stream) serializationStream) as RdlCompile.StateCache;
      }
      catch (Exception ex)
      {
      }
    }

    private void WriteStateFile()
    {
      try
      {
        if (string.IsNullOrEmpty(this.m_stateFile))
          return;
        if (File.Exists(this.m_stateFile))
          File.Delete(this.m_stateFile);
        using (FileStream serializationStream = new FileStream(this.m_stateFile, FileMode.CreateNew))
          new BinaryFormatter().Serialize((Stream) serializationStream, (object) this.m_newState);
      }
      catch (Exception ex)
      {
        this.LogWarning(ProcessingStrings.RdlCompile_CouldNotWriteStateFile, new object[2]
        {
          (object) this.m_stateFile,
          (object) ex.Message
        });
      }
    }

    private void WriteTimeStampFile()
    {
      try
      {
        if (string.IsNullOrEmpty(this.m_timeStampFile))
          return;
        if (File.Exists(this.m_timeStampFile))
          File.Delete(this.m_timeStampFile);
        File.Create(this.m_timeStampFile).Close();
      }
      catch (Exception ex)
      {
        this.LogWarning(ProcessingStrings.RdlCompile_CouldNotWriteStateFile, new object[2]
        {
          (object) this.m_stateFile,
          (object) ex.Message
        });
      }
    }

    private static string FormatString(string unformatted, params object[] args)
    {
      string str = unformatted;
      if (args != null && args.Length > 0)
        str = string.Format((IFormatProvider) CultureInfo.CurrentCulture, unformatted, args);
      return str;
    }

    private void LogWarning(string message, object[] messageArgs)
    {
      this.LogWarning((string) null, (string) null, (string) null, (string) null, 0, 0, 0, 0, message, messageArgs);
    }

    private void LogWarning(
      string subcategory,
      string warningCode,
      string helpKeyword,
      string file,
      int lineNumber,
      int columnNumber,
      int endLineNumber,
      int endColumnNumber,
      string message,
      object[] messageArgs)
    {
      if (message == null)
        throw new ArgumentNullException(nameof (message));
      if (file == null || file.Length == 0)
        file = this.BuildEngine.ProjectFileOfTaskNode;
      this.BuildEngine.LogWarningEvent(new BuildWarningEventArgs(subcategory, warningCode, file, lineNumber, columnNumber, endLineNumber, endColumnNumber, RdlCompile.FormatString(message, messageArgs), helpKeyword, this.GetType().Name));
    }

    public void LogErrorFromException(Exception exception)
    {
      this.LogError(exception.Message, new object[0]);
    }

    private void LogError(string message, object[] messageArgs)
    {
      this.LogError((string) null, (string) null, (string) null, (string) null, 0, 0, 0, 0, message, messageArgs);
    }

    private void LogError(
      string subcategory,
      string errorCode,
      string helpKeyword,
      string file,
      int lineNumber,
      int columnNumber,
      int endLineNumber,
      int endColumnNumber,
      string message,
      object[] messageArgs)
    {
      if (message == null)
        throw new ArgumentNullException(nameof (message));
      if ((file == null || file.Length == 0) && lineNumber == 0 && columnNumber == 0 && !this.BuildEngine.ContinueOnError)
      {
        file = this.BuildEngine.ProjectFileOfTaskNode;
        lineNumber = this.BuildEngine.LineNumberOfTaskNode;
        columnNumber = this.BuildEngine.ColumnNumberOfTaskNode;
      }
      this.BuildEngine.LogErrorEvent(new BuildErrorEventArgs(subcategory, errorCode, file, lineNumber, columnNumber, endLineNumber, endColumnNumber, RdlCompile.FormatString(message, messageArgs), helpKeyword, this.GetType().Name));
    }

    [Serializable]
    private class StateCache : Hashtable
    {
      public StateCache()
        : base((IEqualityComparer) StringComparer.OrdinalIgnoreCase)
      {
      }

      protected StateCache(SerializationInfo serializationInfo, StreamingContext context)
        : base(serializationInfo, context)
      {
      }

      public DateTime this[string key]
      {
        get => this[(object) key] != null ? (DateTime) this[(object) key] : DateTime.MinValue;
        set => this[(object) key] = (object) value;
      }
    }
  }
}
