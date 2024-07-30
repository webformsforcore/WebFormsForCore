// Decompiled with JetBrains decompiler
// Type: System.Web.RegularExpressions.ExpressionBuilderRegexRunner16
// Assembly: System.Web.RegularExpressions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2D011335-7300-40B5-8CDC-1E3EA0A75C61
// Assembly location: C:\Windows\Microsoft.NET\assembly\GAC_MSIL\System.Web.RegularExpressions\v4.0_4.0.0.0__b03f5f7f11d50a3a\System.Web.RegularExpressions.dll
// XML documentation location: C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.8.1\System.Web.RegularExpressions.xml

#if NETFRAMEWORK

using System.Text.RegularExpressions;

#nullable disable
namespace System.Web.RegularExpressions
{
  internal class ExpressionBuilderRegexRunner16 : RegexRunner
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
      if (runtextpos == this.runtextstart)
      {
        this.CheckTimeout();
        int num5;
        int num6 = (num5 = runtextend - runtextpos) + 1;
        while (--num6 > 0)
        {
          if (!RegexRunner.CharInClass(runtext[runtextpos++], "\0\0\u0001d"))
          {
            --runtextpos;
            break;
          }
        }
        if (num5 > num6)
        {
          int num7;
          runtrack[num7 = num4 - 1] = num5 - num6 - 1;
          int num8;
          runtrack[num8 = num7 - 1] = runtextpos - 1;
          runtrack[num4 = num8 - 1] = 2;
        }
      }
      else
        goto label_43;
label_7:
      this.CheckTimeout();
      if (2 <= runtextend - runtextpos && runtext[runtextpos] == '<' && runtext[runtextpos + 1] == '%')
      {
        runtextpos += 2;
        this.CheckTimeout();
        int num9;
        int num10 = (num9 = runtextend - runtextpos) + 1;
        while (--num10 > 0)
        {
          if (!RegexRunner.CharInClass(runtext[runtextpos++], "\0\0\u0001d"))
          {
            --runtextpos;
            break;
          }
        }
        if (num9 > num10)
        {
          int num11;
          runtrack[num11 = num4 - 1] = num9 - num10 - 1;
          int num12;
          runtrack[num12 = num11 - 1] = runtextpos - 1;
          runtrack[num4 = num12 - 1] = 3;
        }
      }
      else
        goto label_43;
label_14:
      this.CheckTimeout();
      if (runtextpos < runtextend && runtext[runtextpos++] == '$')
      {
        this.CheckTimeout();
        int num13;
        int num14 = (num13 = runtextend - runtextpos) + 1;
        while (--num14 > 0)
        {
          if (!RegexRunner.CharInClass(runtext[runtextpos++], "\0\0\u0001d"))
          {
            --runtextpos;
            break;
          }
        }
        if (num13 > num14)
        {
          int num15;
          runtrack[num15 = num4 - 1] = num13 - num14 - 1;
          int num16;
          runtrack[num16 = num15 - 1] = runtextpos - 1;
          runtrack[num4 = num16 - 1] = 4;
        }
      }
      else
        goto label_43;
label_21:
      this.CheckTimeout();
      int num17;
      runstack[num17 = num3 - 1] = -1;
      int num18;
      runstack[num18 = num17 - 1] = 0;
      int num19;
      runtrack[num19 = num4 - 1] = 5;
      this.CheckTimeout();
      goto label_29;
label_22:
      this.CheckTimeout();
      runstack[--num3] = runtextpos;
      int num20;
      runtrack[num20 = num4 - 1] = 1;
      this.CheckTimeout();
      int num21;
      int num22 = (num21 = runtextend - runtextpos) + 1;
      while (--num22 > 0)
      {
        if (!RegexRunner.CharInClass(runtext[runtextpos++], "\0\u0001\0\0"))
        {
          --runtextpos;
          break;
        }
      }
      if (num21 > num22)
      {
        int num23;
        runtrack[num23 = num20 - 1] = num21 - num22 - 1;
        int num24;
        runtrack[num24 = num23 - 1] = runtextpos - 1;
        runtrack[num20 = num24 - 1] = 6;
      }
label_28:
      this.CheckTimeout();
      int[] numArray1 = runstack;
      int index1 = num3;
      num18 = index1 + 1;
      int start1 = numArray1[index1];
      this.Capture(1, start1, runtextpos);
      int num25;
      runtrack[num25 = num20 - 1] = start1;
      runtrack[num19 = num25 - 1] = 7;
label_29:
      this.CheckTimeout();
      int[] numArray2 = runstack;
      int index2 = num18;
      int num26 = index2 + 1;
      int num27 = numArray2[index2];
      int[] numArray3 = runstack;
      int index3 = num26;
      num3 = index3 + 1;
      int num28;
      int num29 = num28 = numArray3[index3];
      int num30;
      runtrack[num30 = num19 - 1] = num29;
      int num31 = runtextpos;
      if ((num28 != num31 || num27 < 0) && num27 < 1)
      {
        int num32;
        runstack[num32 = num3 - 1] = runtextpos;
        runstack[num3 = num32 - 1] = num27 + 1;
        runtrack[num4 = num30 - 1] = 8;
        if (num4 <= 52 || num3 <= 39)
        {
          runtrack[--num4] = 9;
          goto label_43;
        }
        else
          goto label_22;
      }
      else
      {
        int num33;
        runtrack[num33 = num30 - 1] = num27;
        runtrack[num4 = num33 - 1] = 10;
      }
label_33:
      this.CheckTimeout();
      if (2 <= runtextend - runtextpos && runtext[runtextpos] == '%' && runtext[runtextpos + 1] == '>')
      {
        runtextpos += 2;
        this.CheckTimeout();
        int num34;
        int num35 = (num34 = runtextend - runtextpos) + 1;
        while (--num35 > 0)
        {
          if (!RegexRunner.CharInClass(runtext[runtextpos++], "\0\0\u0001d"))
          {
            --runtextpos;
            break;
          }
        }
        if (num34 > num35)
        {
          int num36;
          runtrack[num36 = num4 - 1] = num34 - num35 - 1;
          int num37;
          runtrack[num37 = num36 - 1] = runtextpos - 1;
          runtrack[num4 = num37 - 1] = 11;
        }
      }
      else
        goto label_43;
label_40:
      this.CheckTimeout();
      int num38;
      if (runtextpos >= runtextend)
      {
        this.CheckTimeout();
        int[] numArray4 = runstack;
        int index4 = num3;
        int num39 = index4 + 1;
        int start2 = numArray4[index4];
        this.Capture(0, start2, runtextpos);
        int num40;
        runtrack[num40 = num4 - 1] = start2;
        runtrack[num38 = num40 - 1] = 7;
      }
      else
        goto label_43;
label_42:
      this.CheckTimeout();
      this.runtextpos = runtextpos;
      return;
label_43:
      int index5;
      int num41;
      while (true)
      {
        this.runtrackpos = num4;
        this.runstackpos = num3;
        this.EnsureStorage();
        int runtrackpos2 = this.runtrackpos;
        num3 = this.runstackpos;
        runtrack = this.runtrack;
        runstack = this.runstack;
        int[] numArray5 = runtrack;
        int index6 = runtrackpos2;
        num4 = index6 + 1;
        switch (numArray5[index6])
        {
          case 1:
            this.CheckTimeout();
            ++num3;
            continue;
          case 2:
            goto label_46;
          case 3:
            goto label_48;
          case 4:
            goto label_50;
          case 5:
            this.CheckTimeout();
            num3 += 2;
            continue;
          case 6:
            goto label_53;
          case 7:
            this.CheckTimeout();
            runstack[--num3] = runtrack[num4++];
            this.Uncapture();
            continue;
          case 8:
            this.CheckTimeout();
            int[] numArray6 = runstack;
            int index7 = num3;
            index5 = index7 + 1;
            if ((num41 = numArray6[index7] - 1) < 0)
            {
              runstack[index5] = runtrack[num4++];
              runstack[num3 = index5 - 1] = num41;
              continue;
            }
            goto label_57;
          case 9:
            goto label_22;
          case 10:
            this.CheckTimeout();
            int[] numArray7 = runtrack;
            int index8 = num4;
            int num42 = index8 + 1;
            int num43 = numArray7[index8];
            int[] numArray8 = runstack;
            int index9;
            int num44 = index9 = num3 - 1;
            int[] numArray9 = runtrack;
            int index10 = num42;
            num4 = index10 + 1;
            int num45 = numArray9[index10];
            numArray8[index9] = num45;
            runstack[num3 = num44 - 1] = num43;
            continue;
          case 11:
            goto label_60;
          default:
            goto label_44;
        }
      }
label_44:
      this.CheckTimeout();
      int[] numArray10 = runtrack;
      int index11 = num4;
      num38 = index11 + 1;
      runtextpos = numArray10[index11];
      goto label_42;
label_46:
      this.CheckTimeout();
      int[] numArray11 = runtrack;
      int index12 = num4;
      int num46 = index12 + 1;
      runtextpos = numArray11[index12];
      int[] numArray12 = runtrack;
      int index13 = num46;
      num4 = index13 + 1;
      int num47 = numArray12[index13];
      if (num47 > 0)
      {
        int num48;
        runtrack[num48 = num4 - 1] = num47 - 1;
        int num49;
        runtrack[num49 = num48 - 1] = runtextpos - 1;
        runtrack[num4 = num49 - 1] = 2;
        goto label_7;
      }
      else
        goto label_7;
label_48:
      this.CheckTimeout();
      int[] numArray13 = runtrack;
      int index14 = num4;
      int num50 = index14 + 1;
      runtextpos = numArray13[index14];
      int[] numArray14 = runtrack;
      int index15 = num50;
      num4 = index15 + 1;
      int num51 = numArray14[index15];
      if (num51 > 0)
      {
        int num52;
        runtrack[num52 = num4 - 1] = num51 - 1;
        int num53;
        runtrack[num53 = num52 - 1] = runtextpos - 1;
        runtrack[num4 = num53 - 1] = 3;
        goto label_14;
      }
      else
        goto label_14;
label_50:
      this.CheckTimeout();
      int[] numArray15 = runtrack;
      int index16 = num4;
      int num54 = index16 + 1;
      runtextpos = numArray15[index16];
      int[] numArray16 = runtrack;
      int index17 = num54;
      num4 = index17 + 1;
      int num55 = numArray16[index17];
      if (num55 > 0)
      {
        int num56;
        runtrack[num56 = num4 - 1] = num55 - 1;
        int num57;
        runtrack[num57 = num56 - 1] = runtextpos - 1;
        runtrack[num4 = num57 - 1] = 4;
        goto label_21;
      }
      else
        goto label_21;
label_53:
      this.CheckTimeout();
      int[] numArray17 = runtrack;
      int index18 = num4;
      int num58 = index18 + 1;
      runtextpos = numArray17[index18];
      int[] numArray18 = runtrack;
      int index19 = num58;
      num20 = index19 + 1;
      int num59 = numArray18[index19];
      if (num59 > 0)
      {
        int num60;
        runtrack[num60 = num20 - 1] = num59 - 1;
        int num61;
        runtrack[num61 = num60 - 1] = runtextpos - 1;
        runtrack[num20 = num61 - 1] = 6;
        goto label_28;
      }
      else
        goto label_28;
label_57:
      int[] numArray19 = runstack;
      int index20 = index5;
      num3 = index20 + 1;
      runtextpos = numArray19[index20];
      int num62;
      runtrack[num62 = num4 - 1] = num41;
      runtrack[num4 = num62 - 1] = 10;
      goto label_33;
label_60:
      this.CheckTimeout();
      int[] numArray20 = runtrack;
      int index21 = num4;
      int num63 = index21 + 1;
      runtextpos = numArray20[index21];
      int[] numArray21 = runtrack;
      int index22 = num63;
      num4 = index22 + 1;
      int num64 = numArray21[index22];
      if (num64 > 0)
      {
        int num65;
        runtrack[num65 = num4 - 1] = num64 - 1;
        int num66;
        runtrack[num66 = num65 - 1] = runtextpos - 1;
        runtrack[num4 = num66 - 1] = 11;
        goto label_40;
      }
      else
        goto label_40;
    }

    protected override bool FindFirstChar()
    {
      if (this.runtextpos <= this.runtextstart)
        return true;
      this.runtextpos = this.runtextend;
      return false;
    }

    protected override void InitTrackCount() => this.runtrackcount = 13;
  }
}
#endif