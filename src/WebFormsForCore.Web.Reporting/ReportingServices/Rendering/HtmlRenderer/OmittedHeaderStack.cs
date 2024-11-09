// Decompiled with JetBrains decompiler
// Type: Microsoft.ReportingServices.Rendering.HtmlRenderer.OmittedHeaderStack
// Assembly: Microsoft.ReportViewer.WebForms, Version=12.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91
// MVID: F82ADCE6-59A1-4E44-AF2B-7E8AD7E2F93B
// Assembly location: C:\Users\simon\Downloads\Microsoft.ReportViewer.WebForms.dll

using System.Collections.Generic;
using System.Text;

#nullable disable
namespace Microsoft.ReportingServices.Rendering.HtmlRenderer
{
  internal class OmittedHeaderStack : Stack<OmittedHeaderData>
  {
    public string GetHeaders(int column, int currentLevel, string idPrefix)
    {
      StringBuilder stringBuilder = new StringBuilder();
      foreach (OmittedHeaderData omittedHeaderData in (Stack<OmittedHeaderData>) this)
      {
        string id = omittedHeaderData.IDs[column];
        if (id != null && omittedHeaderData.Level > currentLevel)
        {
          if (stringBuilder.Length > 0)
            stringBuilder.Append(" ");
          if (idPrefix != null)
            stringBuilder.Append(idPrefix);
          stringBuilder.Append(id);
        }
      }
      return stringBuilder.ToString();
    }

    public void PopLevel(int level)
    {
      if (this.Count == 0)
        return;
      for (OmittedHeaderData omittedHeaderData = this.Peek(); omittedHeaderData.Level < level && this.Count != 0; omittedHeaderData = this.Peek())
      {
        this.Pop();
        if (this.Count <= 0)
          break;
      }
    }

    public void Push(int level, int column, int colspan, string id, int columnCount)
    {
      this.PopLevel(level);
      OmittedHeaderData omittedHeaderData = (OmittedHeaderData) null;
      if (this.Count > 0)
      {
        omittedHeaderData = this.Peek();
        if (omittedHeaderData.Level != level)
          omittedHeaderData = (OmittedHeaderData) null;
      }
      if (omittedHeaderData == null)
      {
        omittedHeaderData = new OmittedHeaderData();
        omittedHeaderData.IDs = new string[columnCount];
        omittedHeaderData.Level = level;
        this.Push(omittedHeaderData);
      }
      int num = column + colspan;
      for (int index = column; index < num; ++index)
        omittedHeaderData.IDs[index] = id;
    }
  }
}
