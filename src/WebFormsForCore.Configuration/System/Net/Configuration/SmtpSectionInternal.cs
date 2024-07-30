
using System.Configuration;
using System.Net.Mail;
using System.Threading;

#nullable disable
namespace System.Net.Configuration
{
  internal sealed class SmtpSectionInternal
  {
    private SmtpDeliveryMethod deliveryMethod;
    private SmtpDeliveryFormat deliveryFormat;
    private string from;
    private SmtpNetworkElementInternal network;
    private SmtpSpecifiedPickupDirectoryElementInternal specifiedPickupDirectory;
    private static object classSyncObject;

    internal SmtpSectionInternal(SmtpSection section)
    {
      this.deliveryMethod = section.DeliveryMethod;
      this.deliveryFormat = section.DeliveryFormat;
      this.from = section.From;
      this.network = new SmtpNetworkElementInternal(section.Network);
      this.specifiedPickupDirectory = new SmtpSpecifiedPickupDirectoryElementInternal(section.SpecifiedPickupDirectory);
    }

    internal SmtpDeliveryMethod DeliveryMethod => this.deliveryMethod;

    internal SmtpDeliveryFormat DeliveryFormat => this.deliveryFormat;

    internal SmtpNetworkElementInternal Network => this.network;

    internal string From => this.from;

    internal SmtpSpecifiedPickupDirectoryElementInternal SpecifiedPickupDirectory
    {
      get => this.specifiedPickupDirectory;
    }

    internal static object ClassSyncObject
    {
      get
      {
        if (SmtpSectionInternal.classSyncObject == null)
          Interlocked.CompareExchange(ref SmtpSectionInternal.classSyncObject, new object(), (object) null);
        return SmtpSectionInternal.classSyncObject;
      }
    }

    internal static SmtpSectionInternal GetSection()
    {
      lock (SmtpSectionInternal.ClassSyncObject)
        return !(PrivilegedConfigurationManager.GetSection(ConfigurationStrings.SmtpSectionPath) is SmtpSection section) ? (SmtpSectionInternal) null : new SmtpSectionInternal(section);
    }
  }
}
