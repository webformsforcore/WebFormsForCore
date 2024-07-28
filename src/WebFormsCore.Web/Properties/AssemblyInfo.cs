using System;
using System.Reflection;
using System.Resources;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;
using System.Web.Security;

[assembly: TypeForwardedTo(typeof(RoleProvider))]
[assembly: TypeForwardedTo(typeof(MembershipPasswordException))]
[assembly: TypeForwardedTo(typeof(MembershipValidatePasswordEventHandler))]
[assembly: TypeForwardedTo(typeof(ValidatePasswordEventArgs))]
[assembly: TypeForwardedTo(typeof(MembershipCreateStatus))]
[assembly: TypeForwardedTo(typeof(MembershipCreateUserException))]
[assembly: TypeForwardedTo(typeof(MembershipPasswordFormat))]
[assembly: TypeForwardedTo(typeof(MembershipProvider))]
[assembly: TypeForwardedTo(typeof(MembershipProviderCollection))]
[assembly: TypeForwardedTo(typeof(MembershipUser))]
[assembly: TypeForwardedTo(typeof(MembershipUserCollection))]

[assembly: InternalsVisibleTo("System.Design, PublicKey=002400000480000094000000060200000024000052534131000400000100010007d1fa57c4aed9f0a32e84aa0faefd0de9e8fd6aec8f87fb03766c834c99921eb23be79ad9d5dcc1dd9ad236132102900b723cf980957fc4e177108fc607774f29e8320e92ea05ece4e821c0a5efe8f1645c4c0c93c1ab99285d622caa652c1dfad63d745d6f2de5f17e5eaf0fc4963d261c8a12436518206dc093344d5ad293")]
[assembly: InternalsVisibleTo("System.Web.Extensions, PublicKey=0024000004800000940000000602000000240000525341310004000001000100b5fc90e7027f67871e773a8fde8938c81dd402ba65b9201d60593e96c492651e889cc13f1415ebb53fac1131ae0bd333c5ee6021672d9718ea31a8aebd0da0072f25d87dba6fc90ffd598ed4da35e44c398c454307e8e33b8426143daec9f596836f97c8f74750e5975c64e2189f45def46b2a2b1247adc3652bf5c308055da9")]
[assembly: InternalsVisibleTo("System.Web.Extensions, PublicKey=00240000048000009400000006020000002400005253413100040000010001003d8a5d984adcdd2aa737bf3ed437819f4e7c988cb40a4dc2cf3daeb1aef98355808f626e611754c29f8647123d00e6d52797da1dd16721d906c3d09d0425e8983ea7e265a579f13c3b22b7505632eff882b7c345dfa08f6cf0f86a67896398cc2066dc260e92203f4a8bd66ab336fb6d82b88893e1e588f126a7ebbff4057cca")]
[assembly: InternalsVisibleTo("System.Web.Extensions.Test, PublicKey=002400000480000094000000060200000024000052534131000400000100010007d1fa57c4aed9f0a32e84aa0faefd0de9e8fd6aec8f87fb03766c834c99921eb23be79ad9d5dcc1dd9ad236132102900b723cf980957fc4e177108fc607774f29e8320e92ea05ece4e821c0a5efe8f1645c4c0c93c1ab99285d622caa652c1dfad63d745d6f2de5f17e5eaf0fc4963d261c8a12436518206dc093344d5ad293")]
[assembly: InternalsVisibleTo("System.Web.Test, PublicKey=002400000480000094000000060200000024000052534131000400000100010007d1fa57c4aed9f0a32e84aa0faefd0de9e8fd6aec8f87fb03766c834c99921eb23be79ad9d5dcc1dd9ad236132102900b723cf980957fc4e177108fc607774f29e8320e92ea05ece4e821c0a5efe8f1645c4c0c93c1ab99285d622caa652c1dfad63d745d6f2de5f17e5eaf0fc4963d261c8a12436518206dc093344d5ad293")]
[assembly: InternalsVisibleTo("System.Web.Routing.Test, PublicKey=002400000480000094000000060200000024000052534131000400000100010007d1fa57c4aed9f0a32e84aa0faefd0de9e8fd6aec8f87fb03766c834c99921eb23be79ad9d5dcc1dd9ad236132102900b723cf980957fc4e177108fc607774f29e8320e92ea05ece4e821c0a5efe8f1645c4c0c93c1ab99285d622caa652c1dfad63d745d6f2de5f17e5eaf0fc4963d261c8a12436518206dc093344d5ad293")]

#if ATLAS_DEV
[assembly: ComVisible(false)]
[assembly: CLSCompliant(true)]
[assembly: AssemblyVersion("4.0.0.0")]
[assembly: NeutralResourcesLanguage("en-US")]
#endif

#if WebFormsCore
[assembly: AssemblyTitle("System.Web.dll")]
[assembly: AssemblyDescription("System.Web.dll")]
[assembly: AssemblyDefaultAlias("System.Web.dll")]
[assembly: AssemblyCompany("Estrellas de Esperanza")]
[assembly: AssemblyProduct("EstrellasDeEsperanza.WebFormsCore")]
[assembly: AssemblyCopyright("© Estrellas de Esperanza. All rights reserved.")]
[assembly: AssemblyFileVersion("8.0.0.0")]
[assembly: AssemblyInformationalVersion("8.0.0.0")]
[assembly: SatelliteContractVersion("8.0.0.0")]
[assembly: NeutralResourcesLanguage("en-US")]
[assembly: AssemblyDelaySign(false)]
[assembly: AssemblyKeyFile(@"..\SigningKey\WebFormsCore.snk")]
[assembly: AssemblyVersion("8.0.0.0")]
#endif

[assembly:System.Runtime.InteropServices.TypeLibVersion(2, 4)]

#if NETFRAMEWORK
// Opts into the VS loading icons from the FrameworkIcon Satellite assemblies found under VSIP\Icons
[assembly:System.Drawing.BitmapSuffixInSatelliteAssemblyAttribute()]
#endif