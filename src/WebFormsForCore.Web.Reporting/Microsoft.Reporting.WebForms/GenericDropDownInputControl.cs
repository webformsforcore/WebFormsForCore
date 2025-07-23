using System;
using System.Collections.Generic;
using System.Globalization;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

namespace Microsoft.Reporting.WebForms;

internal abstract class GenericDropDownInputControl : BaseParameterInputControl, IPostBackEventHandler
{
	internal const int _MultiTextBoxColumns = 28;

	protected const int IFrameZIndex = 10;

	private bool m_useAbsoluteScreenPositioning = true;

	private PostBackTextBox m_inputControl;

	private HtmlInputImageWithClientID m_ddButton;

	private string m_disabledImage;

	private HtmlGenericControl m_floatingIframe;

	private bool m_addSpaceBeforeImage;

	protected List<Control> m_absolutePositionedControls = new List<Control>(2);

	protected bool UseAbsoluteScreenPositioning => m_useAbsoluteScreenPositioning;

	protected string RelativeDivID
	{
		get
		{
			EnsureChildControls();
			if (Page == null)
			{
				throw new Exception("ClientID accessed before control added to page.");
			}
			return ClientID + "_RelativeDiv";
		}
	}

	protected abstract string FrameAccessibleName { get; }

	internal PostBackTextBox InputControl
	{
		get
		{
			EnsureChildControls();
			return m_inputControl;
		}
	}

	internal HtmlInputImage Image
	{
		get
		{
			EnsureChildControls();
			return m_ddButton;
		}
	}

	protected string DisabledImageLocation
	{
		get
		{
			return m_disabledImage;
		}
		set
		{
			m_disabledImage = value;
		}
	}

	protected override string[] CustomControlIds
	{
		get
		{
			EnsureChildControls();
			return new string[2] { m_ddButton.ClientID, m_inputControl.ClientID };
		}
	}

	public override string PrimaryFormElementId
	{
		get
		{
			EnsureChildControls();
			return m_inputControl.ClientID;
		}
	}

	protected HtmlGenericControl Iframe
	{
		get
		{
			EnsureChildControls();
			return m_floatingIframe;
		}
	}

	protected bool AddSpaceBeforeImage
	{
		get
		{
			return m_addSpaceBeforeImage;
		}
		set
		{
			m_addSpaceBeforeImage = value;
		}
	}

	public override string AltText
	{
		set
		{
			InputControl.ToolTip = value;
		}
	}

	public override bool AutoPostBack
	{
		set
		{
			InputControl.AutoPostBack = value;
			base.AutoPostBack = value;
		}
	}

	public string DropDownClientSideObjectName => string.Format(CultureInfo.InvariantCulture, "$get('{0}').ClientObject", JavaScriptHelper.StringEscapeSingleQuote(ClientID));

	private bool DDButtonDisabled
	{
		get
		{
			if (m_nullCheckBox != null)
			{
				return m_nullCheckBox.Checked;
			}
			return false;
		}
	}

	protected GenericDropDownInputControl(ReportParameterInfo reportParam, IBrowserDetection browserDetection, bool useAbsoluteScreenPositioning)
		: base(reportParam, browserDetection)
	{
		m_useAbsoluteScreenPositioning = useAbsoluteScreenPositioning;
	}

	protected override void CreateChildControls()
	{
		Controls.Clear();
		base.CreateChildControls();
		m_inputControl = new PostBackTextBox();
		m_inputControl.ID = "txtValue";
		m_inputControl.Columns = 28;
		Controls.Add(m_inputControl);
		m_ddButton = new HtmlInputImageWithClientID();
		m_ddButton.ID = "ddDropDownButton";
		m_ddButton.EnableViewState = false;
		m_ddButton.CausesValidation = false;
		m_ddButton.Alt = LocalizationHelper.Current.ParameterDropDownToolTip;
		m_ddButton.Attributes.Add("title", m_ddButton.Alt);
		Controls.Add(m_ddButton);
		m_floatingIframe = new HtmlGenericControl("iframe");
		AddFloatingAttributes(m_floatingIframe);
		m_floatingIframe.Style.Add(HtmlTextWriterStyle.ZIndex, 10.ToString(CultureInfo.InvariantCulture));
		m_floatingIframe.Attributes.Add("src", "javascript:'';");
		m_floatingIframe.Attributes.Add("frameBorder", "0");
		string frameAccessibleName = FrameAccessibleName;
		m_floatingIframe.Attributes.Add("title", frameAccessibleName);
		m_floatingIframe.Attributes.Add("longdesc", frameAccessibleName);
		Controls.Add(m_floatingIframe);
		m_absolutePositionedControls.Add(m_floatingIframe);
	}

