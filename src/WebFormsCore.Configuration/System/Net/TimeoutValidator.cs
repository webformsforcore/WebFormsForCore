// Decompiled with JetBrains decompiler
// Type: System.Net.TimeoutValidator
// Assembly: System, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 84F5A18A-F2B1-435C-B86E-09CE162E61E4
// Assembly location: C:\Windows\Microsoft.NET\assembly\GAC_MSIL\System\v4.0_4.0.0.0__b77a5c561934e089\System.dll
// XML documentation location: C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.8.1\System.xml

using System.Configuration;

#nullable disable
namespace System.Net
{
  internal sealed class TimeoutValidator : ConfigurationValidatorBase
  {
    private bool _zeroValid;

    internal TimeoutValidator(bool zeroValid) => this._zeroValid = zeroValid;

    public override bool CanValidate(Type type) => type == typeof (int) || type == typeof (long);

    public override void Validate(object value)
    {
      if (value == null)
        return;
      int num = (int) value;
      if ((!this._zeroValid || num != 0) && num <= 0 && num != -1)
        throw new ConfigurationErrorsException(SR.GetString("net_io_timeout_use_gt_zero"));
    }
  }
}
