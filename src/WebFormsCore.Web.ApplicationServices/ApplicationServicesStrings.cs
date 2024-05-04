// Decompiled with JetBrains decompiler
// Type: System.Web.ApplicationServicesStrings
// Assembly: System.Web.ApplicationServices, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 49FC561C-A827-422E-A5C7-EDE4066C7817
// Assembly location: C:\Windows\Microsoft.NET\assembly\GAC_MSIL\System.Web.ApplicationServices\v4.0_4.0.0.0__31bf3856ad364e35\System.Web.ApplicationServices.dll
// XML documentation location: C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.8.1\System.Web.ApplicationServices.xml

using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;

#nullable disable
namespace System.Web
{
  [GeneratedCode("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
  [DebuggerNonUserCode]
  [CompilerGenerated]
  internal class ApplicationServicesStrings
  {
    private static ResourceManager resourceMan;
    private static CultureInfo resourceCulture;

    internal ApplicationServicesStrings()
    {
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    internal static ResourceManager ResourceManager
    {
      get
      {
        if (ApplicationServicesStrings.resourceMan == null)
          ApplicationServicesStrings.resourceMan = new ResourceManager("System.Web.ApplicationServicesStrings", typeof (ApplicationServicesStrings).Assembly);
        return ApplicationServicesStrings.resourceMan;
      }
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    internal static CultureInfo Culture
    {
      get => ApplicationServicesStrings.resourceCulture;
      set => ApplicationServicesStrings.resourceCulture = value;
    }

    internal static string Can_not_use_encrypted_passwords_with_autogen_keys
    {
      get
      {
        return ApplicationServicesStrings.ResourceManager.GetString(nameof (Can_not_use_encrypted_passwords_with_autogen_keys), ApplicationServicesStrings.resourceCulture);
      }
    }

    internal static string CustomLoader_ForbiddenByHost
    {
      get
      {
        return ApplicationServicesStrings.ResourceManager.GetString(nameof (CustomLoader_ForbiddenByHost), ApplicationServicesStrings.resourceCulture);
      }
    }

    internal static string CustomLoader_MustImplementICustomLoader
    {
      get
      {
        return ApplicationServicesStrings.ResourceManager.GetString(nameof (CustomLoader_MustImplementICustomLoader), ApplicationServicesStrings.resourceCulture);
      }
    }

    internal static string CustomLoader_NoAttributeFound
    {
      get
      {
        return ApplicationServicesStrings.ResourceManager.GetString(nameof (CustomLoader_NoAttributeFound), ApplicationServicesStrings.resourceCulture);
      }
    }

    internal static string CustomLoader_NotInFullTrust
    {
      get
      {
        return ApplicationServicesStrings.ResourceManager.GetString(nameof (CustomLoader_NotInFullTrust), ApplicationServicesStrings.resourceCulture);
      }
    }

    internal static string Membership_DuplicateEmail
    {
      get
      {
        return ApplicationServicesStrings.ResourceManager.GetString(nameof (Membership_DuplicateEmail), ApplicationServicesStrings.resourceCulture);
      }
    }

    internal static string Membership_DuplicateProviderUserKey
    {
      get
      {
        return ApplicationServicesStrings.ResourceManager.GetString(nameof (Membership_DuplicateProviderUserKey), ApplicationServicesStrings.resourceCulture);
      }
    }

    internal static string Membership_DuplicateUserName
    {
      get
      {
        return ApplicationServicesStrings.ResourceManager.GetString(nameof (Membership_DuplicateUserName), ApplicationServicesStrings.resourceCulture);
      }
    }

    internal static string Membership_InvalidAnswer
    {
      get
      {
        return ApplicationServicesStrings.ResourceManager.GetString(nameof (Membership_InvalidAnswer), ApplicationServicesStrings.resourceCulture);
      }
    }

    internal static string Membership_InvalidEmail
    {
      get
      {
        return ApplicationServicesStrings.ResourceManager.GetString(nameof (Membership_InvalidEmail), ApplicationServicesStrings.resourceCulture);
      }
    }

    internal static string Membership_InvalidPassword
    {
      get
      {
        return ApplicationServicesStrings.ResourceManager.GetString(nameof (Membership_InvalidPassword), ApplicationServicesStrings.resourceCulture);
      }
    }

    internal static string Membership_InvalidProviderUserKey
    {
      get
      {
        return ApplicationServicesStrings.ResourceManager.GetString(nameof (Membership_InvalidProviderUserKey), ApplicationServicesStrings.resourceCulture);
      }
    }

    internal static string Membership_InvalidQuestion
    {
      get
      {
        return ApplicationServicesStrings.ResourceManager.GetString(nameof (Membership_InvalidQuestion), ApplicationServicesStrings.resourceCulture);
      }
    }

    internal static string Membership_InvalidUserName
    {
      get
      {
        return ApplicationServicesStrings.ResourceManager.GetString(nameof (Membership_InvalidUserName), ApplicationServicesStrings.resourceCulture);
      }
    }

    internal static string Membership_no_error
    {
      get
      {
        return ApplicationServicesStrings.ResourceManager.GetString(nameof (Membership_no_error), ApplicationServicesStrings.resourceCulture);
      }
    }

    internal static string Membership_provider_name_invalid
    {
      get
      {
        return ApplicationServicesStrings.ResourceManager.GetString(nameof (Membership_provider_name_invalid), ApplicationServicesStrings.resourceCulture);
      }
    }

    internal static string Membership_UserRejected
    {
      get
      {
        return ApplicationServicesStrings.ResourceManager.GetString(nameof (Membership_UserRejected), ApplicationServicesStrings.resourceCulture);
      }
    }

    internal static string Parameter_can_not_be_empty
    {
      get
      {
        return ApplicationServicesStrings.ResourceManager.GetString(nameof (Parameter_can_not_be_empty), ApplicationServicesStrings.resourceCulture);
      }
    }

    internal static string Platform_not_supported
    {
      get
      {
        return ApplicationServicesStrings.ResourceManager.GetString(nameof (Platform_not_supported), ApplicationServicesStrings.resourceCulture);
      }
    }

    internal static string Provider_Error
    {
      get
      {
        return ApplicationServicesStrings.ResourceManager.GetString(nameof (Provider_Error), ApplicationServicesStrings.resourceCulture);
      }
    }

    internal static string Provider_must_implement_type
    {
      get
      {
        return ApplicationServicesStrings.ResourceManager.GetString(nameof (Provider_must_implement_type), ApplicationServicesStrings.resourceCulture);
      }
    }
  }
}
