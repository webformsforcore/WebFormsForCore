
#nullable disable
namespace System.Net.Configuration
{
  internal sealed class SmtpSpecifiedPickupDirectoryElementInternal
  {
    private string pickupDirectoryLocation;

    internal SmtpSpecifiedPickupDirectoryElementInternal(SmtpSpecifiedPickupDirectoryElement element)
    {
      this.pickupDirectoryLocation = element.PickupDirectoryLocation;
    }

    internal string PickupDirectoryLocation => this.pickupDirectoryLocation;
  }
}
