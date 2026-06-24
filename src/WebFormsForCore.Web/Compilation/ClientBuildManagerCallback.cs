//------------------------------------------------------------------------------
// <copyright file="ClientBuildManagerCallback.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>                                                                
//------------------------------------------------------------------------------

/************************************************************************************************************/


namespace System.Web.Compilation;

using System;
using System.Security.Permissions;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Web.UI;


//
// This is a callback class implemented by ClientBuildManager callers. It is used
// to receive status information about the build.
//
[PermissionSet(SecurityAction.LinkDemand, Unrestricted = true)]
[PermissionSet(SecurityAction.InheritanceDemand, Unrestricted = true)]
public class ClientBuildManagerCallback : MarshalByRefObject {

    // This includes both errors and warnings
    public Action<string, int, int, string, string> OnCompilerError = null;
    public virtual void ReportCompilerError(CompilerError error) { OnCompilerError?.Invoke(error.FileName, error.Line, error.Column, error.ErrorNumber, error.ErrorText); }

    public Action<string, int, string> OnParseError = null;
    public virtual void ReportParseError(ParserError error) { OnParseError?.Invoke(error.VirtualPath, error.Line, error.ErrorText); }

    public Action<string> OnProgress = null;
    public virtual void ReportProgress(string message) { OnProgress?.Invoke(message); }

    // DevDiv 180798. The default lease is 5 minutes, so we return null to allow compilation
    // calls to exceed 5 minutes. In doing so, we need to call RemotingService.Disconnect 
    // to release the object.
    public override object InitializeLifetimeService() {
        return null;
    }
}
