﻿#if NETFRAMEWORK

using System.Text.RegularExpressions;

#nullable disable
namespace System.Web.RegularExpressions
{
  internal class AspExprRegexFactory5 : RegexRunnerFactory
  {
    protected override RegexRunner CreateInstance() => (RegexRunner) new AspExprRegexRunner5();
  }
}

#endif