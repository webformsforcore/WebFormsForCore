// Decompiled with JetBrains decompiler
// Type: Microsoft.ReportingServices.Rendering.HtmlRenderer.ListLevelStack
// Assembly: Microsoft.ReportViewer.WebForms, Version=12.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91
// MVID: F82ADCE6-59A1-4E44-AF2B-7E8AD7E2F93B
// Assembly location: C:\Users\simon\Downloads\Microsoft.ReportViewer.WebForms.dll

using Microsoft.ReportingServices.Rendering.RPLProcessing;
using System.Collections.Generic;

#nullable disable
namespace Microsoft.ReportingServices.Rendering.HtmlRenderer
{
  internal class ListLevelStack
  {
    private List<ListLevel> m_listLevels = new List<ListLevel>();

    internal void PushTo(
      HTML4Renderer renderer,
      int listLevel,
      RPLFormat.ListStyles style,
      bool writeNoVerticalMargin)
    {
      if (listLevel == 0)
        this.PopAll();
      else if (this.m_listLevels.Count == 0)
      {
        this.Push(renderer, listLevel, style, writeNoVerticalMargin);
      }
      else
      {
        ListLevel listLevel1 = this.m_listLevels[this.m_listLevels.Count - 1];
        if (listLevel == listLevel1.Level)
        {
          if (style == listLevel1.Style)
            return;
          this.Pop();
          this.Push(renderer, listLevel, style, writeNoVerticalMargin);
        }
        else if (listLevel > listLevel1.Level)
        {
          this.Push(renderer, listLevel, style, writeNoVerticalMargin);
        }
        else
        {
          for (; listLevel < listLevel1.Level; listLevel1 = this.m_listLevels[this.m_listLevels.Count - 1])
          {
            this.Pop();
            if (this.m_listLevels.Count == 0)
            {
              listLevel1 = (ListLevel) null;
              break;
            }
          }
          if (listLevel1 != null && listLevel1.Style != style)
            this.Pop();
          this.Push(renderer, listLevel, style, writeNoVerticalMargin);
        }
      }
    }

    internal void Pop()
    {
      if (this.m_listLevels.Count == 0)
        return;
      ListLevel listLevel = this.m_listLevels[this.m_listLevels.Count - 1];
      this.m_listLevels.RemoveAt(this.m_listLevels.Count - 1);
      listLevel.Close();
    }

    internal void PopAll()
    {
      for (int index = this.m_listLevels.Count - 1; index > -1; --index)
        this.Pop();
    }

    internal ListLevel Push(
      HTML4Renderer renderer,
      int listLevel,
      RPLFormat.ListStyles style,
      bool writeNoVerticalMarginClass)
    {
      int num = listLevel - this.m_listLevels.Count;
      ListLevel listLevel1 = (ListLevel) null;
      for (; num > 0; --num)
      {
        listLevel1 = new ListLevel(renderer, this.m_listLevels.Count + 1, style);
        this.m_listLevels.Add(listLevel1);
        listLevel1.Open(writeNoVerticalMarginClass);
      }
      return listLevel1;
    }
  }
}
