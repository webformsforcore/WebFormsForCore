// Decompiled with JetBrains decompiler
// Type: System.Web.RegularExpressions.BindItemParametersRegexRunner27
// Assembly: System.Web.RegularExpressions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2D011335-7300-40B5-8CDC-1E3EA0A75C61
// Assembly location: C:\Windows\Microsoft.NET\assembly\GAC_MSIL\System.Web.RegularExpressions\v4.0_4.0.0.0__b03f5f7f11d50a3a\System.Web.RegularExpressions.dll
// XML documentation location: C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.8.1\System.Web.RegularExpressions.xml

#if NETFRAMEWORK

using System.Text.RegularExpressions;

#nullable disable
namespace System.Web.RegularExpressions
{
  internal class BindItemParametersRegexRunner27 : RegexRunner
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
      int num5;
      runstack[num5 = num3 - 1] = runtextpos;
      int num6;
      runtrack[num6 = num4 - 1] = 1;
      this.CheckTimeout();
      int num7;
      runstack[num7 = num5 - 1] = runtextpos;
      int num8;
      runtrack[num8 = num6 - 1] = 1;
      this.CheckTimeout();
      int end;
      if (1 <= runtextend - runtextpos)
      {
        end = runtextpos + 1;
        int num9 = 1;
        while (RegexRunner.CharInClass(runtext[end - num9--], "\0\u0002\n./\0\u0002\u0004\u0005\u0003\u0001\u0006\t\u0013\0"))
        {
          if (num9 <= 0)
          {
            this.CheckTimeout();
            int num10;
            int num11 = (num10 = runtextend - end) + 1;
            while (--num11 > 0)
            {
              if (!RegexRunner.CharInClass(runtext[end++], "\0\u0002\n./\0\u0002\u0004\u0005\u0003\u0001\u0006\t\u0013\0"))
              {
                --end;
                break;
              }
            }
            if (num10 > num11)
            {
              int num12;
              runtrack[num12 = num8 - 1] = num10 - num11 - 1;
              int num13;
              runtrack[num13 = num12 - 1] = end - 1;
              runtrack[num8 = num13 - 1] = 2;
              goto label_10;
            }
            else
              goto label_10;
          }
        }
        goto label_19;
      }
      else
        goto label_19;
label_10:
      this.CheckTimeout();
      int[] numArray1 = runstack;
      int index1 = num7;
      int num14 = index1 + 1;
      int start1 = numArray1[index1];
      this.Capture(1, start1, end);
      int num15;
      runtrack[num15 = num8 - 1] = start1;
      int num16;
      runtrack[num16 = num15 - 1] = 3;
      this.CheckTimeout();
      int[] numArray2 = runstack;
      int index2 = num14;
      num7 = index2 + 1;
      int start2 = numArray2[index2];
      this.Capture(2, start2, end);
      int num17;
      runtrack[num17 = num16 - 1] = start2;
      runtrack[num8 = num17 - 1] = 3;
      this.CheckTimeout();
      int num18;
      int num19 = (num18 = runtextend - end) + 1;
      while (--num19 > 0)
      {
        if (!RegexRunner.CharInClass(runtext[end++], "\0\0\u0001d"))
        {
          --end;
          break;
        }
      }
      if (num18 > num19)
      {
        int num20;
        runtrack[num20 = num8 - 1] = num18 - num19 - 1;
        int num21;
        runtrack[num21 = num20 - 1] = end - 1;
        runtrack[num8 = num21 - 1] = 4;
      }
label_16:
      this.CheckTimeout();
      int num22;
      if (end >= runtextend)
      {
        this.CheckTimeout();
        int[] numArray3 = runstack;
        int index3 = num7;
        int num23 = index3 + 1;
        int start3 = numArray3[index3];
        this.Capture(0, start3, end);
        int num24;
        runtrack[num24 = num8 - 1] = start3;
        runtrack[num22 = num24 - 1] = 3;
      }
      else
        goto label_19;
label_18:
      this.CheckTimeout();
      this.runtextpos = end;
      return;
label_19:
      while (true)
      {
        this.runtrackpos = num8;
        this.runstackpos = num7;
        this.EnsureStorage();
        int runtrackpos2 = this.runtrackpos;
        num7 = this.runstackpos;
        runtrack = this.runtrack;
        runstack = this.runstack;
        int[] numArray4 = runtrack;
        int index4 = runtrackpos2;
        num8 = index4 + 1;
        switch (numArray4[index4])
        {
          case 1:
            this.CheckTimeout();
            ++num7;
            continue;
          case 2:
            goto label_22;
          case 3:
            this.CheckTimeout();
            runstack[--num7] = runtrack[num8++];
            this.Uncapture();
            continue;
          case 4:
            goto label_25;
          default:
            goto label_20;
        }
      }
label_20:
      this.CheckTimeout();
      int[] numArray5 = runtrack;
      int index5 = num8;
      num22 = index5 + 1;
      end = numArray5[index5];
      goto label_18;
label_22:
      this.CheckTimeout();
      int[] numArray6 = runtrack;
      int index6 = num8;
      int num25 = index6 + 1;
      end = numArray6[index6];
      int[] numArray7 = runtrack;
      int index7 = num25;
      num8 = index7 + 1;
      int num26 = numArray7[index7];
      if (num26 > 0)
      {
        int num27;
        runtrack[num27 = num8 - 1] = num26 - 1;
        int num28;
        runtrack[num28 = num27 - 1] = end - 1;
        runtrack[num8 = num28 - 1] = 2;
        goto label_10;
      }
      else
        goto label_10;
label_25:
      this.CheckTimeout();
      int[] numArray8 = runtrack;
      int index8 = num8;
      int num29 = index8 + 1;
      end = numArray8[index8];
      int[] numArray9 = runtrack;
      int index9 = num29;
      num8 = index9 + 1;
      int num30 = numArray9[index9];
      if (num30 > 0)
      {
        int num31;
        runtrack[num31 = num8 - 1] = num30 - 1;
        int num32;
        runtrack[num32 = num31 - 1] = end - 1;
        runtrack[num8 = num32 - 1] = 4;
        goto label_16;
      }
      else
        goto label_16;
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
        if (RegexRunner.CharInClass(runtext[runtextpos++], "\0\u0002\n./\0\u0002\u0004\u0005\u0003\u0001\u0006\t\u0013\0"))
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

    protected override void InitTrackCount() => this.runtrackcount = 9;
  }
}

#endif