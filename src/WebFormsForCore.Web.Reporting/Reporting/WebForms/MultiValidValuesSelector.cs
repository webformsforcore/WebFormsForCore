// Decompiled with JetBrains decompiler
// Type: Microsoft.Reporting.WebForms.MultiValidValuesSelector
// Assembly: Microsoft.ReportViewer.WebForms, Version=12.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91
// MVID: F82ADCE6-59A1-4E44-AF2B-7E8AD7E2F93B
// Assembly location: C:\Users\simon\Downloads\Microsoft.ReportViewer.WebForms.dll

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

#nullable disable
namespace Microsoft.Reporting.WebForms
{
  internal sealed class MultiValidValuesSelector : MultiValueSelector
  {
    private Dictionary<string, CheckBox> m_valueToCheckBox = new Dictionary<string, CheckBox>();
    private List<CheckBox> m_allCheckBoxes = new List<CheckBox>();
    private IList<ValidValue> m_validValues;
    private BaseParameterInputControl m_containingControl;
    private CheckBox m_selectAll;
    private string m_checkBoxCssClass;
    private MultiValidValuesSelector.SelectedIndicies m_selectedIndicies;

    public MultiValidValuesSelector(
      IList<ValidValue> validValues,
      BaseParameterInputControl containingControl)
    {
      this.m_validValues = validValues;
      this.m_containingControl = containingControl;
    }

    public override void AddScriptDescriptors(ScriptControlDescriptor desc)
    {
      this.EnsureChildControls();
      base.AddScriptDescriptors(desc);
      desc.AddProperty("HiddenIndicesId", (object) this.m_selectedIndicies.HiddenFieldForClientUpdates);
    }

    public string CheckBoxCssClass
    {
      get => this.m_checkBoxCssClass;
      set => this.m_checkBoxCssClass = value;
    }

    protected override void CreateChildControls()
    {
      this.Controls.Clear();
      base.CreateChildControls();
      this.m_valueToCheckBox.Clear();
      this.m_allCheckBoxes.Clear();
      this.m_selectAll = (CheckBox) new MultiValidValuesSelector.NonPostBackCheckBox();
      this.m_selectAll.EnableViewState = false;
      this.m_selectAll.Text = HttpUtility.HtmlEncode(LocalizationHelper.Current.SelectAll);
      this.Controls.Add((Control) this.m_selectAll);
      this.m_selectedIndicies = new MultiValidValuesSelector.SelectedIndicies((IEnumerable<ValidValue>) this.m_validValues);
      this.m_selectedIndicies.ValueChanged += new EventHandler(((MultiValueSelector) this).OnChange);
      this.Controls.Add((Control) this.m_selectedIndicies);
      if (this.m_validValues == null)
        return;
      foreach (ValidValue validValue in (IEnumerable<ValidValue>) this.m_validValues)
      {
        CheckBox child = (CheckBox) new MultiValidValuesSelector.NonPostBackCheckBox();
        string str = HttpUtility.HtmlEncode(this.m_containingControl.GetLabelForValidValue(validValue));
        child.Text = str.Replace(" ", "&nbsp;");
        child.EnableViewState = false;
        this.Controls.Add((Control) child);
        if (!this.m_valueToCheckBox.ContainsKey(validValue.Value))
          this.m_valueToCheckBox.Add(validValue.Value, child);
        this.m_allCheckBoxes.Add(child);
      }
    }

    protected override void Render(HtmlTextWriter writer)
    {
      this.EnsureChildControls();
      this.SetSelectorBorder();
      this.AddAttributesToRender(writer);
      if (this.m_allCheckBoxes.Count > 6)
        writer.AddStyleAttribute(HtmlTextWriterStyle.Height, "150px");
      else if (this.m_allCheckBoxes.Count < 3)
        writer.AddStyleAttribute(HtmlTextWriterStyle.Height, "75px");
      writer.AddStyleAttribute(HtmlTextWriterStyle.Overflow, "auto");
      if (string.IsNullOrEmpty(this.CssClass))
        writer.AddStyleAttribute(HtmlTextWriterStyle.BackgroundColor, "window");
      writer.RenderBeginTag(HtmlTextWriterTag.Div);
      writer.AddAttribute(HtmlTextWriterAttribute.Cellpadding, "0");
      writer.AddAttribute(HtmlTextWriterAttribute.Cellspacing, "0");
      if (string.IsNullOrEmpty(this.CssClass))
        writer.AddStyleAttribute(HtmlTextWriterStyle.BackgroundColor, "window");
      writer.RenderBeginTag(HtmlTextWriterTag.Table);
      string input = (string) null;
      if (this.m_allCheckBoxes.Count > 1)
      {
        this.m_selectAll.Attributes.Add("onclick", string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}.OnSelectAllClick(this);", (object) this.ClientSideObjectName));
        input = this.m_selectAll.ClientID;
        this.WriteCheckBox(this.m_selectAll, writer);
      }
      foreach (CheckBox allCheckBox in this.m_allCheckBoxes)
      {
        allCheckBox.Attributes["onclick"] = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}.OnValidValueClick(this, '{1}');", (object) this.ClientSideObjectName, (object) JavaScriptHelper.StringEscapeSingleQuote(input));
        this.WriteCheckBox(allCheckBox, writer);
      }
      writer.RenderEndTag();
      this.m_selectedIndicies.RenderControl(writer);
      writer.RenderEndTag();
    }

