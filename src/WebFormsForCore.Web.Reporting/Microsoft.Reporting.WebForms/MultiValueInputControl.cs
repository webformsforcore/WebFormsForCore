using System;
using System.Collections.Generic;
using System.Globalization;
using System.Web.UI;

namespace Microsoft.Reporting.WebForms;

internal abstract class MultiValueInputControl : GenericDropDownInputControl
{
	private MultiValueSelector m_floatingEditor;

	protected override bool CustomControlHasValue
	{
		get
		{
			EnsureChildControls();
			return m_floatingEditor.HasValue;
		}
	}

	protected override string[] CustomControlValue
	{
		get
		{
			EnsureChildControls();
			return m_floatingEditor.Value;
		}
		set
		{
			EnsureChildControls();
			m_floatingEditor.Value = value;
		}
	}

	protected override string FrameAccessibleName => Strings.PlaceHolderFrameAccessibleName(base.ReportParameter.Prompt);

	protected MultiValueInputControl(ReportParameterInfo reportParam, IBrowserDetection browserDetection, bool useAbsoluteScreenPositioning)
		: base(reportParam, browserDetection, useAbsoluteScreenPositioning)
	{
	}

	protected override void CreateChildControls()
	{
		Controls.Clear();
		base.CreateChildControls();
		base.InputControl.EnableViewState = false;
		base.InputControl.ReadOnly = true;
		base.Image.Src = EmbeddedResourceOperation.CreateUrl("Microsoft.Reporting.WebForms.Icons.MultiValueSelect.gif");
		base.Image.Style.Add(HtmlTextWriterStyle.VerticalAlign, "top");
		if (base.BrowserDetection.IsIE)
		{
			base.Image.Style.Add(HtmlTextWriterStyle.MarginTop, "1px");
		}
		m_floatingEditor = CreateFloatingEditor();
		AddFloatingAttributes(m_floatingEditor);
		m_floatingEditor.ID = "divDropDown";
		m_floatingEditor.Style.Add(HtmlTextWriterStyle.ZIndex, 11.ToString(CultureInfo.InvariantCulture));
		Controls.Add(m_floatingEditor);
		m_absolutePositionedControls.Add(m_floatingEditor);
	}

	protected override void OnPreRender(EventArgs e)
	{
		EnsureChildControls();
		base.OnPreRender(e);
		m_floatingEditor.ClientSideObjectName = base.ClientObject;
	}

	protected override void Render(HtmlTextWriter writer)
	{
		EnsureChildControls();
		m_floatingEditor.Font.CopyFrom(Font);
		base.Render(writer);
	}

	public override IEnumerable<ScriptDescriptor> GetScriptDescriptors()
	{
		EnsureChildControls();
		ScriptControlDescriptor scriptControlDescriptor = new ScriptControlDescriptor("Microsoft.Reporting.WebFormsClient._MultiValueParameterInputControl", ClientID);
		AddDropDownDescriptorProperties(scriptControlDescriptor);
		m_floatingEditor.AddScriptDescriptors(scriptControlDescriptor);
		scriptControlDescriptor.AddProperty("HasValidValueList", this is MultiValueValidValuesInputControl);
		scriptControlDescriptor.AddProperty("AllowBlank", base.ReportParameter.AllowBlank && base.ReportParameter.DataType == ParameterDataType.String);
		scriptControlDescriptor.AddProperty("FloatingEditorId", m_floatingEditor.ClientID);
		scriptControlDescriptor.AddProperty("ListSeparator", CultureInfo.CurrentCulture.TextInfo.ListSeparator + " ");
		scriptControlDescriptor.AddProperty("GripImage", EmbeddedResourceOperation.CreateUrl("Microsoft.Reporting.WebForms.Icons.HandleGrip.gif"));
		scriptControlDescriptor.AddProperty("GripImageRTL", EmbeddedResourceOperation.CreateUrl("Microsoft.Reporting.WebForms.Icons.HandleGripRTL.gif"));
		return new ScriptDescriptor[1] { scriptControlDescriptor };
	}

	protected abstract MultiValueSelector CreateFloatingEditor();
}
