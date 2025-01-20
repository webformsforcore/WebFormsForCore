
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

#nullable disable
namespace Microsoft.Reporting.WebForms
{
  internal abstract class GenericDropDownInputControl : 
    BaseParameterInputControl,
    IPostBackEventHandler
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

    protected GenericDropDownInputControl(
      ReportParameterInfo reportParam,
      IBrowserDetection browserDetection,
      bool useAbsoluteScreenPositioning)
      : base(reportParam, browserDetection)
    {
      this.m_useAbsoluteScreenPositioning = useAbsoluteScreenPositioning;
    }

    protected bool UseAbsoluteScreenPositioning => this.m_useAbsoluteScreenPositioning;

    protected string RelativeDivID
    {
      get
      {
        this.EnsureChildControls();
        if (this.Page == null)
          throw new Exception("ClientID accessed before control added to page.");
        return this.ClientID + "_RelativeDiv";
      }
    }

    protected abstract string FrameAccessibleName { get; }

    internal PostBackTextBox InputControl
    {
      get
      {
        this.EnsureChildControls();
        return this.m_inputControl;
      }
    }

    internal HtmlInputImage Image
    {
      get
      {
        this.EnsureChildControls();
        return (HtmlInputImage) this.m_ddButton;
      }
    }

    protected string DisabledImageLocation
    {
      get => this.m_disabledImage;
      set => this.m_disabledImage = value;
    }

    protected override string[] CustomControlIds
    {
      get
      {
        this.EnsureChildControls();
        return new string[2]
        {
          this.m_ddButton.ClientID,
          this.m_inputControl.ClientID
        };
      }
    }

    public override string PrimaryFormElementId
    {
      get
      {
        this.EnsureChildControls();
        return this.m_inputControl.ClientID;
      }
    }

    protected HtmlGenericControl Iframe
    {
      get
      {
        this.EnsureChildControls();
        return this.m_floatingIframe;
      }
    }

    protected bool AddSpaceBeforeImage
    {
      get => this.m_addSpaceBeforeImage;
      set => this.m_addSpaceBeforeImage = value;
    }

    public override string AltText
    {
      set => this.InputControl.ToolTip = value;
    }

    protected override void CreateChildControls()
    {
      this.Controls.Clear();
      base.CreateChildControls();
      this.m_inputControl = new PostBackTextBox();
      this.m_inputControl.ID = "txtValue";
      this.m_inputControl.Columns = 28;
      this.Controls.Add((Control) this.m_inputControl);
      this.m_ddButton = new HtmlInputImageWithClientID();
      this.m_ddButton.ID = "ddDropDownButton";
      this.m_ddButton.EnableViewState = false;
      this.m_ddButton.CausesValidation = false;
      this.m_ddButton.Alt = LocalizationHelper.Current.ParameterDropDownToolTip;
      this.m_ddButton.Attributes.Add("title", this.m_ddButton.Alt);
      this.Controls.Add((Control) this.m_ddButton);
      this.m_floatingIframe = new HtmlGenericControl("iframe");
      this.AddFloatingAttributes((HtmlControl) this.m_floatingIframe);
      this.m_floatingIframe.Style.Add(HtmlTextWriterStyle.ZIndex, 10.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      this.m_floatingIframe.Attributes.Add("src", "javascript:'';");
      this.m_floatingIframe.Attributes.Add("frameBorder", "0");
      string frameAccessibleName = this.FrameAccessibleName;
      this.m_floatingIframe.Attributes.Add("title", frameAccessibleName);
      this.m_floatingIframe.Attributes.Add("longdesc", frameAccessibleName);
      this.Controls.Add((Control) this.m_floatingIframe);
      this.m_absolutePositionedControls.Add((Control) this.m_floatingIframe);
    }

    protected override BaseValidator CreateParameterRequiresValueValidator()
    {
      return (BaseValidator) new ParameterInputRequiredValidator();
    }

    protected void AddFloatingAttributes(WebControl wc)
    {
      this.AddFloatingAttributes(wc.Attributes, wc.Style);
    }

    protected void AddFloatingAttributes(HtmlControl hc)
    {
      this.AddFloatingAttributes(hc.Attributes, hc.Style);
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
      return string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}.SelectImage({1});", (object) this.ClientObject, (object) boolParam);
    }