    private void WriteCheckBox(CheckBox checkBox, HtmlTextWriter writer)
    {
      writer.RenderBeginTag(HtmlTextWriterTag.Tr);
      writer.AddAttribute(HtmlTextWriterAttribute.Nowrap, "nowrap");
      writer.RenderBeginTag(HtmlTextWriterTag.Td);
      checkBox.Font.CopyFrom(this.Font);
      checkBox.CssClass = this.CheckBoxCssClass;
      checkBox.RenderControl(writer);
      writer.RenderEndTag();
      writer.RenderEndTag();
    }

    public override bool HasValue
    {
      get
      {
        this.EnsureChildControls();
        IList<int> indicies = this.m_selectedIndicies.Indicies;
        return indicies != null && indicies.Count > 0;
      }
    }

    public override string[] Value
    {
      get
      {
        this.EnsureChildControls();
        IList<int> indicies = this.m_selectedIndicies.Indicies;
        List<string> stringList = new List<string>(indicies.Count);
        foreach (int index in (IEnumerable<int>) indicies)
          stringList.Add(this.m_validValues[index].Value);
        return stringList.ToArray();
      }
      set
      {
        this.EnsureChildControls();
        int count = this.m_allCheckBoxes.Count;
        for (int index = 0; index < count; ++index)
          this.m_allCheckBoxes[index].Checked = false;
        foreach (string key in value)
        {
          CheckBox checkBox;
          if (this.m_valueToCheckBox.TryGetValue(key, out checkBox))
            checkBox.Checked = true;
        }
        List<int> intList = new List<int>();
        bool flag = false;
        for (int index = 0; index < count; ++index)
        {
          if (!this.m_allCheckBoxes[index].Checked)
            flag = true;
          else
            intList.Add(index);
        }
        if (this.m_selectAll != null)
          this.m_selectAll.Checked = !flag;
        this.m_selectedIndicies.Indicies = (IList<int>) intList;
      }
    }

    private class NonPostBackCheckBox : CheckBox
    {
      protected override bool LoadPostData(string postDataKey, NameValueCollection postCollection)
      {
        return false;
      }
    }

    private class SelectedIndicies : CompositeControl
    {
      private const string ValidValueListHash = "ValidValueHash";
      private HiddenField m_hiddenIndices;
      private uint? m_validValuesHash;

      public event EventHandler ValueChanged;

      public SelectedIndicies(IEnumerable<ValidValue> validValues)
      {
        this.m_validValuesHash = new uint?(MultiValidValuesSelector.SelectedIndicies.ComputeValidValuesHash(validValues));
      }

      public string HiddenFieldForClientUpdates
      {
        get
        {
          this.EnsureChildControls();
          return this.m_hiddenIndices.ClientID;
        }
      }

      public IList<int> Indicies
      {
        get
        {
          this.EnsureChildControls();
          uint? nullable1 = (uint?) this.ViewState["ValidValueHash"];
          if (nullable1.HasValue)
          {
            uint? nullable2 = nullable1;
            uint? validValuesHash = this.m_validValuesHash;
            if (((int) nullable2.GetValueOrDefault() != (int) validValuesHash.GetValueOrDefault() ? 1 : (nullable2.HasValue != validValuesHash.HasValue ? 1 : 0)) != 0)
              return (IList<int>) null;
          }
          string[] strArray = this.m_hiddenIndices.Value.Split(',');
          List<int> indicies = new List<int>(strArray.Length);
          foreach (string s in strArray)
          {
            int result;
            if (!int.TryParse(s, out result))
              return (IList<int>) null;
            indicies.Add(result);
          }
          return (IList<int>) indicies;
        }
        set
        {
          this.EnsureChildControls();
          StringBuilder stringBuilder = new StringBuilder();
          if (value != null)
          {
            foreach (int num in (IEnumerable<int>) value)
            {
              if (stringBuilder.Length > 0)
                stringBuilder.Append(',');
              stringBuilder.Append(num.ToString((IFormatProvider) CultureInfo.InvariantCulture));
            }
          }
          this.m_hiddenIndices.Value = stringBuilder.ToString();
          this.ViewState["ValidValueHash"] = (object) this.m_validValuesHash;
        }
      }

      protected override void CreateChildControls()
      {
        base.CreateChildControls();
        this.Controls.Clear();
        this.m_hiddenIndices = new HiddenField();
        this.m_hiddenIndices.ID = "HiddenIndices";
        this.m_hiddenIndices.ValueChanged += new EventHandler(this.OnChange);
        this.Controls.Add((Control) this.m_hiddenIndices);
      }

      private void OnChange(object sender, EventArgs e)
      {
        if (this.ValueChanged == null)
          return;
        this.ValueChanged((object) this, EventArgs.Empty);
      }

      private static uint ComputeValidValuesHash(IEnumerable<ValidValue> validValues)
      {
        uint validValuesHash = 0;
        if (validValues != null)
        {
          foreach (ValidValue validValue in validValues)
          {
            ulong num = (ulong) validValuesHash + (ulong) validValue.Label.GetHashCode() + (ulong) validValue.Value.GetHashCode();
            validValuesHash = num > (ulong) uint.MaxValue ? (uint) (num % (ulong) uint.MaxValue) : (uint) num;
          }
        }
        return validValuesHash;
      }
    }
  }
}
