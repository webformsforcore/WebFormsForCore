// Decompiled with JetBrains decompiler
// Type: System.Net.Configuration.SmtpSectionInternal
// Assembly: System, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 84F5A18A-F2B1-435C-B86E-09CE162E61E4
// Assembly location: C:\Windows\Microsoft.NET\assembly\GAC_MSIL\System\v4.0_4.0.0.0__b77a5c561934e089\System.dll
// XML documentation location: C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.8.1\System.xml

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
