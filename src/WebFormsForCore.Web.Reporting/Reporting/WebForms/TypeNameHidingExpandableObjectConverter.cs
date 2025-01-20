
using System;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.InteropServices;

#nullable disable
namespace Microsoft.Reporting.WebForms
{
  [ComVisible(false)]
  internal sealed class TypeNameHidingExpandableObjectConverter : ExpandableObjectConverter
  {
    public override object ConvertTo(
      ITypeDescriptorContext context,
      CultureInfo culture,
      object value,
      Type destinationType)
    {
      return destinationType == typeof (string) ? (object) string.Empty : base.ConvertTo(context, culture, value, destinationType);
    }
  }
}
