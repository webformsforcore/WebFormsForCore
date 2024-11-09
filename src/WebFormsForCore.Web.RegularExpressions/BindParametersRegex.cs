#if NETFRAMEWORK

using System.Collections;
using System.Text.RegularExpressions;

#nullable disable
namespace System.Web.RegularExpressions
{
  internal class BindParametersRegex : Regex
  {
    public BindParametersRegex()
    {
      this.pattern = "\\s*((\"(?<fieldName>(([\\w\\.]+)|(\\[.+\\])))\")|('(?<fieldName>(([\\w\\.]+)|(\\[.+\\])))'))\\s*(,\\s*((\"(?<formatString>.*)\")|('(?<formatString>.*)'))\\s*)?\\s*\\z";
      this.roptions = RegexOptions.Multiline | RegexOptions.Singleline;
      this.internalMatchTimeout = TimeSpan.FromTicks(-10000L);
      this.factory = (RegexRunnerFactory) new BindParametersRegexFactory18();
      this.capnames = new Hashtable();
      this.capnames.Add((object) "13", (object) 13);
      this.capnames.Add((object) "fieldName", (object) 14);
      this.capnames.Add((object) "8", (object) 8);
      this.capnames.Add((object) "9", (object) 9);
      this.capnames.Add((object) "11", (object) 11);
      this.capnames.Add((object) "2", (object) 2);
      this.capnames.Add((object) "3", (object) 3);
      this.capnames.Add((object) "0", (object) 0);
      this.capnames.Add((object) "1", (object) 1);
      this.capnames.Add((object) "6", (object) 6);
      this.capnames.Add((object) "7", (object) 7);
      this.capnames.Add((object) "4", (object) 4);
      this.capnames.Add((object) "5", (object) 5);
      this.capnames.Add((object) "12", (object) 12);
      this.capnames.Add((object) "formatString", (object) 15);
      this.capnames.Add((object) "10", (object) 10);
      this.capslist = new string[16];
      this.capslist[0] = "0";
      this.capslist[1] = "1";
      this.capslist[2] = "2";
      this.capslist[3] = "3";
      this.capslist[4] = "4";
      this.capslist[5] = "5";
      this.capslist[6] = "6";
      this.capslist[7] = "7";
      this.capslist[8] = "8";
      this.capslist[9] = "9";
      this.capslist[10] = "10";
      this.capslist[11] = "11";
      this.capslist[12] = "12";
      this.capslist[13] = "13";
      this.capslist[14] = "fieldName";
      this.capslist[15] = "formatString";
      this.capsize = 16;
      this.InitializeReferences();
    }

    public BindParametersRegex(TimeSpan A_1)
      : this()
    {
      Regex.ValidateMatchTimeout(A_1);
      this.internalMatchTimeout = A_1;
    }
  }
}

#endif