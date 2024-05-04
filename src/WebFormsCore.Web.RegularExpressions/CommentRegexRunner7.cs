// Decompiled with JetBrains decompiler
// Type: System.Web.RegularExpressions.CommentRegexRunner7
// Assembly: System.Web.RegularExpressions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2D011335-7300-40B5-8CDC-1E3EA0A75C61
// Assembly location: C:\Windows\Microsoft.NET\assembly\GAC_MSIL\System.Web.RegularExpressions\v4.0_4.0.0.0__b03f5f7f11d50a3a\System.Web.RegularExpressions.dll
// XML documentation location: C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.8.1\System.Web.RegularExpressions.xml

#if NETFRAMEWORK

using System.Text.RegularExpressions;

#nullable disable
namespace System.Web.RegularExpressions
{
  internal class CommentRegexRunner7 : RegexRunner
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
      int index1;
      runstack[index1 = runstackpos - 1] = runtextpos;
      int num3;
      runtrack[num3 = num2 - 1] = 1;
      this.CheckTimeout();
      int num4;
      int num5;
      int num6;
      if (runtextpos == this.runtextstart)
      {
        this.CheckTimeout();
        if (4 <= runtextend - runtextpos && runtext[runtextpos] == '<' && runtext[runtextpos + 1] == '%' && runtext[runtextpos + 2] == '-' && runtext[runtextpos + 3] == '-')
        {
          num4 = runtextpos + 4;
          this.CheckTimeout();
          runstack[num5 = index1 - 1] = -1;
          runtrack[num6 = num3 - 1] = 1;
          this.CheckTimeout();
        }
        else
          goto label_18;
      }
      else
        goto label_18;
label_12:
      this.CheckTimeout();
      int[] numArray1 = runstack;
      int index2 = num5;
      index1 = index2 + 1;
      int num7;
      int num8 = num7 = numArray1[index2];
      int num9;
      runtrack[num9 = num6 - 1] = num8 == -1 ? num4 : num8;
      int num10 = num4;
      if (num7 != num10)
      {
        int num11;
        runtrack[num11 = num9 - 1] = num4;
        runtrack[num3 = num11 - 1] = 4;
      }
      else
      {
        runstack[--index1] = num8;
        runtrack[num3 = num9 - 1] = 5;
      }
      this.CheckTimeout();
      int end1;
      int num12;
      if (3 <= runtextend - num4 && runtext[num4] == '-' && runtext[num4 + 1] == '%' && runtext[num4 + 2] == '>')
      {
        end1 = num4 + 3;
        this.CheckTimeout();
        int[] numArray2 = runstack;
        int index3 = index1;
        int num13 = index3 + 1;
        int start = numArray2[index3];
        this.Capture(0, start, end1);
        int num14;
        runtrack[num14 = num3 - 1] = start;
        runtrack[num12 = num14 - 1] = 3;
      }
      else
        goto label_18;
label_17:
      this.CheckTimeout();
      this.runtextpos = end1;
      return;
label_18:
      while (true)
      {
        int num15;
        string str;
        int index4;
        do
        {
          int end2;
          do
          {
            this.runtrackpos = num3;
            this.runstackpos = index1;
            this.EnsureStorage();
            int runtrackpos2 = this.runtrackpos;
            index1 = this.runstackpos;
            runtrack = this.runtrack;
            runstack = this.runstack;
            int[] numArray3 = runtrack;
            int index5 = runtrackpos2;
            num3 = index5 + 1;
            int num16;
            switch (numArray3[index5])
            {
              case 1:
                goto label_20;
              case 2:
                this.CheckTimeout();
                int[] numArray4 = runtrack;
                int index6 = num3;
                int num17 = index6 + 1;
                end2 = numArray4[index6];
                int[] numArray5 = runtrack;
                int index7 = num17;
                num16 = index7 + 1;
                int num18 = numArray5[index7];
                if (num18 > 0)
                {
                  int num19;
                  runtrack[num19 = num16 - 1] = num18 - 1;
                  int num20;
                  runtrack[num20 = num19 - 1] = end2 - 1;
                  runtrack[num16 = num20 - 1] = 2;
                  break;
                }
                break;
              case 3:
                goto label_23;
              case 4:
                this.CheckTimeout();
                int[] numArray6 = runtrack;
                int index8 = num3;
                int num21 = index8 + 1;
                end2 = numArray6[index8];
                runstack[--index1] = end2;
                runtrack[num15 = num21 - 1] = 5;
                if (num15 > 40 && index1 > 30)
                {
                  this.CheckTimeout();
                  int num22;
                  runstack[num22 = index1 - 1] = end2;
                  int num23;
                  runtrack[num23 = num15 - 1] = 1;
                  this.CheckTimeout();
                  runstack[index1 = num22 - 1] = end2;
                  runtrack[num16 = num23 - 1] = 1;
                  this.CheckTimeout();
                  int num24;
                  int num25 = (num24 = runtextend - end2) + 1;
                  while (--num25 > 0)
                  {
                    if (runtext[end2++] == '-')
                    {
                      --end2;
                      break;
                    }
                  }
                  if (num24 > num25)
                  {
                    int num26;
                    runtrack[num26 = num16 - 1] = num24 - num25 - 1;
                    int num27;
                    runtrack[num27 = num26 - 1] = end2 - 1;
                    runtrack[num16 = num27 - 1] = 2;
                    break;
                  }
                  break;
                }
                goto label_25;
              case 5:
                goto label_26;
              default:
                goto label_19;
            }
            this.CheckTimeout();
            int start = runstack[index1++];
            this.Capture(2, start, end2);
            int num28;
            runtrack[num28 = num16 - 1] = start;
            runtrack[num3 = num28 - 1] = 3;
            this.CheckTimeout();
          }
          while (end2 >= runtextend);
          str = runtext;
          index4 = end2;
          num4 = index4 + 1;
        }
        while (str[index4] != '-');
        break;
label_20:
        this.CheckTimeout();
        ++index1;
        continue;
label_23:
        this.CheckTimeout();
        runstack[--index1] = runtrack[num3++];
        this.Uncapture();
        continue;
label_25:
        runtrack[num3 = num15 - 1] = 6;
        continue;
label_26:
        this.CheckTimeout();
        runstack[index1] = runtrack[num3++];
      }
      this.CheckTimeout();
      int[] numArray7 = runstack;
      int index9 = index1;
      num5 = index9 + 1;
      int start1 = numArray7[index9];
      this.Capture(1, start1, num4);
      int num29;
      runtrack[num29 = num3 - 1] = start1;
      runtrack[num6 = num29 - 1] = 3;
      goto label_12;
label_19:
      this.CheckTimeout();
      int[] numArray8 = runtrack;
      int index10 = num3;
      num12 = index10 + 1;
      end1 = numArray8[index10];
      goto label_17;
    }

    protected override bool FindFirstChar()
    {
      if (this.runtextpos <= this.runtextstart)
        return true;
      this.runtextpos = this.runtextend;
      return false;
    }

    protected override void InitTrackCount() => this.runtrackcount = 10;
  }
}
#endif