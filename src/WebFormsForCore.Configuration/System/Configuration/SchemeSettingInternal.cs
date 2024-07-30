
#nullable disable
namespace System.Configuration
{
  internal sealed class SchemeSettingInternal
  {
    private string name;
    private GenericUriParserOptions options;

    public SchemeSettingInternal(string name, GenericUriParserOptions options)
    {
      this.name = name.ToLowerInvariant();
      this.options = options;
    }

    public string Name => this.name;

    public GenericUriParserOptions Options => this.options;
  }
}
