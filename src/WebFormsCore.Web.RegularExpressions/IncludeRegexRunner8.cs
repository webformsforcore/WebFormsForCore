// Decompiled with JetBrains decompiler
// Type: System.Web.RegularExpressions.IncludeRegexRunner8
// Assembly: System.Web.RegularExpressions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2D011335-7300-40B5-8CDC-1E3EA0A75C61
// Assembly location: C:\Windows\Microsoft.NET\assembly\GAC_MSIL\System.Web.RegularExpressions\v4.0_4.0.0.0__b03f5f7f11d50a3a\System.Web.RegularExpressions.dll
// XML documentation location: C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.8.1\System.Web.RegularExpressions.xml

#if NETFRAMEWORK

using System.Globalization;
using System.Text.RegularExpressions;

#nullable disable
namespace System.Web.RegularExpressions
{
  internal class IncludeRegexRunner8 : RegexRunner
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
      int num25 = 0;
      if (runtextpos == this.runtextstart)
      {
        this.CheckTimeout();
        if (4 <= runtextend - runtextpos && runtext[runtextpos] == '<' && runtext[runtextpos + 1] == '!' && runtext[runtextpos + 2] == '-' && runtext[runtextpos + 3] == '-')
        {
          num5 = runtextpos + 4;
          this.CheckTimeout();
          int num6;
          int num7 = (num6 = runtextend - num5) + 1;
          while (--num7 > 0)
          {
            if (!RegexRunner.CharInClass(runtext[num5++], "\0\0\u0001d"))
            {
              --num5;
              break;
            }
          }
          if (num6 > num7)
          {
            int num8;
            runtrack[num8 = num4 - 1] = num6 - num7 - 1;
            int num9;
            runtrack[num9 = num8 - 1] = num5 - 1;
            runtrack[num4 = num9 - 1] = 2;
          }
        }
        else
          goto label_68;
      }
      else
        goto label_68;
label_8:
      this.CheckTimeout();
      int num10;
      if (num5 < runtextend)
      {
        string str = runtext;
        int index1 = num5;
        int index2 = index1 + 1;
        if (str[index1] == '#')
        {
          this.CheckTimeout();
          if (7 <= runtextend - index2 && char.ToLower(runtext[index2], CultureInfo.CurrentCulture) == 'i' && char.ToLower(runtext[index2 + 1], CultureInfo.CurrentCulture) == 'n' && char.ToLower(runtext[index2 + 2], CultureInfo.CurrentCulture) == 'c' && char.ToLower(runtext[index2 + 3], CultureInfo.CurrentCulture) == 'l' && char.ToLower(runtext[index2 + 4], CultureInfo.CurrentCulture) == 'u' && char.ToLower(runtext[index2 + 5], CultureInfo.CurrentCulture) == 'd' && char.ToLower(runtext[index2 + 6], CultureInfo.CurrentCulture) == 'e')
          {
            num10 = index2 + 7;
            this.CheckTimeout();
            int num11;
            int num12 = (num11 = runtextend - num10) + 1;
            while (--num12 > 0)
            {
              if (!RegexRunner.CharInClass(runtext[num10++], "\0\0\u0001d"))
              {
                --num10;
                break;
              }
            }
            if (num11 > num12)
            {
              int num13;
              runtrack[num13 = num4 - 1] = num11 - num12 - 1;
              int num14;
              runtrack[num14 = num13 - 1] = num10 - 1;
              runtrack[num4 = num14 - 1] = 3;
            }
          }
          else
            goto label_68;
        }
        else
          goto label_68;
      }
      else
        goto label_68;
label_17:
      this.CheckTimeout();
      runstack[--num3] = num10;
      runtrack[--num4] = 1;
      this.CheckTimeout();
      int end1;
      if (1 <= runtextend - num10)
      {
        end1 = num10 + 1;
        int num15 = 1;
        while (RegexRunner.CharInClass(runtext[end1 - num15--], "\0\0\n\0\u0002\u0004\u0005\u0003\u0001\u0006\t\u0013\0"))
        {
          if (num15 <= 0)
          {
            this.CheckTimeout();
            int num16;
            int num17 = (num16 = runtextend - end1) + 1;
            while (--num17 > 0)
            {
              if (!RegexRunner.CharInClass(runtext[end1++], "\0\0\n\0\u0002\u0004\u0005\u0003\u0001\u0006\t\u0013\0"))
              {
                --end1;
                break;
              }
            }
            if (num16 > num17)
            {
              int num18;
              runtrack[num18 = num4 - 1] = num16 - num17 - 1;
              int num19;
              runtrack[num19 = num18 - 1] = end1 - 1;
              runtrack[num4 = num19 - 1] = 4;
              goto label_27;
            }
            else
              goto label_27;
          }
        }
        goto label_68;
      }
      else
        goto label_68;
label_27:
      this.CheckTimeout();
      int start1 = runstack[num3++];
      this.Capture(1, start1, end1);
      int num20;
      runtrack[num20 = num4 - 1] = start1;
      runtrack[num4 = num20 - 1] = 5;
      this.CheckTimeout();
      int num21;
      int num22 = (num21 = runtextend - end1) + 1;
      while (--num22 > 0)
      {
        if (!RegexRunner.CharInClass(runtext[end1++], "\0\0\u0001d"))
        {
          --end1;
          break;
        }
      }
      if (num21 > num22)
      {
        int num23;
        runtrack[num23 = num4 - 1] = num21 - num22 - 1;
        int num24;
        runtrack[num24 = num23 - 1] = end1 - 1;
        runtrack[num4 = num24 - 1] = 6;
      }
label_33:
      this.CheckTimeout();
      if (end1 < runtextend)
      {
        string str = runtext;
        int index = end1;
        num25 = index + 1;
        if (str[index] == '=')
        {
          this.CheckTimeout();
          int num26;
          int num27 = (num26 = runtextend - num25) + 1;
          while (--num27 > 0)
          {
            if (!RegexRunner.CharInClass(runtext[num25++], "\0\0\u0001d"))
            {
              --num25;
              break;
            }
          }
          if (num26 > num27)
          {
            int num28;
            runtrack[num28 = num4 - 1] = num26 - num27 - 1;
            int num29;
            runtrack[num29 = num28 - 1] = num25 - 1;
            runtrack[num4 = num29 - 1] = 7;
          }
        }
        else
          goto label_68;
      }
      else
        goto label_68;
label_41:
      this.CheckTimeout();
      int num30 = runtextend - num25;
      if (num30 >= 1)
        num30 = 1;
      int num31 = num30;
      int num32 = num30 + 1;
      while (--num32 > 0)
      {
        if (!RegexRunner.CharInClass(runtext[num25++], "\0\u0004\0\"#'("))
        {
          --num25;
          break;
        }
      }
      if (num31 > num32)
      {
        int num33;
        runtrack[num33 = num4 - 1] = num31 - num32 - 1;
        int num34;
        runtrack[num34 = num33 - 1] = num25 - 1;
        runtrack[num4 = num34 - 1] = 8;
      }
label_49:
      this.CheckTimeout();
      runstack[--num3] = num25;
      runtrack[--num4] = 1;
      this.CheckTimeout();
      int num35;
      if ((num35 = runtextend - num25) > 0)
      {
        int num36;
        runtrack[num36 = num4 - 1] = num35 - 1;
        int num37;
        runtrack[num37 = num36 - 1] = num25;
        runtrack[num4 = num37 - 1] = 9;
      }
label_51:
      this.CheckTimeout();
      int start2 = runstack[num3++];
      this.Capture(2, start2, num25);
      int num38;
      runtrack[num38 = num4 - 1] = start2;
      runtrack[num4 = num38 - 1] = 5;
      this.CheckTimeout();
      int num39 = runtextend - num25;
      if (num39 >= 1)
        num39 = 1;
      int num40 = num39;
      int num41 = num39 + 1;
      while (--num41 > 0)
      {
        if (!RegexRunner.CharInClass(runtext[num25++], "\0\u0004\0\"#'("))
        {
          --num25;
          break;
        }
      }
      if (num40 > num41)
      {
        int num42;
        runtrack[num42 = num4 - 1] = num40 - num41 - 1;
        int num43;
        runtrack[num43 = num42 - 1] = num25 - 1;
        runtrack[num4 = num43 - 1] = 10;
      }
label_59:
      this.CheckTimeout();
      int num44;
      int num45 = (num44 = runtextend - num25) + 1;
      while (--num45 > 0)
      {
        if (!RegexRunner.CharInClass(runtext[num25++], "\0\0\u0001d"))
        {
          --num25;
          break;
        }
      }
      if (num44 > num45)
      {
        int num46;
        runtrack[num46 = num4 - 1] = num44 - num45 - 1;
        int num47;
        runtrack[num47 = num46 - 1] = num25 - 1;
        runtrack[num4 = num47 - 1] = 11;
      }
label_65:
      this.CheckTimeout();
      int end2;
      int num48;
      if (3 <= runtextend - num25 && runtext[num25] == '-' && runtext[num25 + 1] == '-' && runtext[num25 + 2] == '>')
      {
        end2 = num25 + 3;
        this.CheckTimeout();
        int[] numArray = runstack;
        int index = num3;
        int num49 = index + 1;
        int start3 = numArray[index];
        this.Capture(0, start3, end2);
        int num50;
        runtrack[num50 = num4 - 1] = start3;
        runtrack[num48 = num50 - 1] = 5;
      }
      else
        goto label_68;
label_67:
      this.CheckTimeout();
      this.runtextpos = end2;
      return;
label_68:
      int num51 = 0;
      string str1 = null;
      int index3 = 0;
      do
      {
        this.runtrackpos = num4;
        this.runstackpos = num3;
        this.EnsureStorage();
        int runtrackpos2 = this.runtrackpos;
        num3 = this.runstackpos;
        runtrack = this.runtrack;
        runstack = this.runstack;
        int[] numArray1 = runtrack;
        int index4 = runtrackpos2;
        num4 = index4 + 1;
        switch (numArray1[index4])
        {
          case 1:
            this.CheckTimeout();
            ++num3;
            continue;
          case 2:
            this.CheckTimeout();
            int[] numArray2 = runtrack;
            int index5 = num4;
            int num52 = index5 + 1;
            num5 = numArray2[index5];
            int[] numArray3 = runtrack;
            int index6 = num52;
            num4 = index6 + 1;
            int num53 = numArray3[index6];
            if (num53 > 0)
            {
              int num54;
              runtrack[num54 = num4 - 1] = num53 - 1;
              int num55;
              runtrack[num55 = num54 - 1] = num5 - 1;
              runtrack[num4 = num55 - 1] = 2;
              goto label_8;
            }
            else
              goto label_8;
          case 3:
            this.CheckTimeout();
            int[] numArray4 = runtrack;
            int index7 = num4;
            int num56 = index7 + 1;
            num10 = numArray4[index7];
            int[] numArray5 = runtrack;
            int index8 = num56;
            num4 = index8 + 1;
            int num57 = numArray5[index8];
            if (num57 > 0)
            {
              int num58;
              runtrack[num58 = num4 - 1] = num57 - 1;
              int num59;
              runtrack[num59 = num58 - 1] = num10 - 1;
              runtrack[num4 = num59 - 1] = 3;
              goto label_17;
            }
            else
              goto label_17;
          case 4:
            this.CheckTimeout();
            int[] numArray6 = runtrack;
            int index9 = num4;
            int num60 = index9 + 1;
            end1 = numArray6[index9];
            int[] numArray7 = runtrack;
            int index10 = num60;
            num4 = index10 + 1;
            int num61 = numArray7[index10];
            if (num61 > 0)
            {
              int num62;
              runtrack[num62 = num4 - 1] = num61 - 1;
              int num63;
              runtrack[num63 = num62 - 1] = end1 - 1;
              runtrack[num4 = num63 - 1] = 4;
              goto label_27;
            }
            else
              goto label_27;
          case 5:
            this.CheckTimeout();
            runstack[--num3] = runtrack[num4++];
            this.Uncapture();
            continue;
          case 6:
            this.CheckTimeout();
            int[] numArray8 = runtrack;
            int index11 = num4;
            int num64 = index11 + 1;
            end1 = numArray8[index11];
            int[] numArray9 = runtrack;
            int index12 = num64;
            num4 = index12 + 1;
            int num65 = numArray9[index12];
            if (num65 > 0)
            {
              int num66;
              runtrack[num66 = num4 - 1] = num65 - 1;
              int num67;
              runtrack[num67 = num66 - 1] = end1 - 1;
              runtrack[num4 = num67 - 1] = 6;
              goto label_33;
            }
            else
              goto label_33;
          case 7:
            this.CheckTimeout();
            int[] numArray10 = runtrack;
            int index13 = num4;
            int num68 = index13 + 1;
            num25 = numArray10[index13];
            int[] numArray11 = runtrack;
            int index14 = num68;
            num4 = index14 + 1;
            int num69 = numArray11[index14];
            if (num69 > 0)
            {
              int num70;
              runtrack[num70 = num4 - 1] = num69 - 1;
              int num71;
              runtrack[num71 = num70 - 1] = num25 - 1;
              runtrack[num4 = num71 - 1] = 7;
              goto label_41;
            }
            else
              goto label_41;
          case 8:
            this.CheckTimeout();
            int[] numArray12 = runtrack;
            int index15 = num4;
            int num72 = index15 + 1;
            num25 = numArray12[index15];
            int[] numArray13 = runtrack;
            int index16 = num72;
            num4 = index16 + 1;
            int num73 = numArray13[index16];
            if (num73 > 0)
            {
              int num74;
              runtrack[num74 = num4 - 1] = num73 - 1;
              int num75;
              runtrack[num75 = num74 - 1] = num25 - 1;
              runtrack[num4 = num75 - 1] = 8;
              goto label_49;
            }
            else
              goto label_49;
          case 9:
            this.CheckTimeout();
            int[] numArray14 = runtrack;
            int index17 = num4;
            int num76 = index17 + 1;
            int num77 = numArray14[index17];
            int[] numArray15 = runtrack;
            int index18 = num76;
            num4 = index18 + 1;
            num51 = numArray15[index18];
            str1 = runtext;
            index3 = num77;
            num25 = index3 + 1;
            continue;
          case 10:
            goto label_87;
          case 11:
            goto label_89;
          default:
            this.CheckTimeout();
            int[] numArray16 = runtrack;
            int index19 = num4;
            num48 = index19 + 1;
            end2 = numArray16[index19];
            goto label_67;
        }
      }
      while (!RegexRunner.CharInClass(str1[index3], "\u0001\u0004\0\"#'("));
      if (num51 > 0)
      {
        int num78;
        runtrack[num78 = num4 - 1] = num51 - 1;
        int num79;
        runtrack[num79 = num78 - 1] = num25;
        runtrack[num4 = num79 - 1] = 9;
        goto label_51;
      }
      else
        goto label_51;
label_87:
      this.CheckTimeout();
      int[] numArray17 = runtrack;
      int index20 = num4;
      int num80 = index20 + 1;
      num25 = numArray17[index20];
      int[] numArray18 = runtrack;
      int index21 = num80;
      num4 = index21 + 1;
      int num81 = numArray18[index21];
      if (num81 > 0)
      {
        int num82;
        runtrack[num82 = num4 - 1] = num81 - 1;
        int num83;
        runtrack[num83 = num82 - 1] = num25 - 1;
        runtrack[num4 = num83 - 1] = 10;
        goto label_59;
      }
      else
        goto label_59;
label_89:
      this.CheckTimeout();
      int[] numArray19 = runtrack;
      int index22 = num4;
      int num84 = index22 + 1;
      num25 = numArray19[index22];
      int[] numArray20 = runtrack;
      int index23 = num84;
      num4 = index23 + 1;
      int num85 = numArray20[index23];
      if (num85 > 0)
      {
        int num86;
        runtrack[num86 = num4 - 1] = num85 - 1;
        int num87;
        runtrack[num87 = num86 - 1] = num25 - 1;
        runtrack[num4 = num87 - 1] = 11;
        goto label_65;
      }
      else
        goto label_65;
    }

    protected override bool FindFirstChar()
    {
      if (this.runtextpos <= this.runtextstart)
        return true;
      this.runtextpos = this.runtextend;
      return false;
    }

    protected override void InitTrackCount() => this.runtrackcount = 16;
  }
}

#endif