	protected override BaseValidator CreateParameterRequiresValueValidator()
	{
		return new ParameterInputRequiredValidator();
	}

	protected void AddFloatingAttributes(WebControl wc)
	{
		AddFloatingAttributes(wc.Attributes, wc.Style);
	}

	protected void AddFloatingAttributes(HtmlControl hc)
	{
		AddFloatingAttributes(hc.Attributes, hc.Style);
	}

	private void AddFloatingAttributes(AttributeCollection attributes, CssStyleCollection style)
	{
		attributes.Add("onclick", "event.cancelBubble=true;");
		attributes.Add("onactivate", "event.cancelBubble=true;");
		style.Add(HtmlTextWriterStyle.Display, "none");
		style.Add(HtmlTextWriterStyle.Position, "absolute");
	}

	public string ToggleImageScript(string boolParam)
	{
		return string.Format(CultureInfo.InvariantCulture, "{0}.SelectImage({1});", base.ClientObject, boolParam);
	}

	protected override void OnPreRender(EventArgs e)
	{
		EnsureChildControls();
		if (string.IsNullOrEmpty(DisabledImageLocation))
		{
			DisabledImageLocation = Image.Src;
		}
		Iframe.Attributes[HtmlTextWriterAttribute.Name.ToString()] = Iframe.ClientID;
		base.OnPreRender(e);
		if (!base.Disabled && !DDButtonDisabled)
		{
			m_ddButton.Style.Add(HtmlTextWriterStyle.Cursor, "pointer");
		}
		else
		{
			m_ddButton.Style.Add(HtmlTextWriterStyle.Cursor, "default");
		}
		if (base.ParameterRequiresValueValidator != null)
		{
			base.ParameterRequiresValueValidator.ControlToValidate = InputControl.ID;
		}
	}

	public void RaisePostBackEvent(string eventArgument)
	{
		OnBubbleEvent(this, new AutoPostBackEventArgs());
	}

	protected override void RenderContents(HtmlTextWriter writer)
	{
		EnsureChildControls();
		ApplyStylesToTextBox(m_inputControl);
		if (base.Disabled || DDButtonDisabled)
		{
			m_ddButton.Src = DisabledImageLocation;
			m_ddButton.Disabled = true;
		}
		writer.AddStyleAttribute(HtmlTextWriterStyle.WhiteSpace, "nowrap");
		writer.AddAttribute("onactivate", "event.cancelBubble=true;");
		writer.RenderBeginTag(HtmlTextWriterTag.Div);
		m_inputControl.RenderControl(writer);
		if (AddSpaceBeforeImage)
		{
			writer.Write("&nbsp;");
		}
		m_ddButton.RenderControl(writer);
		if (m_nullCheckBox != null)
		{
			writer.Write("&nbsp;");
			RenderNullCheckBox(writer);
		}
		writer.RenderEndTag();
		if (!UseAbsoluteScreenPositioning)
		{
			writer.AddStyleAttribute(HtmlTextWriterStyle.Position, "relative");
			writer.AddAttribute(HtmlTextWriterAttribute.Id, RelativeDivID);
			writer.RenderBeginTag(HtmlTextWriterTag.Div);
			RenderAbsolutePositionedControls(writer);
			writer.RenderEndTag();
		}
		if (base.Validators.HasValidatorsToRender)
		{
			base.Validators.RenderControl(writer);
		}
	}

	public void RenderAbsolutePositionedControls(HtmlTextWriter writer)
	{
		foreach (Control absolutePositionedControl in m_absolutePositionedControls)
		{
			absolutePositionedControl.RenderControl(writer);
		}
	}

	protected void AddDropDownDescriptorProperties(ScriptControlDescriptor desc)
	{
		EnsureChildControls();
		AddBaseDescriptorProperties(desc);
		desc.AddProperty("EnabledImageSrc", Image.Src);
		desc.AddProperty("DisabledImageSrc", DisabledImageLocation);
		desc.AddProperty("ImageId", m_ddButton.ClientID);
		desc.AddProperty("TextBoxId", m_inputControl.ClientID);
		desc.AddProperty("FloatingIframeId", m_floatingIframe.ClientID);
		desc.AddProperty("RelativeDivId", UseAbsoluteScreenPositioning ? null : RelativeDivID);
	}

	protected override void SetCustomControlEnableState(bool enabled)
	{
		m_inputControl.Enabled = enabled;
		m_ddButton.Disabled = !enabled;
	}
}
