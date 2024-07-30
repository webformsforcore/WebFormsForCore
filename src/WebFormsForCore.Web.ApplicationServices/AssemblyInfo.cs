using System;
using System.Reflection;
using System.Resources;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;

#if !WebFormsForCore
[assembly: InternalsVisibleTo("System.Web, PublicKey=002400000480000094000000060200000024000052534131000400000100010007d1fa57c4aed9f0a32e84aa0faefd0de9e8fd6aec8f87fb03766c834c99921eb23be79ad9d5dcc1dd9ad236132102900b723cf980957fc4e177108fc607774f29e8320e92ea05ece4e821c0a5efe8f1645c4c0c93c1ab99285d622caa652c1dfad63d745d6f2de5f17e5eaf0fc4963d261c8a12436518206dc093344d5ad293")]
#else
[assembly: InternalsVisibleTo("System.Web, PublicKey=00240000048000009400000006020000002400005253413100040000010001003d8a5d984adcdd2aa737bf3ed437819f4e7c988cb40a4dc2cf3daeb1aef98355808f626e611754c29f8647123d00e6d52797da1dd16721d906c3d09d0425e8983ea7e265a579f13c3b22b7505632eff882b7c345dfa08f6cf0f86a67896398cc2066dc260e92203f4a8bd66ab336fb6d82b88893e1e588f126a7ebbff4057cca")]
[assembly: InternalsVisibleTo("EstrellasDeEsperanza.WebFormsForCore.Web, PublicKey=00240000048000009400000006020000002400005253413100040000010001003d8a5d984adcdd2aa737bf3ed437819f4e7c988cb40a4dc2cf3daeb1aef98355808f626e611754c29f8647123d00e6d52797da1dd16721d906c3d09d0425e8983ea7e265a579f13c3b22b7505632eff882b7c345dfa08f6cf0f86a67896398cc2066dc260e92203f4a8bd66ab336fb6d82b88893e1e588f126a7ebbff4057cca")]
#endif

[assembly: ComVisible(false)]
[assembly: CLSCompliant(true)]
[assembly: AllowPartiallyTrustedCallers(PartialTrustVisibilityLevel = PartialTrustVisibilityLevel.NotVisibleByDefault)]
[assembly: SecurityRules(SecurityRuleSet.Level1, SkipVerificationInFullTrust = true)]
[assembly: AssemblyTitle("System.Web.ApplicationServices.dll")]
[assembly: AssemblyDescription("System.Web.ApplicationServices.dll")]
[assembly: AssemblyDefaultAlias("System.Web.ApplicationServices.dll")]
[assembly: AssemblyCompany("Estrellas de Esperanza")]
[assembly: AssemblyProduct("EstrellasDeEsperanza.WebFormsForCore")]
[assembly: AssemblyCopyright("© Estrellas de Esperanza. All rights reserved.")]
[assembly: AssemblyFileVersion("8.0.0.0")]
[assembly: AssemblyInformationalVersion("8.0.0.0")]
[assembly: SatelliteContractVersion("8.0.0.0")]
[assembly: NeutralResourcesLanguage("en-US")]
[assembly: AssemblyDelaySign(false)]
[assembly: AssemblyKeyFile(@"..\SigningKey\WebFormsForCore.snk")]
[assembly: DefaultDllImportSearchPaths(DllImportSearchPath.System32 | DllImportSearchPath.AssemblyDirectory)]
[assembly: AssemblyVersion("8.0.0.0")]
