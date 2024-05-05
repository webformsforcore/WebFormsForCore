// Decompiled with JetBrains decompiler
// Type: System.Net.ExceptionHelper
// Assembly: System, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 84F5A18A-F2B1-435C-B86E-09CE162E61E4
// Assembly location: C:\Windows\Microsoft.NET\assembly\GAC_MSIL\System\v4.0_4.0.0.0__b77a5c561934e089\System.dll
// XML documentation location: C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.8.1\System.xml

using System.Security.Permissions;
using System.Configuration;
#nullable disable
namespace System.Net
{
  internal static class ExceptionHelper
  {
    internal static readonly KeyContainerPermission KeyContainerPermissionOpen = new KeyContainerPermission(KeyContainerPermissionFlags.Open);
    internal static readonly WebPermission WebPermissionUnrestricted = new WebPermission(PermissionState.Unrestricted);
    internal static readonly SecurityPermission UnmanagedPermission = new SecurityPermission(SecurityPermissionFlag.UnmanagedCode);
    internal static readonly SocketPermission UnrestrictedSocketPermission = new SocketPermission(PermissionState.Unrestricted);
    internal static readonly SecurityPermission InfrastructurePermission = new SecurityPermission(SecurityPermissionFlag.Infrastructure);
    internal static readonly SecurityPermission ControlPolicyPermission = new SecurityPermission(SecurityPermissionFlag.ControlPolicy);
    internal static readonly SecurityPermission ControlPrincipalPermission = new SecurityPermission(SecurityPermissionFlag.ControlPrincipal);

    internal static NotImplementedException MethodNotImplementedException
    {
      get => new NotImplementedException(SR.GetString("net_MethodNotImplementedException"));
    }

    internal static NotImplementedException PropertyNotImplementedException
    {
      get => new NotImplementedException(SR.GetString("net_PropertyNotImplementedException"));
    }

    internal static NotSupportedException MethodNotSupportedException
    {
      get => new NotSupportedException(SR.GetString("net_MethodNotSupportedException"));
    }

    internal static NotSupportedException PropertyNotSupportedException
    {
      get => new NotSupportedException(SR.GetString("net_PropertyNotSupportedException"));
    }

    internal static WebException IsolatedException
    {
      get
      {
        return new WebException(SR.GetString(SR.net_requestaborted), WebExceptionStatus.KeepAliveFailure);
      }
    }

    internal static WebException RequestAbortedException
    {
      get
      {
        return new WebException(SR.GetString(SR.net_requestaborted), WebExceptionStatus.RequestCanceled);
      }
    }

    internal static WebException CacheEntryNotFoundException
    {
      get
      {
        return new WebException(SR.GetString(SR.net_requestaborted), WebExceptionStatus.CacheEntryNotFound);
      }
    }

    internal static WebException RequestProhibitedByCachePolicyException
    {
      get
      {
        return new WebException(SR.GetString(SR.net_requestaborted), WebExceptionStatus.RequestProhibitedByCachePolicy);
      }
    }
  }
}
