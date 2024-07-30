// Decompiled with JetBrains decompiler
// Type: System.Web.RegularExpressions.FormatStringRegexRunner19
// Assembly: System.Web.RegularExpressions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2D011335-7300-40B5-8CDC-1E3EA0A75C61
// Assembly location: C:\Windows\Microsoft.NET\assembly\GAC_MSIL\System.Web.RegularExpressions\v4.0_4.0.0.0__b03f5f7f11d50a3a\System.Web.RegularExpressions.dll
// XML documentation location: C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.8.1\System.Web.RegularExpressions.xml

#if NETFRAMEWORK

using System.Text.RegularExpressions;

#nullable disable
namespace System.Web.RegularExpressions
{
  internal class FormatStringRegexRunner19 : RegexRunner
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
      int num6;
      if (runtextpos <= runtextbeg || runtext[runtextpos - 1] == '\n')
      {
        this.CheckTimeout();
        int num7;
        runstack[num7 = num3 - 1] = runtextpos;
        int num8;
        runtrack[num8 = num4 - 1] = 1;
        this.CheckTimeout();
        runstack[num5 = num7 - 1] = -1;
        runtrack[num6 = num8 - 1] = 1;
        this.CheckTimeout();
        goto label_16;
      }
      else
        goto label_23;
label_2:
      this.CheckTimeout();
      runstack[--num3] = runtextpos;
      int num9;
      runtrack[num9 = num4 - 1] = 1;
      this.CheckTimeout();
      int num10;
      int num11 = (num10 = runtextend - runtextpos) + 1;
      while (--num11 > 0)
      {
        if (runtext[runtextpos++] == '"')
        {
          --runtextpos;
          break;
        }
      }
      if (num10 > num11)
      {
        int num12;
        runtrack[num12 = num9 - 1] = num10 - num11 - 1;
        int num13;
        runtrack[num13 = num12 - 1] = runtextpos - 1;
        runtrack[num9 = num13 - 1] = 2;
      }
label_8:
      this.CheckTimeout();
      int num14;
      runstack[num14 = num3 - 1] = -1;
      int num15;
      runstack[num15 = num14 - 1] = 0;
      int num16;
      runtrack[num16 = num9 - 1] = 3;
      this.CheckTimeout();
      goto label_11;
label_9:
      this.CheckTimeout();
      runstack[--num3] = runtextpos;
      runtrack[--num4] = 1;
      this.CheckTimeout();
      if (2 <= runtextend - runtextpos && runtext[runtextpos] == '"' && runtext[runtextpos + 1] == '"')
      {
        runtextpos += 2;
        this.CheckTimeout();
        int[] numArray = runstack;
        int index = num3;
        num15 = index + 1;
        int start = numArray[index];
        this.Capture(3, start, runtextpos);
        int num17;
        runtrack[num17 = num4 - 1] = start;
        runtrack[num16 = num17 - 1] = 4;
      }
      else
        goto label_23;
label_11:
      this.CheckTimeout();
      int[] numArray1 = runstack;
      int index1 = num15;
      int num18 = index1 + 1;
      int num19 = numArray1[index1];
      int[] numArray2 = runstack;
      int index2 = num18;
      int num20 = index2 + 1;
      int num21;
      int num22 = num21 = numArray2[index2];
      int num23;
      runtrack[num23 = num16 - 1] = num22;
      int num24 = runtextpos;
      int num25;
      if ((num21 != num24 || num19 < 0) && num19 < 1)
      {
        int num26;
        runstack[num26 = num20 - 1] = runtextpos;
        runstack[num3 = num26 - 1] = num19 + 1;
        runtrack[num4 = num23 - 1] = 5;
        if (num4 <= 60 || num3 <= 45)
        {
          runtrack[--num4] = 6;
          goto label_23;
        }
        else
          goto label_9;
      }
      else
      {
        int num27;
        runtrack[num27 = num23 - 1] = num19;
        runtrack[num25 = num27 - 1] = 7;
      }
label_15:
      this.CheckTimeout();
      int[] numArray3 = runstack;
      int index3 = num20;
      num5 = index3 + 1;
      int start1 = numArray3[index3];
      this.Capture(2, start1, runtextpos);
      int num28;
      runtrack[num28 = num25 - 1] = start1;
      runtrack[num6 = num28 - 1] = 4;
label_16:
      this.CheckTimeout();
      int[] numArray4 = runstack;
      int index4 = num5;
      int num29 = index4 + 1;
      int num30;
      int num31 = num30 = numArray4[index4];
      int num32;
      runtrack[num32 = num6 - 1] = num31;
      int num33 = runtextpos;
      int num34;
      if (num30 != num33)
      {
        int num35;
        runtrack[num35 = num32 - 1] = runtextpos;
        runstack[num3 = num29 - 1] = runtextpos;
        runtrack[num4 = num35 - 1] = 8;
        if (num4 <= 60 || num3 <= 45)
        {
          runtrack[--num4] = 9;
          goto label_23;
        }
        else
          goto label_2;
      }
      else
        runtrack[num34 = num32 - 1] = 10;
label_20:
      this.CheckTimeout();
      int[] numArray5 = runstack;
      int index5 = num29;
      num3 = index5 + 1;
      int start2 = numArray5[index5];
      this.Capture(1, start2, runtextpos);
      int num36;
      runtrack[num36 = num34 - 1] = start2;
      runtrack[num4 = num36 - 1] = 4;
      this.CheckTimeout();
      int num37;
      if (runtextpos >= runtextend || runtext[runtextpos] == '\n')
      {
        this.CheckTimeout();
        int[] numArray6 = runstack;
        int index6 = num3;
        int num38 = index6 + 1;
        int start3 = numArray6[index6];
        this.Capture(0, start3, runtextpos);
        int num39;
        runtrack[num39 = num4 - 1] = start3;
        runtrack[num37 = num39 - 1] = 4;
      }
      else
        goto label_23;
label_22:
      this.CheckTimeout();
      this.runtextpos = runtextpos;
      return;
label_23:
      int index7;
      int num40;
      while (true)
      {
        this.runtrackpos = num4;
        this.runstackpos = num3;
        this.EnsureStorage();
        int runtrackpos2 = this.runtrackpos;
        num3 = this.runstackpos;
        runtrack = this.runtrack;
        runstack = this.runstack;
        int[] numArray7 = runtrack;
        int index8 = runtrackpos2;
        num4 = index8 + 1;
        switch (numArray7[index8])
        {
          case 1:
            this.CheckTimeout();
            ++num3;
            continue;
          case 2:
            goto label_26;
          case 3:
            this.CheckTimeout();
            num3 += 2;
            continue;
          case 4:
            this.CheckTimeout();
            runstack[--num3] = runtrack[num4++];
            this.Uncapture();
            continue;
          case 5:
            this.CheckTimeout();
            int[] numArray8 = runstack;
            int index9 = num3;
            index7 = index9 + 1;
            if ((num40 = numArray8[index9] - 1) < 0)
            {
              runstack[index7] = runtrack[num4++];
              runstack[num3 = index7 - 1] = num40;
              continue;
            }
            goto label_31;
          case 6:
            goto label_9;
          case 7:
            this.CheckTimeout();
            int[] numArray9 = runtrack;
            int index10 = num4;
            int num41 = index10 + 1;
            int num42 = numArray9[index10];
            int[] numArray10 = runstack;
            int index11;
            int num43 = index11 = num3 - 1;
            int[] numArray11 = runtrack;
            int index12 = num41;
            num4 = index12 + 1;
            int num44 = numArray11[index12];
            numArray10[index11] = num44;
            runstack[num3 = num43 - 1] = num42;
            continue;
          case 8:
            goto label_34;
          case 9:
            goto label_2;
          case 10:
            this.CheckTimeout();
            runstack[--num3] = runtrack[num4++];
            continue;
          default:
            goto label_24;
        }
      }
label_24:
      this.CheckTimeout();
      int[] numArray12 = runtrack;
      int index13 = num4;
      num37 = index13 + 1;
      runtextpos = numArray12[index13];
      goto label_22;
label_26:
      this.CheckTimeout();
      int[] numArray13 = runtrack;
      int index14 = num4;
      int num45 = index14 + 1;
      runtextpos = numArray13[index14];
      int[] numArray14 = runtrack;
      int index15 = num45;
      num9 = index15 + 1;
      int num46 = numArray14[index15];
      if (num46 > 0)
      {
        int num47;
        runtrack[num47 = num9 - 1] = num46 - 1;
        int num48;
        runtrack[num48 = num47 - 1] = runtextpos - 1;
        runtrack[num9 = num48 - 1] = 2;
        goto label_8;
      }
      else
        goto label_8;
label_31:
      int[] numArray15 = runstack;
      int index16 = index7;
      num20 = index16 + 1;
      runtextpos = numArray15[index16];
      int num49;
      runtrack[num49 = num4 - 1] = num40;
      runtrack[num25 = num49 - 1] = 7;
      goto label_15;
label_34:
      this.CheckTimeout();
      int[] numArray16 = runtrack;
      int index17 = num4;
      int num50 = index17 + 1;
      runtextpos = numArray16[index17];
      int[] numArray17 = runstack;
      int index18 = num3;
      num29 = index18 + 1;
      int num51 = numArray17[index18];
      runtrack[num34 = num50 - 1] = 10;
      goto label_20;
    }

    protected override bool FindFirstChar() => true;

    protected override void InitTrackCount() => this.runtrackcount = 15;
  }
}
#endif