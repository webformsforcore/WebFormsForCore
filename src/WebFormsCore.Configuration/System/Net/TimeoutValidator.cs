
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
