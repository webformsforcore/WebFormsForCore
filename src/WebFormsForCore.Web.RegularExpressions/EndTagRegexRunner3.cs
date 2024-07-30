// Decompiled with JetBrains decompiler
// Type: System.Web.RegularExpressions.EndTagRegexRunner3
// Assembly: System.Web.RegularExpressions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2D011335-7300-40B5-8CDC-1E3EA0A75C61
// Assembly location: C:\Windows\Microsoft.NET\assembly\GAC_MSIL\System.Web.RegularExpressions\v4.0_4.0.0.0__b03f5f7f11d50a3a\System.Web.RegularExpressions.dll
// XML documentation location: C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.8.1\System.Web.RegularExpressions.xml

#if NETFRAMEWORK

using System.Text.RegularExpressions;

#nullable disable
namespace System.Web.RegularExpressions
{
  internal class EndTagRegexRunner3 : RegexRunner
  {
    protected override void Go()
    {
      string runtext = this.runtext;
      int runtextstart = this.runtextstart;
      int runtextbeg = this.runtextbeg;
      int runtextend = this.runtextend;
      int runtextpos = this.runtextpos;
      int[] runtrack = this.runtrack;
      int runtrackpos1 = this.runtrackpos;
      int[] runstack = this.runstack;
      int runstackpos = this.runstackpos;
      this.CheckTimeout();
      int num1;
      runtrack[num1 = runtrackpos1 - 1] = runtextpos;
      int num2;
      runtrack[num2 = num1 - 1] = 0;
      this.CheckTimeout();
      int num3;
      runstack[num3 = runstackpos - 1] = runtextpos;
      int num4;
      runtrack[num4 = num2 - 1] = 1;
      this.CheckTimeout();
      int end1;
      if (runtextpos == this.runtextstart)
      {
        this.CheckTimeout();
        if (2 <= runtextend - runtextpos && runtext[runtextpos] == '<' && runtext[runtextpos + 1] == '/')
        {
          int num5 = runtextpos + 2;
          this.CheckTimeout();
          runstack[--num3] = num5;
          runtrack[--num4] = 1;
          this.CheckTimeout();
          if (1 <= runtextend - num5)
          {
            end1 = num5 + 1;
            int num6 = 1;
            while (RegexRunner.CharInClass(runtext[end1 - num6--], "\0\u0004\n./:;\0\u0002\u0004\u0005\u0003\u0001\u0006\t\u0013\0"))
            {
              if (num6 <= 0)
              {
                this.CheckTimeout();
                int num7;
                int num8 = (num7 = runtextend - end1) + 1;
                while (--num8 > 0)
                {
                  if (!RegexRunner.CharInClass(runtext[end1++], "\0\u0004\n./:;\0\u0002\u0004\u0005\u0003\u0001\u0006\t\u0013\0"))
                  {
                    --end1;
                    break;
                  }
                }
                if (num7 > num8)
                {
                  int num9;
                  runtrack[num9 = num4 - 1] = num7 - num8 - 1;
                  int num10;
                  runtrack[num10 = num9 - 1] = end1 - 1;
                  runtrack[num4 = num10 - 1] = 2;
                  goto label_12;
                }
                else
                  goto label_12;
              }
            }
            goto label_22;
          }
          else
            goto label_22;
        }
        else
          goto label_22;
      }
      else
        goto label_22;
label_12:
      this.CheckTimeout();
      int start1 = runstack[num3++];
      this.Capture(1, start1, end1);
      int num11;
      runtrack[num11 = num4 - 1] = start1;
      runtrack[num4 = num11 - 1] = 3;
      this.CheckTimeout();
      int num12;
      int num13 = (num12 = runtextend - end1) + 1;
      while (--num13 > 0)
      {
        if (!RegexRunner.CharInClass(runtext[end1++], "\0\0\u0001d"))
        {
          --end1;
          break;
        }
      }
      if (num12 > num13)
      {
        int num14;
        runtrack[num14 = num4 - 1] = num12 - num13 - 1;
        int num15;
        runtrack[num15 = num14 - 1] = end1 - 1;
        runtrack[num4 = num15 - 1] = 4;
      }
label_18:
      this.CheckTimeout();
      int end2;
      int num16;
      if (end1 < runtextend)
      {
        string str = runtext;
        int index1 = end1;
        end2 = index1 + 1;
        if (str[index1] == '>')
        {
          this.CheckTimeout();
          int[] numArray = runstack;
          int index2 = num3;
          int num17 = index2 + 1;
          int start2 = numArray[index2];
          this.Capture(0, start2, end2);
          int num18;
          runtrack[num18 = num4 - 1] = start2;
          runtrack[num16 = num18 - 1] = 3;
        }
        else
          goto label_22;
      }
      else
        goto label_22;
label_21:
      this.CheckTimeout();
      this.runtextpos = end2;
      return;
label_22:
      while (true)
      {
        this.runtrackpos = num4;
        this.runstackpos = num3;
        this.EnsureStorage();
        int runtrackpos2 = this.runtrackpos;
        num3 = this.runstackpos;
        runtrack = this.runtrack;
        runstack = this.runstack;
        int[] numArray = runtrack;
        int index = runtrackpos2;
        num4 = index + 1;
        switch (numArray[index])
        {
          case 1:
            this.CheckTimeout();
            ++num3;
            continue;
          case 2:
            goto label_25;
          case 3:
            this.CheckTimeout();
            runstack[--num3] = runtrack[num4++];
            this.Uncapture();
            continue;
          case 4:
            goto label_28;
          default:
            goto label_23;
        }
      }
label_23:
      this.CheckTimeout();
      int[] numArray1 = runtrack;
      int index3 = num4;
      num16 = index3 + 1;
      end2 = numArray1[index3];
      goto label_21;
label_25:
      this.CheckTimeout();
      int[] numArray2 = runtrack;
      int index4 = num4;
      int num19 = index4 + 1;
      end1 = numArray2[index4];
      int[] numArray3 = runtrack;
      int index5 = num19;
      num4 = index5 + 1;
      int num20 = numArray3[index5];
      if (num20 > 0)
      {
        int num21;
        runtrack[num21 = num4 - 1] = num20 - 1;
        int num22;
        runtrack[num22 = num21 - 1] = end1 - 1;
        runtrack[num4 = num22 - 1] = 2;
        goto label_12;
      }
      else
        goto label_12;
label_28:
      this.CheckTimeout();
      int[] numArray4 = runtrack;
      int index6 = num4;
      int num23 = index6 + 1;
      end1 = numArray4[index6];
      int[] numArray5 = runtrack;
      int index7 = num23;
      num4 = index7 + 1;
      int num24 = numArray5[index7];
      if (num24 > 0)
      {
        int num25;
        runtrack[num25 = num4 - 1] = num24 - 1;
        int num26;
        runtrack[num26 = num25 - 1] = end1 - 1;
        runtrack[num4 = num26 - 1] = 4;
        goto label_18;
      }
      else
        goto label_18;
    }

    protected override bool FindFirstChar()
    {
      if (this.runtextpos <= this.runtextstart)
        return true;
      this.runtextpos = this.runtextend;
      return false;
    }

    protected override void InitTrackCount() => this.runtrackcount = 7;
  }
}
#endif