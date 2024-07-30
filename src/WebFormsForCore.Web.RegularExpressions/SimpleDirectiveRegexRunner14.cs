// Decompiled with JetBrains decompiler
// Type: System.Web.RegularExpressions.SimpleDirectiveRegexRunner14
// Assembly: System.Web.RegularExpressions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2D011335-7300-40B5-8CDC-1E3EA0A75C61
// Assembly location: C:\Windows\Microsoft.NET\assembly\GAC_MSIL\System.Web.RegularExpressions\v4.0_4.0.0.0__b03f5f7f11d50a3a\System.Web.RegularExpressions.dll
// XML documentation location: C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.8.1\System.Web.RegularExpressions.xml

#if NETFRAMEWORK

using System.Text.RegularExpressions;

#nullable disable
namespace System.Web.RegularExpressions
{
  internal class SimpleDirectiveRegexRunner14 : RegexRunner
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
      if (2 <= runtextend - num1 && runtext[num1] == '<' && runtext[num1 + 1] == '%')
      {
        num1 += 2;
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
        goto label_97;
label_7:
      this.CheckTimeout();
      int num10;
      int num11;
      if (num1 < runtextend && runtext[num1++] == '@')
      {
        this.CheckTimeout();
        runstack[num10 = num4 - 1] = -1;
        runtrack[num11 = num5 - 1] = 1;
        this.CheckTimeout();
        goto label_88;
      }
      else
        goto label_97;
label_9:
      this.CheckTimeout();
      runstack[--num4] = num1;
      int num12;
      runtrack[num12 = num5 - 1] = 1;
      this.CheckTimeout();
      int num13;
      int num14 = (num13 = runtextend - num1) + 1;
      while (--num14 > 0)
      {
        if (!RegexRunner.CharInClass(runtext[num1++], "\0\0\u0001d"))
        {
          --num1;
          break;
        }
      }
      if (num13 > num14)
      {
        int num15;
        runtrack[num15 = num12 - 1] = num13 - num14 - 1;
        int num16;
        runtrack[num16 = num15 - 1] = num1 - 1;
        runtrack[num12 = num16 - 1] = 3;
      }
label_15:
      this.CheckTimeout();
      runstack[--num4] = num1;
      runtrack[num5 = num12 - 1] = 1;
      this.CheckTimeout();
      if (num1 < runtextend && RegexRunner.CharInClass(runtext[num1++], "\0\0\n\0\u0002\u0004\u0005\u0003\u0001\u0006\t\u0013\0"))
      {
        this.CheckTimeout();
        int num17;
        int num18 = (num17 = runtextend - num1) + 1;
        while (--num18 > 0)
        {
          if (!RegexRunner.CharInClass(runtext[num1++], "\0\u0002\n:;\0\u0002\u0004\u0005\u0003\u0001\u0006\t\u0013\0"))
          {
            --num1;
            break;
          }
        }
        if (num17 > num18)
        {
          int num19;
          runtrack[num19 = num5 - 1] = num17 - num18 - 1;
          int num20;
          runtrack[num20 = num19 - 1] = num1 - 1;
          runtrack[num5 = num20 - 1] = 4;
        }
      }
      else
        goto label_97;
label_22:
      this.CheckTimeout();
      int num21;
      runstack[num21 = num4 - 1] = this.runtrack.Length - num5;
      int num22;
      runstack[num22 = num21 - 1] = this.Crawlpos();
      int num23;
      runtrack[num23 = num5 - 1] = 5;
      this.CheckTimeout();
      runstack[num4 = num22 - 1] = num1;
      runtrack[num5 = num23 - 1] = 1;
      this.CheckTimeout();
      int num24;
      int num25;
      if (num1 < runtextend && RegexRunner.CharInClass(runtext[num1++], "\u0001\0\n\0\u0002\u0004\u0005\u0003\u0001\u0006\t\u0013\0"))
      {
        this.CheckTimeout();
        int[] numArray1 = runtrack;
        int index1;
        int num26 = index1 = num5 - 1;
        int[] numArray2 = runstack;
        int index2 = num4;
        int num27 = index2 + 1;
        int num28;
        num1 = num28 = numArray2[index2];
        numArray1[index1] = num28;
        runtrack[num24 = num26 - 1] = 6;
        this.CheckTimeout();
        int[] numArray3 = runstack;
        int index3 = num27;
        int num29 = index3 + 1;
        int num30 = numArray3[index3];
        int length = this.runtrack.Length;
        int[] numArray4 = runstack;
        int index4 = num29;
        int num31 = index4 + 1;
        int num32 = numArray4[index4];
        int num33 = length - num32;
        int num34;
        runtrack[num34 = num33 - 1] = num30;
        int num35;
        runtrack[num35 = num34 - 1] = 7;
        this.CheckTimeout();
        int[] numArray5 = runstack;
        int index5 = num31;
        int num36 = index5 + 1;
        int start = numArray5[index5];
        this.Capture(3, start, num1);
        int num37;
        runtrack[num37 = num35 - 1] = start;
        int num38;
        runtrack[num38 = num37 - 1] = 8;
        this.CheckTimeout();
        runstack[num4 = num36 - 1] = num1;
        int num39;
        runtrack[num39 = num38 - 1] = 1;
        this.CheckTimeout();
        int num40;
        runtrack[num40 = num39 - 1] = num1;
        runtrack[num25 = num40 - 1] = 9;
        this.CheckTimeout();
        int num41;
        int num42 = (num41 = runtextend - num1) + 1;
        while (--num42 > 0)
        {
          if (!RegexRunner.CharInClass(runtext[num1++], "\0\0\u0001d"))
          {
            --num1;
            break;
          }
        }
        if (num41 > num42)
        {
          int num43;
          runtrack[num43 = num25 - 1] = num41 - num42 - 1;
          int num44;
          runtrack[num44 = num43 - 1] = num1 - 1;
          runtrack[num25 = num44 - 1] = 10;
        }
      }
      else
        goto label_97;
label_29:
      this.CheckTimeout();
      runstack[--num4] = num1;
      runtrack[num5 = num25 - 1] = 1;
      this.CheckTimeout();
      if (num1 < runtextend && runtext[num1++] == '=')
      {
        this.CheckTimeout();
        int start = runstack[num4++];
        this.Capture(4, start, num1);
        int num45;
        runtrack[num45 = num5 - 1] = start;
        runtrack[num5 = num45 - 1] = 8;
        this.CheckTimeout();
        int num46;
        int num47 = (num46 = runtextend - num1) + 1;
        while (--num47 > 0)
        {
          if (!RegexRunner.CharInClass(runtext[num1++], "\0\0\u0001d"))
          {
            --num1;
            break;
          }
        }
        if (num46 > num47)
        {
          int num48;
          runtrack[num48 = num5 - 1] = num46 - num47 - 1;
          int num49;
          runtrack[num49 = num48 - 1] = num1 - 1;
          runtrack[num5 = num49 - 1] = 11;
        }
      }
      else
        goto label_97;
label_36:
      this.CheckTimeout();
      int num50;
      if (num1 < runtextend && runtext[num1++] == '"')
      {
        this.CheckTimeout();
        runstack[--num4] = num1;
        runtrack[num50 = num5 - 1] = 1;
        this.CheckTimeout();
        int num51;
        int num52 = (num51 = runtextend - num1) + 1;
        while (--num52 > 0)
        {
          if (runtext[num1++] == '"')
          {
            --num1;
            break;
          }
        }
        if (num51 > num52)
        {
          int num53;
          runtrack[num53 = num50 - 1] = num51 - num52 - 1;
          int num54;
          runtrack[num54 = num53 - 1] = num1 - 1;
          runtrack[num50 = num54 - 1] = 12;
        }
      }
      else
        goto label_97;
label_43:
      this.CheckTimeout();
      int start1 = runstack[num4++];
      this.Capture(5, start1, num1);
      int num55;
      runtrack[num55 = num50 - 1] = start1;
      runtrack[num5 = num55 - 1] = 8;
      this.CheckTimeout();
      if (num1 < runtextend && runtext[num1++] == '"')
        this.CheckTimeout();
      else
        goto label_97;
label_87:
      this.CheckTimeout();
      int[] numArray6 = runstack;
      int index6 = num4;
      int num56 = index6 + 1;
      int start2 = numArray6[index6];
      this.Capture(2, start2, num1);
      int num57;
      runtrack[num57 = num5 - 1] = start2;
      int num58;
      runtrack[num58 = num57 - 1] = 8;
      this.CheckTimeout();
      int[] numArray7 = runstack;
      int index7 = num56;
      num10 = index7 + 1;
      int start3 = numArray7[index7];
      this.Capture(1, start3, num1);
      int num59;
      runtrack[num59 = num58 - 1] = start3;
      runtrack[num11 = num59 - 1] = 8;
label_88:
      this.CheckTimeout();
      int[] numArray8 = runstack;
      int index8 = num10;
      num4 = index8 + 1;
      int num60;
      int num61 = num60 = numArray8[index8];
      int num62;
      runtrack[num62 = num11 - 1] = num61;
      int num63 = num1;
      if (num60 != num63)
      {
        int num64;
        runtrack[num64 = num62 - 1] = num1;
        runstack[--num4] = num1;
        runtrack[num5 = num64 - 1] = 22;
        if (num5 <= 204 || num4 <= 153)
        {
          runtrack[--num5] = 23;
          goto label_97;
        }
        else
          goto label_9;
      }
      else
        runtrack[num5 = num62 - 1] = 24;
label_92:
      this.CheckTimeout();
      int num65;
      if ((num65 = runtextend - num1) > 0)
      {
        int num66;
        runtrack[num66 = num5 - 1] = num65 - 1;
        int num67;
        runtrack[num67 = num66 - 1] = num1;
        runtrack[num5 = num67 - 1] = 25;
      }
label_94:
      this.CheckTimeout();
      int end;
      if (2 <= runtextend - num1 && runtext[num1] == '%' && runtext[num1 + 1] == '>')
      {
        end = num1 + 2;
        this.CheckTimeout();
        int[] numArray9 = runstack;
        int index9 = num4;
        int num68 = index9 + 1;
        int start4 = numArray9[index9];
        this.Capture(0, start4, end);
        int num69;
        runtrack[num69 = num5 - 1] = start4;
        runtrack[num24 = num69 - 1] = 8;
      }
      else
        goto label_97;
label_96:
      this.CheckTimeout();
      this.runtextpos = end;
      return;
label_97:
      int num70;
      int num71;
      int num72;
      int num73 = 0;
      string str1 = null;
      int index10 = 0;
      do
      {
        int num74 = 0;
        string str2 = null;
        int index11 = 0;
        do
        {
          do
          {
            do
            {
              this.runtrackpos = num5;
              this.runstackpos = num4;
              this.EnsureStorage();
              int runtrackpos2 = this.runtrackpos;
              num4 = this.runstackpos;
              runtrack = this.runtrack;
              runstack = this.runstack;
              int[] numArray10 = runtrack;
              int index12 = runtrackpos2;
              num5 = index12 + 1;
              int num75;
              int num76;
              switch (numArray10[index12])
              {
                case 1:
                  goto label_99;
                case 2:
                  goto label_100;
                case 3:
                  goto label_102;
                case 4:
                  goto label_104;
                case 5:
                  goto label_106;
                case 6:
                  goto label_107;
                case 7:
                  goto label_108;
                case 8:
                  goto label_111;
                case 9:
                  this.CheckTimeout();
                  int[] numArray11 = runtrack;
                  int index13 = num5;
                  int num77 = index13 + 1;
                  num1 = numArray11[index13];
                  this.CheckTimeout();
                  int num78;
                  runtrack[num78 = num77 - 1] = num1;
                  runtrack[num75 = num78 - 1] = 13;
                  this.CheckTimeout();
                  int num79;
                  int num80 = (num79 = runtextend - num1) + 1;
                  while (--num80 > 0)
                  {
                    if (!RegexRunner.CharInClass(runtext[num1++], "\0\0\u0001d"))
                    {
                      --num1;
                      break;
                    }
                  }
                  if (num79 > num80)
                  {
                    int num81;
                    runtrack[num81 = num75 - 1] = num79 - num80 - 1;
                    int num82;
                    runtrack[num82 = num81 - 1] = num1 - 1;
                    runtrack[num75 = num82 - 1] = 14;
                    break;
                  }
                  break;
                case 10:
                  goto label_113;
                case 11:
                  goto label_115;
                case 12:
                  goto label_117;
                case 13:
                  goto label_119;
                case 14:
                  this.CheckTimeout();
                  int[] numArray12 = runtrack;
                  int index14 = num5;
                  int num83 = index14 + 1;
                  num1 = numArray12[index14];
                  int[] numArray13 = runtrack;
                  int index15 = num83;
                  num75 = index15 + 1;
                  int num84 = numArray13[index15];
                  if (num84 > 0)
                  {
                    int num85;
                    runtrack[num85 = num75 - 1] = num84 - 1;
                    int num86;
                    runtrack[num86 = num85 - 1] = num1 - 1;
                    runtrack[num75 = num86 - 1] = 14;
                    break;
                  }
                  break;
                case 15:
                  this.CheckTimeout();
                  int[] numArray14 = runtrack;
                  int index16 = num5;
                  int num87 = index16 + 1;
                  num1 = numArray14[index16];
                  int[] numArray15 = runtrack;
                  int index17 = num87;
                  num5 = index17 + 1;
                  int num88 = numArray15[index17];
                  if (num88 > 0)
                  {
                    int num89;
                    runtrack[num89 = num5 - 1] = num88 - 1;
                    int num90;
                    runtrack[num90 = num89 - 1] = num1 - 1;
                    runtrack[num5 = num90 - 1] = 15;
                    goto label_57;
                  }
                  else
                    goto label_57;
                case 16:
                  this.CheckTimeout();
                  int[] numArray16 = runtrack;
                  int index18 = num5;
                  int num91 = index18 + 1;
                  num1 = numArray16[index18];
                  int[] numArray17 = runtrack;
                  int index19 = num91;
                  num76 = index19 + 1;
                  int num92 = numArray17[index19];
                  if (num92 > 0)
                  {
                    int num93;
                    runtrack[num93 = num76 - 1] = num92 - 1;
                    int num94;
                    runtrack[num94 = num93 - 1] = num1 - 1;
                    runtrack[num76 = num94 - 1] = 16;
                    goto label_64;
                  }
                  else
                    goto label_64;
                case 17:
                  goto label_126;
                case 18:
                  goto label_127;
                case 19:
                  goto label_129;
                case 20:
                  goto label_131;
                case 21:
                  goto label_133;
                case 22:
                  goto label_136;
                case 23:
                  goto label_9;
                case 24:
                  goto label_137;
                case 25:
                  goto label_138;
                default:
                  goto label_98;
              }
              this.CheckTimeout();
              runstack[--num4] = num1;
              runtrack[num5 = num75 - 1] = 1;
              this.CheckTimeout();
              if (num1 < runtextend && runtext[num1++] == '=')
              {
                this.CheckTimeout();
                int start5 = runstack[num4++];
                this.Capture(4, start5, num1);
                int num95;
                runtrack[num95 = num5 - 1] = start5;
                runtrack[num5 = num95 - 1] = 8;
                this.CheckTimeout();
                int num96;
                int num97 = (num96 = runtextend - num1) + 1;
                while (--num97 > 0)
                {
                  if (!RegexRunner.CharInClass(runtext[num1++], "\0\0\u0001d"))
                  {
                    --num1;
                    break;
                  }
                }
                if (num96 > num97)
                {
                  int num98;
                  runtrack[num98 = num5 - 1] = num96 - num97 - 1;
                  int num99;
                  runtrack[num99 = num98 - 1] = num1 - 1;
                  runtrack[num5 = num99 - 1] = 15;
                }
              }
              else
                continue;
label_57:
              this.CheckTimeout();
              if (num1 < runtextend && runtext[num1++] == '\'')
              {
                this.CheckTimeout();
                runstack[--num4] = num1;
                runtrack[num76 = num5 - 1] = 1;
                this.CheckTimeout();
                int num100;
                int num101 = (num100 = runtextend - num1) + 1;
                while (--num101 > 0)
                {
                  if (runtext[num1++] == '\'')
                  {
                    --num1;
                    break;
                  }
                }
                if (num100 > num101)
                {
                  int num102;
                  runtrack[num102 = num76 - 1] = num100 - num101 - 1;
                  int num103;
                  runtrack[num103 = num102 - 1] = num1 - 1;
                  runtrack[num76 = num103 - 1] = 16;
                }
              }
              else
                continue;
label_64:
              this.CheckTimeout();
              int start6 = runstack[num4++];
              this.Capture(5, start6, num1);
              int num104;
              runtrack[num104 = num76 - 1] = start6;
              runtrack[num5 = num104 - 1] = 8;
              this.CheckTimeout();
            }
            while (num1 >= runtextend || runtext[num1++] != '\'');
            goto label_65;
label_71:
            this.CheckTimeout();
            runstack[--num4] = num1;
            int num105;
            runtrack[num5 = num105 - 1] = 1;
            this.CheckTimeout();
            continue;
label_119:
            this.CheckTimeout();
            int[] numArray18 = runtrack;
            int index20 = num5;
            int num106 = index20 + 1;
            num1 = numArray18[index20];
            this.CheckTimeout();
            int num107;
            runtrack[num107 = num106 - 1] = num1;
            runtrack[num105 = num107 - 1] = 17;
            this.CheckTimeout();
            int num108;
            int num109 = (num108 = runtextend - num1) + 1;
            while (--num109 > 0)
            {
              if (!RegexRunner.CharInClass(runtext[num1++], "\0\0\u0001d"))
              {
                --num1;
                break;
              }
            }
            if (num108 > num109)
            {
              int num110;
              runtrack[num110 = num105 - 1] = num108 - num109 - 1;
              int num111;
              runtrack[num111 = num110 - 1] = num1 - 1;
              runtrack[num105 = num111 - 1] = 18;
              goto label_71;
            }
            else
              goto label_71;
label_127:
            this.CheckTimeout();
            int[] numArray19 = runtrack;
            int index21 = num5;
            int num112 = index21 + 1;
            num1 = numArray19[index21];
            int[] numArray20 = runtrack;
            int index22 = num112;
            num105 = index22 + 1;
            int num113 = numArray20[index22];
            if (num113 > 0)
            {
              int num114;
              runtrack[num114 = num105 - 1] = num113 - 1;
              int num115;
              runtrack[num115 = num114 - 1] = num1 - 1;
              runtrack[num105 = num115 - 1] = 18;
              goto label_71;
            }
            else
              goto label_71;
          }
          while (num1 >= runtextend || runtext[num1++] != '=');
          goto label_72;
label_98:
          this.CheckTimeout();
          int[] numArray21 = runtrack;
          int index23 = num5;
          num24 = index23 + 1;
          end = numArray21[index23];
          goto label_96;
label_99:
          this.CheckTimeout();
          ++num4;
          continue;
label_100:
          this.CheckTimeout();
          int[] numArray22 = runtrack;
          int index24 = num5;
          int num116 = index24 + 1;
          num1 = numArray22[index24];
          int[] numArray23 = runtrack;
          int index25 = num116;
          num5 = index25 + 1;
          int num117 = numArray23[index25];
          if (num117 > 0)
          {
            int num118;
            runtrack[num118 = num5 - 1] = num117 - 1;
            int num119;
            runtrack[num119 = num118 - 1] = num1 - 1;
            runtrack[num5 = num119 - 1] = 2;
            goto label_7;
          }
          else
            goto label_7;
label_102:
          this.CheckTimeout();
          int[] numArray24 = runtrack;
          int index26 = num5;
          int num120 = index26 + 1;
          num1 = numArray24[index26];
          int[] numArray25 = runtrack;
          int index27 = num120;
          num12 = index27 + 1;
          int num121 = numArray25[index27];
          if (num121 > 0)
          {
            int num122;
            runtrack[num122 = num12 - 1] = num121 - 1;
            int num123;
            runtrack[num123 = num122 - 1] = num1 - 1;
            runtrack[num12 = num123 - 1] = 3;
            goto label_15;
          }
          else
            goto label_15;
label_104:
          this.CheckTimeout();
          int[] numArray26 = runtrack;
          int index28 = num5;
          int num124 = index28 + 1;
          num1 = numArray26[index28];
          int[] numArray27 = runtrack;
          int index29 = num124;
          num5 = index29 + 1;
          int num125 = numArray27[index29];
          if (num125 > 0)
          {
            int num126;
            runtrack[num126 = num5 - 1] = num125 - 1;
            int num127;
            runtrack[num127 = num126 - 1] = num1 - 1;
            runtrack[num5 = num127 - 1] = 4;
            goto label_22;
          }
          else
            goto label_22;
label_106:
          this.CheckTimeout();
          num4 += 2;
          continue;
label_107:
          this.CheckTimeout();
          runstack[--num4] = runtrack[num5++];
          continue;
label_108:
          this.CheckTimeout();
          int num128 = runtrack[num5++];
          if (num128 != this.Crawlpos())
          {
            do
            {
              this.Uncapture();
            }
            while (num128 != this.Crawlpos());
          }
          continue;
label_111:
          this.CheckTimeout();
          runstack[--num4] = runtrack[num5++];
          this.Uncapture();
          continue;
label_113:
          this.CheckTimeout();
          int[] numArray28 = runtrack;
          int index30 = num5;
          int num129 = index30 + 1;
          num1 = numArray28[index30];
          int[] numArray29 = runtrack;
          int index31 = num129;
          num25 = index31 + 1;
          int num130 = numArray29[index31];
          if (num130 > 0)
          {
            int num131;
            runtrack[num131 = num25 - 1] = num130 - 1;
            int num132;
            runtrack[num132 = num131 - 1] = num1 - 1;
            runtrack[num25 = num132 - 1] = 10;
            goto label_29;
          }
          else
            goto label_29;
label_115:
          this.CheckTimeout();
          int[] numArray30 = runtrack;
          int index32 = num5;
          int num133 = index32 + 1;
          num1 = numArray30[index32];
          int[] numArray31 = runtrack;
          int index33 = num133;
          num5 = index33 + 1;
          int num134 = numArray31[index33];
          if (num134 > 0)
          {
            int num135;
            runtrack[num135 = num5 - 1] = num134 - 1;
            int num136;
            runtrack[num136 = num135 - 1] = num1 - 1;
            runtrack[num5 = num136 - 1] = 11;
            goto label_36;
          }
          else
            goto label_36;
label_117:
          this.CheckTimeout();
          int[] numArray32 = runtrack;
          int index34 = num5;
          int num137 = index34 + 1;
          num1 = numArray32[index34];
          int[] numArray33 = runtrack;
          int index35 = num137;
          num50 = index35 + 1;
          int num138 = numArray33[index35];
          if (num138 > 0)
          {
            int num139;
            runtrack[num139 = num50 - 1] = num138 - 1;
            int num140;
            runtrack[num140 = num139 - 1] = num1 - 1;
            runtrack[num50 = num140 - 1] = 12;
            goto label_43;
          }
          else
            goto label_43;
label_126:
          this.CheckTimeout();
          int[] numArray34 = runtrack;
          int index36 = num5;
          int num141 = index36 + 1;
          num1 = numArray34[index36];
          this.CheckTimeout();
          int num142;
          runstack[num142 = num4 - 1] = num1;
          int num143;
          runtrack[num143 = num141 - 1] = 1;
          this.CheckTimeout();
          int[] numArray35 = runstack;
          int index37 = num142;
          int num144 = index37 + 1;
          int start7 = numArray35[index37];
          this.Capture(4, start7, num1);
          int num145;
          runtrack[num145 = num143 - 1] = start7;
          int num146;
          runtrack[num146 = num145 - 1] = 8;
          this.CheckTimeout();
          runstack[num4 = num144 - 1] = num1;
          runtrack[num5 = num146 - 1] = 1;
          this.CheckTimeout();
          if ((num72 = runtextend - num1) <= 0)
            goto label_86;
          else
            goto label_85;
label_129:
          this.CheckTimeout();
          int[] numArray36 = runtrack;
          int index38 = num5;
          int num147 = index38 + 1;
          num1 = numArray36[index38];
          int[] numArray37 = runtrack;
          int index39 = num147;
          num70 = index39 + 1;
          int num148 = numArray37[index39];
          if (num148 > 0)
          {
            int num149;
            runtrack[num149 = num70 - 1] = num148 - 1;
            int num150;
            runtrack[num150 = num149 - 1] = num1 - 1;
            runtrack[num70 = num150 - 1] = 19;
            goto label_78;
          }
          else
            goto label_78;
label_131:
          this.CheckTimeout();
          int[] numArray38 = runtrack;
          int index40 = num5;
          int num151 = index40 + 1;
          num1 = numArray38[index40];
          int[] numArray39 = runtrack;
          int index41 = num151;
          num71 = index41 + 1;
          int num152 = numArray39[index41];
          if (num152 > 0)
          {
            int num153;
            runtrack[num153 = num71 - 1] = num152 - 1;
            int num154;
            runtrack[num154 = num153 - 1] = num1 - 1;
            runtrack[num71 = num154 - 1] = 20;
            goto label_84;
          }
          else
            goto label_84;
label_133:
          this.CheckTimeout();
          int[] numArray40 = runtrack;
          int index42 = num5;
          int num155 = index42 + 1;
          int num156 = numArray40[index42];
          int[] numArray41 = runtrack;
          int index43 = num155;
          num5 = index43 + 1;
          num74 = numArray41[index43];
          str2 = runtext;
          index11 = num156;
          num1 = index11 + 1;
        }
        while (!RegexRunner.CharInClass(str2[index11], "\0\0\u0001d"));
        if (num74 > 0)
        {
          int num157;
          runtrack[num157 = num5 - 1] = num74 - 1;
          int num158;
          runtrack[num158 = num157 - 1] = num1;
          runtrack[num5 = num158 - 1] = 21;
          goto label_86;
        }
        else
          goto label_86;
label_136:
        this.CheckTimeout();
        int[] numArray42 = runtrack;
        int index44 = num5;
        int num159 = index44 + 1;
        num1 = numArray42[index44];
        int num160 = runstack[num4++];
        runtrack[num5 = num159 - 1] = 24;
        goto label_92;
label_137:
        this.CheckTimeout();
        runstack[--num4] = runtrack[num5++];
        continue;
label_138:
        this.CheckTimeout();
        int[] numArray43 = runtrack;
        int index45 = num5;
        int num161 = index45 + 1;
        int num162 = numArray43[index45];
        int[] numArray44 = runtrack;
        int index46 = num161;
        num5 = index46 + 1;
        num73 = numArray44[index46];
        str1 = runtext;
        index10 = num162;
        num1 = index10 + 1;
      }
      while (!RegexRunner.CharInClass(str1[index10], "\0\0\u0001d"));
      goto label_139;
label_65:
      this.CheckTimeout();
      goto label_87;
label_72:
      this.CheckTimeout();
      int start8 = runstack[num4++];
      this.Capture(4, start8, num1);
      int num163;
      runtrack[num163 = num5 - 1] = start8;
      runtrack[num70 = num163 - 1] = 8;
      this.CheckTimeout();
      int num164;
      int num165 = (num164 = runtextend - num1) + 1;
      while (--num165 > 0)
      {
        if (!RegexRunner.CharInClass(runtext[num1++], "\0\0\u0001d"))
        {
          --num1;
          break;
        }
      }
      if (num164 > num165)
      {
        int num166;
        runtrack[num166 = num70 - 1] = num164 - num165 - 1;
        int num167;
        runtrack[num167 = num166 - 1] = num1 - 1;
        runtrack[num70 = num167 - 1] = 19;
      }
label_78:
      this.CheckTimeout();
      runstack[--num4] = num1;
      runtrack[num71 = num70 - 1] = 1;
      this.CheckTimeout();
      int num168;
      int num169 = (num168 = runtextend - num1) + 1;
      while (--num169 > 0)
      {
        if (!RegexRunner.CharInClass(runtext[num1++], "\u0001\b\u0001\"#%&'(>?d"))
        {
          --num1;
          break;
        }
      }
      if (num168 > num169)
      {
        int num170;
        runtrack[num170 = num71 - 1] = num168 - num169 - 1;
        int num171;
        runtrack[num171 = num170 - 1] = num1 - 1;
        runtrack[num71 = num171 - 1] = 20;
      }
label_84:
      this.CheckTimeout();
      int start9 = runstack[num4++];
      this.Capture(5, start9, num1);
      int num172;
      runtrack[num172 = num71 - 1] = start9;
      runtrack[num5 = num172 - 1] = 8;
      this.CheckTimeout();
      goto label_87;
label_85:
      int num173;
      runtrack[num173 = num5 - 1] = num72 - 1;
      int num174;
      runtrack[num174 = num173 - 1] = num1;
      runtrack[num5 = num174 - 1] = 21;
label_86:
      this.CheckTimeout();
      int start10 = runstack[num4++];
      this.Capture(5, start10, num1);
      int num175;
      runtrack[num175 = num5 - 1] = start10;
      runtrack[num5 = num175 - 1] = 8;
      goto label_87;
label_139:
      if (num73 > 0)
      {
        int num176;
        runtrack[num176 = num5 - 1] = num73 - 1;
        int num177;
        runtrack[num177 = num176 - 1] = num1;
        runtrack[num5 = num177 - 1] = 25;
        goto label_94;
      }
      else
        goto label_94;
    }

