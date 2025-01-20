
using Microsoft.ReportingServices.Common;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Web;
using System.Web.UI;

#nullable disable
namespace Microsoft.Reporting.WebForms
{
  internal class CalendarDropDownInputControl : GenericDropDownInputControl
  {
    public CalendarDropDownInputControl(
      ReportParameterInfo reportParam,
      IBrowserDetection browserDetection,
      bool useAbsoluteScreenPositioning)
      : base(reportParam, browserDetection, useAbsoluteScreenPositioning)
    {
      this.AddSpaceBeforeImage = true;
    }

    public static bool IsSupported(IBrowserDetection browserDetection)
    {
      return !browserDetection.IsSafari && CultureInfo.CurrentCulture.Calendar is GregorianCalendar;
    }

    protected override void CreateChildControls()
    {
      this.Controls.Clear();
      base.CreateChildControls();
      this.InputControl.TextChanged += new EventHandler(((BaseParameterInputControl) this).OnCustomControlChanged);
      this.Image.Src = EmbeddedResourceOperation.CreateUrl("Microsoft.Reporting.WebForms.calendar.gif");
      this.DisabledImageLocation = EmbeddedResourceOperation.CreateUrl("Microsoft.Reporting.WebForms.calendar_disabled.gif");
      this.Iframe.Attributes.Add("scrolling", "no");
      if (!this.ReportParameter.Nullable)
        return;
      this.CreateNullCheckBox();
    }

    private string ServerCalendarUrl => CalendarIframeOperation.CreateUrl() + "&selectDate=";

    protected override void OnPreRender(EventArgs e)
    {
      this.EnsureChildControls();
      this.Iframe.Attributes.Add("src", this.ServerCalendarUrl + HttpUtility.UrlEncode(this.InputControl.Text));
      string str = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "\r\nthis.resultfield=$get('{0}');\r\nthis.resultfunc=function(resultField){{{1}.OnCalendarSelection(resultField);}}", (object) JavaScriptHelper.StringEscapeSingleQuote(this.InputControl.ClientID), (object) this.ClientObject);
      if (this.Iframe.Attributes["onload"] == null)
      {
        this.Iframe.Attributes.Add("onload", str);
      }
      else
      {
        AttributeCollection attributes;
        (attributes = this.Iframe.Attributes)["onload"] = attributes["onload"] + str;
      }
      base.OnPreRender(e);
    }

    protected override string[] CustomControlValue
    {
      get
      {
        this.EnsureChildControls();
        return new string[1]{ this.InputControl.Text };
      }
      set
      {
        if (value == null || value.Length != 1)
        {
          this.InputControl.Text = "";
        }
        else
        {
          string str = value[0];
          DateTimeOffset dateTimeOffset;
          bool flag;
          if (DateTimeUtil.TryParseDateTime(str, (CultureInfo) null, ref dateTimeOffset, ref flag))
          {
            if (flag)
              this.InputControl.Text = dateTimeOffset.ToString();
            else if (dateTimeOffset.TimeOfDay == CalendarPageControl._Midnight)
              this.InputControl.Text = dateTimeOffset.Date.ToShortDateString();
            else
              this.InputControl.Text = dateTimeOffset.DateTime.ToString();
          }
          else
            this.InputControl.Text = str;
        }
      }
    }

    protected override bool CustomControlHasValue => this.InputControl.Text.Length > 0;

    protected override string FrameAccessibleName
    {
      get => Strings.CalendarFrameAccessibleName(this.ReportParameter.Prompt);
    }

    public override IEnumerable<ScriptDescriptor> GetScriptDescriptors()
    {
      ScriptControlDescriptor desc = new ScriptControlDescriptor("Microsoft.Reporting.WebFormsClient._CalendarDropDownParameterInputControl", this.ClientID);
      this.AddDropDownDescriptorProperties(desc);
      desc.AddProperty("BaseCalendarUrl", (object) this.ServerCalendarUrl);
      return (IEnumerable<ScriptDescriptor>) new ScriptDescriptor[1]
      {
        (ScriptDescriptor) desc
      };
    }
  }
}
