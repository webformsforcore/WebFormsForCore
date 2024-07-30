
using System.Collections.Generic;

#nullable disable
namespace System.Configuration
{
  internal sealed class UriSectionData
  {
    private UriIdnScope? idnScope;
    private bool? iriParsing;
    private Dictionary<string, SchemeSettingInternal> schemeSettings;

    public UriSectionData()
    {
      this.schemeSettings = new Dictionary<string, SchemeSettingInternal>();
    }

    public UriIdnScope? IdnScope
    {
      get => this.idnScope;
      set => this.idnScope = value;
    }

    public bool? IriParsing
    {
      get => this.iriParsing;
      set => this.iriParsing = value;
    }

    public Dictionary<string, SchemeSettingInternal> SchemeSettings => this.schemeSettings;
  }
}
