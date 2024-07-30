// Decompiled with JetBrains decompiler
// Type: System.Web.RegularExpressions.NonWordRegexRunner21
// Assembly: System.Web.RegularExpressions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2D011335-7300-40B5-8CDC-1E3EA0A75C61
// Assembly location: C:\Windows\Microsoft.NET\assembly\GAC_MSIL\System.Web.RegularExpressions\v4.0_4.0.0.0__b03f5f7f11d50a3a\System.Web.RegularExpressions.dll
// XML documentation location: C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.8.1\System.Web.RegularExpressions.xml

#if NETFRAMEWORK

using System.Text.RegularExpressions;

#nullable disable
namespace System.Web.RegularExpressions
{
  internal class NonWordRegexRunner21 : RegexRunner
  {
    protected override void Go()
    {
      string runtext = this.runtext;
      int runtextstart = this.runtextstart;
      int runtextbeg = this.runtextbeg;
      int runtextend = this.runtextend;
      int runtextpos = this.runtextpos;
      int[] runtrack1 = this.runtrack;
      int runtrackpos1 = this.runtrackpos;
      int[] runstack1 = this.runstack;
      int runstackpos1 = this.runstackpos;
      this.CheckTimeout();
      int num1;
      runtrack1[num1 = runtrackpos1 - 1] = runtextpos;
      int num2;
      runtrack1[num2 = num1 - 1] = 0;
      this.CheckTimeout();
      int num3;
      runstack1[num3 = runstackpos1 - 1] = runtextpos;
      int num4;
      runtrack1[num4 = num2 - 1] = 1;
      this.CheckTimeout();
      int end;
      int num5;
      if (runtextpos < runtextend)
      {
        string str = runtext;
        int index1 = runtextpos;
        end = index1 + 1;
        if (RegexRunner.CharInClass(str[index1], "\u0001\0\n\0\u0002\u0004\u0005\u0003\u0001\u0006\t\u0013\0"))
        {
          this.CheckTimeout();
          int[] numArray = runstack1;
          int index2 = num3;
          int num6 = index2 + 1;
          int start = numArray[index2];
          this.Capture(0, start, end);
          int num7;
          runtrack1[num7 = num4 - 1] = start;
          runtrack1[num5 = num7 - 1] = 2;
        }
        else
          goto label_4;
      }
      else
        goto label_4;
label_3:
      this.CheckTimeout();
      this.runtextpos = end;
      return;
label_4:
      int[] runtrack2;
      while (true)
      {
        this.runtrackpos = num4;
        this.runstackpos = num3;
        this.EnsureStorage();
        int runtrackpos2 = this.runtrackpos;
        int runstackpos2 = this.runstackpos;
        runtrack2 = this.runtrack;
        int[] runstack2 = this.runstack;
        int[] numArray = runtrack2;
        int index = runtrackpos2;
        num4 = index + 1;
        switch (numArray[index])
        {
          case 1:
            this.CheckTimeout();
            num3 = runstackpos2 + 1;
            continue;
          case 2:
            this.CheckTimeout();
            runstack2[num3 = runstackpos2 - 1] = runtrack2[num4++];
            this.Uncapture();
            continue;
          default:
            goto label_5;
        }
      }
label_5:
      this.CheckTimeout();
      int[] numArray1 = runtrack2;
      int index3 = num4;
      num5 = index3 + 1;
      end = numArray1[index3];
      goto label_3;
    }

    protected override bool FindFirstChar()
    {
      int runtextpos = this.runtextpos;
      string runtext = this.runtext;
      int num1 = this.runtextend - runtextpos;
      if (num1 <= 0)
        return false;
      do
      {
        --num1;
        if (RegexRunner.CharInClass(runtext[runtextpos++], "\u0001\0\n\0\u0002\u0004\u0005\u0003\u0001\u0006\t\u0013\0"))
          goto label_4;
      }
      while (num1 > 0);
      int num2 = 0;
      goto label_5;
label_4:
      --runtextpos;
      num2 = 1;
label_5:
      this.runtextpos = runtextpos;
      return num2 != 0;
    }

    protected override void InitTrackCount() => this.runtrackcount = 3;
  }
}
#endif