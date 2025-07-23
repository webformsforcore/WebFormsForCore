using System;
using System.Collections.Generic;
using System.Globalization;
using System.Web;
using System.Web.UI;
using Microsoft.ReportingServices.Common;

namespace Microsoft.Reporting.WebForms;

internal class CalendarDropDownInputControl : GenericDropDownInputControl
{
	private string ServerCalendarUrl => CalendarIframeOperation.CreateUrl() + "&selectDate=";

	protected override string[] CustomControlValue
	{
		get
		{
			EnsureChildControls();
			return new string[1] { base.InputControl.Text };
		}
		set
		{
			if (value == null || value.Length != 1)
			{
				base.InputControl.Text = "";
				return;
			}
			string text = value[0];
			DateTimeOffset dateTimeOffset = default(DateTimeOffset);
			bool flag = default(bool);
			if (DateTimeUtil.TryParseDateTime(text, (CultureInfo)null, ref dateTimeOffset, ref flag))
			{
				if (flag)
				{
					base.InputControl.Text = dateTimeOffset.ToString();
				}
				else if (dateTimeOffset.TimeOfDay == CalendarPageControl._Midnight)
				{
					base.InputControl.Text = dateTimeOffset.Date.ToShortDateString();
				}
				else
				{
					base.InputControl.Text = dateTimeOffset.DateTime.ToString();
				}
			}
			else
			{
				base.InputControl.Text = text;
			}
		}
	}

	protected override bool CustomControlHasValue => base.InputControl.Text.Length > 0;

	protected override string FrameAccessibleName => Strings.CalendarFrameAccessibleName(base.ReportParameter.Prompt);

	public CalendarDropDownInputControl(ReportParameterInfo reportParam, IBrowserDetection browserDetection, bool useAbsoluteScreenPositioning)
		: base(reportParam, browserDetection, useAbsoluteScreenPositioning)
	{
		base.AddSpaceBeforeImage = true;
	}

	public static bool IsSupported(IBrowserDetection browserDetection)
	{
		if (!browserDetection.IsSafari)
		{
			return CultureInfo.CurrentCulture.Calendar is GregorianCalendar;
		}
		return false;
	}

	protected override void CreateChildControls()
	{
		Controls.Clear();
		base.CreateChildControls();
		base.InputControl.TextChanged += base.OnCustomControlChanged;
		base.Image.Src = EmbeddedResourceOperation.CreateUrl("Microsoft.Reporting.WebForms.calendar.gif");
		base.DisabledImageLocation = EmbeddedResourceOperation.CreateUrl("Microsoft.Reporting.WebForms.calendar_disabled.gif");
		base.Iframe.Attributes.Add("scrolling", "no");
		if (base.ReportParameter.Nullable)
		{
			CreateNullCheckBox();
		}
	}

	protected override void OnPreRender(EventArgs e)
	{
		EnsureChildControls();
		string value = ServerCalendarUrl + HttpUtility.UrlEncode(base.InputControl.Text);
		base.Iframe.Attributes.Add("src", value);
		string format = "\r\nthis.resultfield=$get('{0}');\r\nthis.resultfunc=function(resultField){{{1}.OnCalendarSelection(resultField);}}";
		string text = string.Format(CultureInfo.InvariantCulture, format, JavaScriptHelper.StringEscapeSingleQuote(base.InputControl.ClientID), base.ClientObject);
		if (base.Iframe.Attributes["onload"] == null)
		{
			base.Iframe.Attributes.Add("onload", text);
		}
		else
		{
			base.Iframe.Attributes["onload"] += text;
		}
		base.OnPreRender(e);
	}

	public override IEnumerable<ScriptDescriptor> GetScriptDescriptors()
	{
		ScriptControlDescriptor scriptControlDescriptor = new ScriptControlDescriptor("Microsoft.Reporting.WebFormsClient._CalendarDropDownParameterInputControl", ClientID);
		AddDropDownDescriptorProperties(scriptControlDescriptor);
		scriptControlDescriptor.AddProperty("BaseCalendarUrl", ServerCalendarUrl);
		return new ScriptDescriptor[1] { scriptControlDescriptor };
	}
}
