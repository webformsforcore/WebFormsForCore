// Decompiled with JetBrains decompiler
// Type: System.Web.RegularExpressions.DataBindRegexRunner15
// Assembly: System.Web.RegularExpressions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2D011335-7300-40B5-8CDC-1E3EA0A75C61
// Assembly location: C:\Windows\Microsoft.NET\assembly\GAC_MSIL\System.Web.RegularExpressions\v4.0_4.0.0.0__b03f5f7f11d50a3a\System.Web.RegularExpressions.dll
// XML documentation location: C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.8.1\System.Web.RegularExpressions.xml

#if NETFRAMEWORK

using System.Text.RegularExpressions;

#nullable disable
namespace System.Web.RegularExpressions
{
  internal class DataBindRegexRunner15 : RegexRunner
  {
    protected override void Go()
    {
      string runtext = this.runtext;
      int runtextstart = this.runtextstart;
      int runtextbeg = this.runtextbeg;
      int runtextend = this.runtextend;
      int num1 = this.runtextpos;
      int[] runtrack = this.runtrack;
      int runtrackpos1 = this.runtrackpos;
      int[] runstack = this.runstack;
      int runstackpos = this.runstackpos;
      this.CheckTimeout();
      int num2;
      runtrack[num2 = runtrackpos1 - 1] = num1;
      int num3;
      runtrack[num3 = num2 - 1] = 0;
      this.CheckTimeout();
      int num4;
      runstack[num4 = runstackpos - 1] = num1;
      int num5;
      runtrack[num5 = num3 - 1] = 1;
      this.CheckTimeout();
      if (num1 == this.runtextstart)
      {
        this.CheckTimeout();
        int num6;
        int num7 = (num6 = runtextend - num1) + 1;
        while (--num7 > 0)
        {
          if (!RegexRunner.CharInClass(runtext[num1++], "\0\0\u0001d"))
          {
            --num1;
            break;
          }
        }
        if (num6 > num7)
        {
          int num8;
          runtrack[num8 = num5 - 1] = num6 - num7 - 1;
          int num9;
          runtrack[num9 = num8 - 1] = num1 - 1;
          runtrack[num5 = num9 - 1] = 2;
        }
      }
      else
        goto label_36;
label_7:
      this.CheckTimeout();
      if (2 <= runtextend - num1 && runtext[num1] == '<' && runtext[num1 + 1] == '%')
      {
        num1 += 2;
        this.CheckTimeout();
        int num10;
        if ((num10 = runtextend - num1) > 0)
        {
          int num11;
          runtrack[num11 = num5 - 1] = num10 - 1;
          int num12;
          runtrack[num12 = num11 - 1] = num1;
          runtrack[num5 = num12 - 1] = 3;
        }
      }
      else
        goto label_36;
label_10:
      this.CheckTimeout();
      int num13;
      int num14;
      if (num1 < runtextend && runtext[num1++] == '#')
      {
        this.CheckTimeout();
        int num15;
        runstack[num15 = num4 - 1] = -1;
        runstack[num13 = num15 - 1] = 0;
        runtrack[num14 = num5 - 1] = 4;
        this.CheckTimeout();
        goto label_14;
      }
      else
        goto label_36;
label_12:
      this.CheckTimeout();
      runstack[--num4] = num1;
      runtrack[--num5] = 1;
      this.CheckTimeout();
      if (num1 < runtextend && runtext[num1++] == ':')
      {
        this.CheckTimeout();
        int[] numArray = runstack;
        int index = num4;
        num13 = index + 1;
        int start = numArray[index];
        this.Capture(1, start, num1);
        int num16;
        runtrack[num16 = num5 - 1] = start;
        runtrack[num14 = num16 - 1] = 5;
      }
      else
        goto label_36;
label_14:
      this.CheckTimeout();
      int[] numArray1 = runstack;
      int index1 = num13;
      int num17 = index1 + 1;
      int num18 = numArray1[index1];
      int[] numArray2 = runstack;
      int index2 = num17;
      int num19 = index2 + 1;
      int num20;
      int num21 = num20 = numArray2[index2];
      int num22;
      runtrack[num22 = num14 - 1] = num21;
      int num23 = num1;
      int num24;
      if ((num20 != num23 || num18 < 0) && num18 < 1)
      {
        int num25;
        runstack[num25 = num19 - 1] = num1;
        runstack[num4 = num25 - 1] = num18 + 1;
        runtrack[num5 = num22 - 1] = 6;
        if (num5 <= 68 || num4 <= 51)
        {
          runtrack[--num5] = 7;
          goto label_36;
        }
        else
          goto label_12;
      }
      else
      {
        int num26;
        runtrack[num26 = num22 - 1] = num18;
        runtrack[num24 = num26 - 1] = 8;
      }
label_18:
      this.CheckTimeout();
      int num27;
      runstack[num27 = num19 - 1] = -1;
      int num28;
      runstack[num28 = num27 - 1] = 0;
      int num29;
      runtrack[num29 = num24 - 1] = 4;
      this.CheckTimeout();
      goto label_22;
label_19:
      this.CheckTimeout();
      runstack[--num4] = num1;
      runtrack[--num5] = 1;
      this.CheckTimeout();
      int num30;
      if ((num30 = runtextend - num1) > 0)
      {
        int num31;
        runtrack[num31 = num5 - 1] = num30 - 1;
        int num32;
        runtrack[num32 = num31 - 1] = num1;
        runtrack[num5 = num32 - 1] = 9;
      }
label_21:
      this.CheckTimeout();
      int[] numArray3 = runstack;
      int index3 = num4;
      num28 = index3 + 1;
      int start1 = numArray3[index3];
      this.Capture(2, start1, num1);
      int num33;
      runtrack[num33 = num5 - 1] = start1;
      runtrack[num29 = num33 - 1] = 5;
label_22:
      this.CheckTimeout();
      int[] numArray4 = runstack;
      int index4 = num28;
      int num34 = index4 + 1;
      int num35 = numArray4[index4];
      int[] numArray5 = runstack;
      int index5 = num34;
      num4 = index5 + 1;
      int num36;
      int num37 = num36 = numArray5[index5];
      int num38;
      runtrack[num38 = num29 - 1] = num37;
      int num39 = num1;
      if ((num36 != num39 || num35 < 0) && num35 < 1)
      {
        int num40;
        runstack[num40 = num4 - 1] = num1;
        runstack[num4 = num40 - 1] = num35 + 1;
        runtrack[num5 = num38 - 1] = 10;
        if (num5 <= 68 || num4 <= 51)
        {
          runtrack[--num5] = 11;
          goto label_36;
        }
        else
          goto label_19;
      }
      else
      {
        int num41;
        runtrack[num41 = num38 - 1] = num35;
        runtrack[num5 = num41 - 1] = 8;
      }
label_26:
      this.CheckTimeout();
      if (2 <= runtextend - num1 && runtext[num1] == '%' && runtext[num1 + 1] == '>')
      {
        num1 += 2;
        this.CheckTimeout();
        int num42;
        int num43 = (num42 = runtextend - num1) + 1;
        while (--num43 > 0)
        {
          if (!RegexRunner.CharInClass(runtext[num1++], "\0\0\u0001d"))
          {
            --num1;
            break;
          }
        }
        if (num42 > num43)
        {
          int num44;
          runtrack[num44 = num5 - 1] = num42 - num43 - 1;
          int num45;
          runtrack[num45 = num44 - 1] = num1 - 1;
          runtrack[num5 = num45 - 1] = 12;
        }
      }
      else
        goto label_36;
label_33:
      this.CheckTimeout();
      int num46;
      if (num1 >= runtextend)
      {
        this.CheckTimeout();
        int[] numArray6 = runstack;
        int index6 = num4;
        int num47 = index6 + 1;
        int start2 = numArray6[index6];
        this.Capture(0, start2, num1);
        int num48;
        runtrack[num48 = num5 - 1] = start2;
        runtrack[num46 = num48 - 1] = 5;
      }
      else
        goto label_36;
label_35:
      this.CheckTimeout();
      this.runtextpos = num1;
      return;
label_36:
      int num49 = 0;
      int index7;
      int num50;
      while (true)
      {
        string str1 = null;
        int index8 = 0;
        do
        {
          int num51 = 0;
          string str2 = null;
          int index9 = 0;
          do
          {
            this.runtrackpos = num5;
            this.runstackpos = num4;
            this.EnsureStorage();
            int runtrackpos2 = this.runtrackpos;
            num4 = this.runstackpos;
            runtrack = this.runtrack;
            runstack = this.runstack;
            int[] numArray7 = runtrack;
            int index10 = runtrackpos2;
            num5 = index10 + 1;
            switch (numArray7[index10])
            {
              case 1:
                this.CheckTimeout();
                ++num4;
                continue;
              case 2:
                this.CheckTimeout();
                int[] numArray8 = runtrack;
                int index11 = num5;
                int num52 = index11 + 1;
                num1 = numArray8[index11];
                int[] numArray9 = runtrack;
                int index12 = num52;
                num5 = index12 + 1;
                int num53 = numArray9[index12];
                if (num53 > 0)
                {
                  int num54;
                  runtrack[num54 = num5 - 1] = num53 - 1;
                  int num55;
                  runtrack[num55 = num54 - 1] = num1 - 1;
                  runtrack[num5 = num55 - 1] = 2;
                  goto label_7;
                }
                else
                  goto label_7;
              case 3:
                this.CheckTimeout();
                int[] numArray10 = runtrack;
                int index13 = num5;
                int num56 = index13 + 1;
                int num57 = numArray10[index13];
                int[] numArray11 = runtrack;
                int index14 = num56;
                num5 = index14 + 1;
                num51 = numArray11[index14];
                str2 = runtext;
                index9 = num57;
                num1 = index9 + 1;
                continue;
              case 4:
                goto label_44;
              case 5:
                goto label_45;
              case 6:
                goto label_46;
              case 7:
                goto label_12;
              case 8:
                goto label_49;
              case 9:
                goto label_50;
              case 10:
                goto label_53;
              case 11:
                goto label_19;
              case 12:
                goto label_56;
              default:
                this.CheckTimeout();
                int[] numArray12 = runtrack;
                int index15 = num5;
                num46 = index15 + 1;
                num1 = numArray12[index15];
                goto label_35;
            }
          }
          while (!RegexRunner.CharInClass(str2[index9], "\0\0\u0001d"));
          if (num51 > 0)
          {
            int num58;
            runtrack[num58 = num5 - 1] = num51 - 1;
            int num59;
            runtrack[num59 = num58 - 1] = num1;
            runtrack[num5 = num59 - 1] = 3;
            goto label_10;
          }
          else
            goto label_10;
label_44:
          this.CheckTimeout();
          num4 += 2;
          continue;
label_45:
          this.CheckTimeout();
          runstack[--num4] = runtrack[num5++];
          this.Uncapture();
          continue;
label_46:
          this.CheckTimeout();
          int[] numArray13 = runstack;
          int index16 = num4;
          int index17 = index16 + 1;
          int num60;
          if ((num60 = numArray13[index16] - 1) >= 0)
          {
            int[] numArray14 = runstack;
            int index18 = index17;
            num19 = index18 + 1;
            num1 = numArray14[index18];
            int num61;
            runtrack[num61 = num5 - 1] = num60;
            runtrack[num24 = num61 - 1] = 8;
            goto label_18;
          }
          else
          {
            runstack[index17] = runtrack[num5++];
            runstack[num4 = index17 - 1] = num60;
            continue;
          }
label_49:
          this.CheckTimeout();
          int[] numArray15 = runtrack;
          int index19 = num5;
          int num62 = index19 + 1;
          int num63 = numArray15[index19];
          int[] numArray16 = runstack;
          int index20;
          int num64 = index20 = num4 - 1;
          int[] numArray17 = runtrack;
          int index21 = num62;
          num5 = index21 + 1;
          int num65 = numArray17[index21];
          numArray16[index20] = num65;
          runstack[num4 = num64 - 1] = num63;
          continue;
label_50:
          this.CheckTimeout();
          int[] numArray18 = runtrack;
          int index22 = num5;
          int num66 = index22 + 1;
          int num67 = numArray18[index22];
          int[] numArray19 = runtrack;
          int index23 = num66;
          num5 = index23 + 1;
          num49 = numArray19[index23];
          str1 = runtext;
          index8 = num67;
          num1 = index8 + 1;
        }
        while (!RegexRunner.CharInClass(str1[index8], "\0\u0001\0\0"));
        break;
label_53:
        this.CheckTimeout();
        int[] numArray20 = runstack;
        int index24 = num4;
        index7 = index24 + 1;
        if ((num50 = numArray20[index24] - 1) < 0)
        {
          runstack[index7] = runtrack[num5++];
          runstack[num4 = index7 - 1] = num50;
        }
        else
          goto label_54;
      }
      if (num49 > 0)
      {
        int num68;
        runtrack[num68 = num5 - 1] = num49 - 1;
        int num69;
        runtrack[num69 = num68 - 1] = num1;
        runtrack[num5 = num69 - 1] = 9;
        goto label_21;
      }
      else
        goto label_21;
label_54:
      int[] numArray21 = runstack;
      int index25 = index7;
      num4 = index25 + 1;
      num1 = numArray21[index25];
      int num70;
      runtrack[num70 = num5 - 1] = num50;
      runtrack[num5 = num70 - 1] = 8;
      goto label_26;
label_56:
      this.CheckTimeout();
      int[] numArray22 = runtrack;
      int index26 = num5;
      int num71 = index26 + 1;
      num1 = numArray22[index26];
      int[] numArray23 = runtrack;
      int index27 = num71;
      num5 = index27 + 1;
      int num72 = numArray23[index27];
      if (num72 > 0)
      {
        int num73;
        runtrack[num73 = num5 - 1] = num72 - 1;
        int num74;
        runtrack[num74 = num73 - 1] = num1 - 1;
        runtrack[num5 = num74 - 1] = 12;
        goto label_33;
      }
      else
        goto label_33;
    }

    protected override bool FindFirstChar()
    {
      if (this.runtextpos <= this.runtextstart)
        return true;
      this.runtextpos = this.runtextend;
      return false;
    }

    protected override void InitTrackCount() => this.runtrackcount = 17;
  }
}
#endif