    protected override bool FindFirstChar()
    {
      string runtext = this.runtext;
      int runtextend = this.runtextend;
      int num1;
      int num2;
      for (int index = this.runtextpos + 1; index < runtextend; index = num1 + num2)
      {
        int num3;
        if ((num3 = (int) runtext[index]) == 37)
          goto label_31;
        else
          goto label_5;
label_2:
        num2 = index;
        continue;
label_5:
        int num4;
        if ((uint) (num4 = num3 - 37) > 23U)
        {
          num1 = 2;
          goto label_2;
        }
        else
        {
          switch (num4)
          {
            case 1:
              num1 = 2;
              goto label_2;
            case 2:
              num1 = 2;
              goto label_2;
            case 3:
              num1 = 2;
              goto label_2;
            case 4:
              num1 = 2;
              goto label_2;
            case 5:
              num1 = 2;
              goto label_2;
            case 6:
              num1 = 2;
              goto label_2;
            case 7:
              num1 = 2;
              goto label_2;
            case 8:
              num1 = 2;
              goto label_2;
            case 9:
              num1 = 2;
              goto label_2;
            case 10:
              num1 = 2;
              goto label_2;
            case 11:
              num1 = 2;
              goto label_2;
            case 12:
              num1 = 2;
              goto label_2;
            case 13:
              num1 = 2;
              goto label_2;
            case 14:
              num1 = 2;
              goto label_2;
            case 15:
              num1 = 2;
              goto label_2;
            case 16:
              num1 = 2;
              goto label_2;
            case 17:
              num1 = 2;
              goto label_2;
            case 18:
              num1 = 2;
              goto label_2;
            case 19:
              num1 = 2;
              goto label_2;
            case 20:
              num1 = 2;
              goto label_2;
            case 21:
              num1 = 2;
              goto label_2;
            case 22:
              num1 = 2;
              goto label_2;
            case 23:
              num1 = 1;
              goto label_2;
            default:
              num1 = 0;
              goto label_2;
          }
        }
label_31:
        int num5 = index;
        int num6;
        if (runtext[num6 = num5 - 1] != '<')
        {
          num1 = 1;
          goto label_2;
        }
        else
        {
          this.runtextpos = num6;
          return true;
        }
      }
      this.runtextpos = this.runtextend;
      return false;
    }

    protected override void InitTrackCount() => this.runtrackcount = 51;
  }
}
#endif