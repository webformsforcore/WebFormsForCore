﻿
#if NETFRAMEWORK

using System.Text.RegularExpressions;

#nullable disable
namespace System.Web.RegularExpressions
{
  internal class GTRegexRunner10 : RegexRunner
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
        string str1 = runtext;
        int index1 = runtextpos;
        int num6 = index1 + 1;
        if (str1[index1] != '%')
        {
          this.CheckTimeout();
          if (num6 < runtextend)
          {
            string str2 = runtext;
            int index2 = num6;
            end = index2 + 1;
            if (str2[index2] == '>')
            {
              this.CheckTimeout();
              int[] numArray = runstack1;
              int index3 = num3;
              int num7 = index3 + 1;
              int start = numArray[index3];
              this.Capture(0, start, end);
              int num8;
              runtrack1[num8 = num4 - 1] = start;
              runtrack1[num5 = num8 - 1] = 2;
            }
            else
              goto label_6;
          }
          else
            goto label_6;
        }
        else
          goto label_6;
      }
      else
        goto label_6;
label_5:
      this.CheckTimeout();
      this.runtextpos = end;
      return;
label_6:
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
            goto label_7;
        }
      }
label_7:
      this.CheckTimeout();
      int[] numArray1 = runtrack2;
      int index4 = num4;
      num5 = index4 + 1;
      end = numArray1[index4];
      goto label_5;
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
        if (RegexRunner.CharInClass(runtext[runtextpos++], "\0\u0003\0\0%&"))
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