    protected override void OnPreRender(EventArgs e)
    {
      this.EnsureChildControls();
      if (string.IsNullOrEmpty(this.DisabledImageLocation))
        this.DisabledImageLocation = this.Image.Src;
      this.Iframe.Attributes[HtmlTextWriterAttribute.Name.ToString()] = this.Iframe.ClientID;
      base.OnPreRender(e);
      if (!this.Disabled && !this.DDButtonDisabled)
        this.m_ddButton.Style.Add(HtmlTextWriterStyle.Cursor, "pointer");
      else
        this.m_ddButton.Style.Add(HtmlTextWriterStyle.Cursor, "default");
      if (this.ParameterRequiresValueValidator == null)
        return;
      this.ParameterRequiresValueValidator.ControlToValidate = this.InputControl.ID;
    }

    public override bool AutoPostBack
    {
      set
      {
        this.InputControl.AutoPostBack = value;
        base.AutoPostBack = value;
      }
    }

    public void RaisePostBackEvent(string eventArgument)
    {
      this.OnBubbleEvent((object) this, (EventArgs) new AutoPostBackEventArgs());
    }

    public string DropDownClientSideObjectName
    {
      get
      {
        return string.Format((IFormatProvider) CultureInfo.InvariantCulture, "$get('{0}').ClientObject", (object) JavaScriptHelper.StringEscapeSingleQuote(this.ClientID));
      }
    }

    protected override void RenderContents(HtmlTextWriter writer)
    {
      this.EnsureChildControls();
      this.ApplyStylesToTextBox((TextBox) this.m_inputControl);
      if (this.Disabled || this.DDButtonDisabled)
      {
        this.m_ddButton.Src = this.DisabledImageLocation;
        this.m_ddButton.Disabled = true;
      }
      writer.AddStyleAttribute(HtmlTextWriterStyle.WhiteSpace, "nowrap");
      writer.AddAttribute("onactivate", "event.cancelBubble=true;");
      writer.RenderBeginTag(HtmlTextWriterTag.Div);
      this.m_inputControl.RenderControl(writer);
      if (this.AddSpaceBeforeImage)
        writer.Write("&nbsp;");
      this.m_ddButton.RenderControl(writer);
      if (this.m_nullCheckBox != null)
      {
        writer.Write("&nbsp;");
        this.RenderNullCheckBox(writer);
      }
      writer.RenderEndTag();
      if (!this.UseAbsoluteScreenPositioning)
      {
        writer.AddStyleAttribute(HtmlTextWriterStyle.Position, "relative");
        writer.AddAttribute(HtmlTextWriterAttribute.Id, this.RelativeDivID);
        writer.RenderBeginTag(HtmlTextWriterTag.Div);
        this.RenderAbsolutePositionedControls(writer);
        writer.RenderEndTag();
      }
      if (!this.Validators.HasValidatorsToRender)
        return;
      this.Validators.RenderControl(writer);
    }

    public void RenderAbsolutePositionedControls(HtmlTextWriter writer)
    {
      foreach (Control positionedControl in this.m_absolutePositionedControls)
        positionedControl.RenderControl(writer);
    }

    protected void AddDropDownDescriptorProperties(ScriptControlDescriptor desc)
    {
      this.EnsureChildControls();
      this.AddBaseDescriptorProperties(desc);
      desc.AddProperty("EnabledImageSrc", (object) this.Image.Src);
      desc.AddProperty("DisabledImageSrc", (object) this.DisabledImageLocation);
      desc.AddProperty("ImageId", (object) this.m_ddButton.ClientID);
      desc.AddProperty("TextBoxId", (object) this.m_inputControl.ClientID);
      desc.AddProperty("FloatingIframeId", (object) this.m_floatingIframe.ClientID);
      desc.AddProperty("RelativeDivId", this.UseAbsoluteScreenPositioning ? (object) (string) null : (object) this.RelativeDivID);
    }

    protected override void SetCustomControlEnableState(bool enabled)
    {
      this.m_inputControl.Enabled = enabled;
      this.m_ddButton.Disabled = !enabled;
    }

    private bool DDButtonDisabled => this.m_nullCheckBox != null && this.m_nullCheckBox.Checked;
  }
}
