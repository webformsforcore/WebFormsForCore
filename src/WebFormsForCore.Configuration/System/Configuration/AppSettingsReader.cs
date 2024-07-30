using System.Collections.Specialized;
using System.Globalization;

#nullable disable
namespace System.Configuration
{
  /// <summary>Provides a method for reading values of a particular type from the configuration.</summary>
  public class AppSettingsReader
  {
    private NameValueCollection map;
    private static Type stringType = typeof (string);
    private static Type[] paramsArray = new Type[1]
    {
      AppSettingsReader.stringType
    };
    private static string NullString = "None";

    /// <summary>Initializes a new instance of the <see cref="T:System.Configuration.AppSettingsReader" /> class.</summary>
    public AppSettingsReader() => this.map = ConfigurationManager.AppSettings;

    /// <summary>Gets the value for a specified key from the <see cref="P:System.Configuration.ConfigurationSettings.AppSettings" /> property and returns an object of the specified type containing the value from the configuration.</summary>
    /// <param name="key">The key for which to get the value.</param>
    /// <param name="type">The type of the object to return.</param>
    /// <returns>The value of the specified key.</returns>
    /// <exception cref="T:System.ArgumentNullException">
    ///         <paramref name="key" /> is <see langword="null" />.
    /// -or-
    ///  <paramref name="type" /> is <see langword="null" />.</exception>
    /// <exception cref="T:System.InvalidOperationException">
    ///         <paramref name="key" /> does not exist in the <see langword="&lt;appSettings&gt;" /> configuration section.
    /// -or-
    ///  The value in the <see langword="&lt;appSettings&gt;" /> configuration section for <paramref name="key" /> is not of type <paramref name="type" />.</exception>
    public object GetValue(string key, Type type)
    {
      if (key == null)
        throw new ArgumentNullException(nameof (key));
      if (type == (Type) null)
        throw new ArgumentNullException(nameof (type));
      string val = this.map[key];
      if (val == null)
        throw new InvalidOperationException(SR.GetString("AppSettingsReaderNoKey", (object) key));
      if (type == AppSettingsReader.stringType)
      {
        switch (this.GetNoneNesting(val))
        {
          case 0:
            return (object) val;
          case 1:
            return (object) null;
          default:
            return (object) val.Substring(1, val.Length - 2);
        }
      }
      else
      {
        try
        {
          return Convert.ChangeType((object) val, type, (IFormatProvider) CultureInfo.InvariantCulture);
        }
        catch (Exception ex)
        {
          throw new InvalidOperationException(SR.GetString("AppSettingsReaderCantParse", (object) (val.Length == 0 ? "AppSettingsReaderEmptyString" : val), (object) key, (object) type.ToString()));
        }
      }
    }

    private int GetNoneNesting(string val)
    {
      int noneNesting = 0;
      int length = val.Length;
      if (length > 1)
      {
        while (val[noneNesting] == '(' && val[length - noneNesting - 1] == ')')
          ++noneNesting;
        if (noneNesting > 0 && string.Compare(AppSettingsReader.NullString, 0, val, noneNesting, length - 2 * noneNesting, StringComparison.Ordinal) != 0)
          noneNesting = 0;
      }
      return noneNesting;
    }
  }
}
