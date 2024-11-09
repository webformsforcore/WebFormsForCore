#if NETFRAMEWORK

using System.Text.RegularExpressions;

#nullable disable
namespace System.Web.RegularExpressions
{
  internal class BindParametersRegexRunner18 : RegexRunner
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
      while (true)
      {
        int num9;
        do
        {
          this.CheckTimeout();
          int num10;
          runstack[num10 = num3 - 1] = runtextpos;
          int num11;
          runtrack[num11 = num4 - 1] = 1;
          this.CheckTimeout();
          int num12;
          runtrack[num12 = num11 - 1] = runtextpos;
          int num13;
          runtrack[num13 = num12 - 1] = 3;
          this.CheckTimeout();
          runstack[num3 = num10 - 1] = runtextpos;
          int num14;
          runtrack[num14 = num13 - 1] = 1;
          this.CheckTimeout();
          if (runtextpos < runtextend && runtext[runtextpos++] == '"')
          {
            this.CheckTimeout();
            int num15;
            runstack[num15 = num3 - 1] = runtextpos;
            int num16;
            runtrack[num16 = num14 - 1] = 1;
            this.CheckTimeout();
            int num17;
            runstack[num17 = num15 - 1] = runtextpos;
            int num18;
            runtrack[num18 = num16 - 1] = 1;
            this.CheckTimeout();
            int num19;
            runtrack[num19 = num18 - 1] = runtextpos;
            int num20;
            runtrack[num20 = num19 - 1] = 4;
            this.CheckTimeout();
            runstack[num3 = num17 - 1] = runtextpos;
            runtrack[num14 = num20 - 1] = 1;
            this.CheckTimeout();
            if (1 <= runtextend - runtextpos)
            {
              ++runtextpos;
              int num21 = 1;
              while (RegexRunner.CharInClass(runtext[runtextpos - num21--], "\0\u0002\n./\0\u0002\u0004\u0005\u0003\u0001\u0006\t\u0013\0"))
              {
                if (num21 <= 0)
                {
                  this.CheckTimeout();
                  int num22;
                  int num23 = (num22 = runtextend - runtextpos) + 1;
                  while (--num23 > 0)
                  {
                    if (!RegexRunner.CharInClass(runtext[runtextpos++], "\0\u0002\n./\0\u0002\u0004\u0005\u0003\u0001\u0006\t\u0013\0"))
                    {
                      --runtextpos;
                      break;
                    }
                  }
                  if (num22 > num23)
                  {
                    int num24;
                    runtrack[num24 = num14 - 1] = num22 - num23 - 1;
                    int num25;
                    runtrack[num25 = num24 - 1] = runtextpos - 1;
                    runtrack[num14 = num25 - 1] = 5;
                    goto label_17;
                  }
                  else
                    goto label_17;
                }
              }
              goto label_108;
            }
            else
              goto label_108;
          }
          else
            goto label_108;
label_17:
          this.CheckTimeout();
          int[] numArray1 = runstack;
          int index1 = num3;
          int num26 = index1 + 1;
          int start1 = numArray1[index1];
          this.Capture(4, start1, runtextpos);
          int num27;
          runtrack[num27 = num14 - 1] = start1;
          int num28;
          runtrack[num28 = num27 - 1] = 6;
          this.CheckTimeout();
label_30:
          this.CheckTimeout();
          int[] numArray2 = runstack;
          int index2 = num26;
          int num29 = index2 + 1;
          int start2 = numArray2[index2];
          this.Capture(3, start2, runtextpos);
          int num30;
          runtrack[num30 = num28 - 1] = start2;
          int num31;
          runtrack[num31 = num30 - 1] = 6;
          this.CheckTimeout();
          int[] numArray3 = runstack;
          int index3 = num29;
          num3 = index3 + 1;
          int start3 = numArray3[index3];
          this.Capture(14, start3, runtextpos);
          int num32;
          runtrack[num32 = num31 - 1] = start3;
          runtrack[num14 = num32 - 1] = 6;
          this.CheckTimeout();
          int num33;
          int num34;
          if (runtextpos < runtextend && runtext[runtextpos++] == '"')
          {
            this.CheckTimeout();
            int[] numArray4 = runstack;
            int index4 = num3;
            num33 = index4 + 1;
            int start4 = numArray4[index4];
            this.Capture(2, start4, runtextpos);
            int num35;
            runtrack[num35 = num14 - 1] = start4;
            runtrack[num34 = num35 - 1] = 6;
            this.CheckTimeout();
          }
          else
            goto label_108;
label_57:
          this.CheckTimeout();
          int[] numArray5 = runstack;
          int index5 = num33;
          num3 = index5 + 1;
          int start5 = numArray5[index5];
          this.Capture(1, start5, runtextpos);
          int num36;
          runtrack[num36 = num34 - 1] = start5;
          int num37;
          runtrack[num37 = num36 - 1] = 6;
          this.CheckTimeout();
          int num38;
          int num39 = (num38 = runtextend - runtextpos) + 1;
          while (--num39 > 0)
          {
            if (!RegexRunner.CharInClass(runtext[runtextpos++], "\0\0\u0001d"))
            {
              --runtextpos;
              break;
            }
          }
          if (num38 > num39)
          {
            int num40;
            runtrack[num40 = num37 - 1] = num38 - num39 - 1;
            int num41;
            runtrack[num41 = num40 - 1] = runtextpos - 1;
            runtrack[num37 = num41 - 1] = 11;
          }
label_63:
          this.CheckTimeout();
          int num42;
          runstack[num42 = num3 - 1] = -1;
          int num43;
          runstack[num43 = num42 - 1] = 0;
          int num44;
          runtrack[num44 = num37 - 1] = 12;
          this.CheckTimeout();
          goto label_95;
label_64:
          this.CheckTimeout();
          runstack[--num3] = runtextpos;
          runtrack[--num14] = 1;
          this.CheckTimeout();
          if (runtextpos < runtextend && runtext[runtextpos++] == ',')
          {
            this.CheckTimeout();
            int num45;
            int num46 = (num45 = runtextend - runtextpos) + 1;
            while (--num46 > 0)
            {
              if (!RegexRunner.CharInClass(runtext[runtextpos++], "\0\0\u0001d"))
              {
                --runtextpos;
                break;
              }
            }
            if (num45 > num46)
            {
              int num47;
              runtrack[num47 = num14 - 1] = num45 - num46 - 1;
              int num48;
              runtrack[num48 = num47 - 1] = runtextpos - 1;
              runtrack[num14 = num48 - 1] = 13;
            }
          }
          else
            goto label_108;
label_71:
          this.CheckTimeout();
          int num49;
          runstack[num49 = num3 - 1] = runtextpos;
          int num50;
          runtrack[num50 = num14 - 1] = 1;
          this.CheckTimeout();
          int num51;
          runtrack[num51 = num50 - 1] = runtextpos;
          int num52;
          runtrack[num52 = num51 - 1] = 14;
          this.CheckTimeout();
          runstack[num3 = num49 - 1] = runtextpos;
          runtrack[num14 = num52 - 1] = 1;
          this.CheckTimeout();
          int num53;
          if (runtextpos < runtextend && runtext[runtextpos++] == '"')
          {
            this.CheckTimeout();
            runstack[--num3] = runtextpos;
            runtrack[num53 = num14 - 1] = 1;
            this.CheckTimeout();
            int num54;
            int num55 = (num54 = runtextend - runtextpos) + 1;
            while (--num55 > 0)
            {
              if (!RegexRunner.CharInClass(runtext[runtextpos++], "\0\u0001\0\0"))
              {
                --runtextpos;
                break;
              }
            }
            if (num54 > num55)
            {
              int num56;
              runtrack[num56 = num53 - 1] = num54 - num55 - 1;
              int num57;
              runtrack[num57 = num56 - 1] = runtextpos - 1;
              runtrack[num53 = num57 - 1] = 15;
            }
          }
          else
            goto label_108;
label_78:
          this.CheckTimeout();
          int start6 = runstack[num3++];
          this.Capture(15, start6, runtextpos);
          int num58;
          runtrack[num58 = num53 - 1] = start6;
          runtrack[num14 = num58 - 1] = 6;
          this.CheckTimeout();
          int num59;
          int num60;
          if (runtextpos < runtextend && runtext[runtextpos++] == '"')
          {
            this.CheckTimeout();
            int[] numArray6 = runstack;
            int index6 = num3;
            num59 = index6 + 1;
            int start7 = numArray6[index6];
            this.Capture(12, start7, runtextpos);
            int num61;
            runtrack[num61 = num14 - 1] = start7;
            runtrack[num60 = num61 - 1] = 6;
            this.CheckTimeout();
          }
          else
            goto label_108;
label_88:
          this.CheckTimeout();
          int[] numArray7 = runstack;
          int index7 = num59;
          num3 = index7 + 1;
          int start8 = numArray7[index7];
          this.Capture(11, start8, runtextpos);
          int num62;
          runtrack[num62 = num60 - 1] = start8;
          int num63;
          runtrack[num63 = num62 - 1] = 6;
          this.CheckTimeout();
          int num64;
          int num65 = (num64 = runtextend - runtextpos) + 1;
          while (--num65 > 0)
          {
            if (!RegexRunner.CharInClass(runtext[runtextpos++], "\0\0\u0001d"))
            {
              --runtextpos;
              break;
            }
          }
          if (num64 > num65)
          {
            int num66;
            runtrack[num66 = num63 - 1] = num64 - num65 - 1;
            int num67;
            runtrack[num67 = num66 - 1] = runtextpos - 1;
            runtrack[num63 = num67 - 1] = 17;
          }
label_94:
          this.CheckTimeout();
          int[] numArray8 = runstack;
          int index8 = num3;
          num43 = index8 + 1;
          int start9 = numArray8[index8];
          this.Capture(10, start9, runtextpos);
          int num68;
          runtrack[num68 = num63 - 1] = start9;
          runtrack[num44 = num68 - 1] = 6;
label_95:
          this.CheckTimeout();
          int[] numArray9 = runstack;
          int index9 = num43;
          int num69 = index9 + 1;
          int num70 = numArray9[index9];
          int[] numArray10 = runstack;
          int index10 = num69;
          num3 = index10 + 1;
          int num71;
          int num72 = num71 = numArray10[index10];
          int num73;
          runtrack[num73 = num44 - 1] = num72;
          int num74 = runtextpos;
          if ((num71 != num74 || num70 < 0) && num70 < 1)
          {
            int num75;
            runstack[num75 = num3 - 1] = runtextpos;
            runstack[num3 = num75 - 1] = num70 + 1;
            runtrack[num14 = num73 - 1] = 18;
            if (num14 <= 236 || num3 <= 177)
            {
              runtrack[--num14] = 19;
              goto label_108;
            }
            else
              goto label_64;
          }
          else
          {
            int num76;
            runtrack[num76 = num73 - 1] = num70;
            runtrack[num14 = num76 - 1] = 20;
          }
label_99:
          this.CheckTimeout();
          int num77;
          int num78 = (num77 = runtextend - runtextpos) + 1;
          while (--num78 > 0)
          {
            if (!RegexRunner.CharInClass(runtext[runtextpos++], "\0\0\u0001d"))
            {
              --runtextpos;
              break;
            }
          }
          if (num77 > num78)
          {
            int num79;
            runtrack[num79 = num14 - 1] = num77 - num78 - 1;
            int num80;
            runtrack[num80 = num79 - 1] = runtextpos - 1;
            runtrack[num14 = num80 - 1] = 21;
          }
label_105:
          this.CheckTimeout();
          int num81;
          if (runtextpos >= runtextend)
          {
            this.CheckTimeout();
            int[] numArray11 = runstack;
            int index11 = num3;
            int num82 = index11 + 1;
            int start10 = numArray11[index11];
            this.Capture(0, start10, runtextpos);
            int num83;
            runtrack[num83 = num14 - 1] = start10;
            runtrack[num81 = num83 - 1] = 6;
          }
          else
            goto label_108;
label_107:
          this.CheckTimeout();
          this.runtextpos = runtextpos;
          return;
label_108:
          int index12;
          int num84;
          while (true)
          {
            do
            {
              do
              {
                do
                {
                  this.runtrackpos = num14;
                  this.runstackpos = num3;
                  this.EnsureStorage();
                  int runtrackpos2 = this.runtrackpos;
                  num3 = this.runstackpos;
                  runtrack = this.runtrack;
                  runstack = this.runstack;
                  int[] numArray12 = runtrack;
                  int index13 = runtrackpos2;
                  num14 = index13 + 1;
                  switch (numArray12[index13])
                  {
                    case 1:
                      goto label_110;
                    case 2:
                      goto label_111;
                    case 3:
                      this.CheckTimeout();
                      int[] numArray13 = runtrack;
                      int index14 = num14;
                      int num85 = index14 + 1;
                      runtextpos = numArray13[index14];
                      this.CheckTimeout();
                      runstack[--num3] = runtextpos;
                      runtrack[num14 = num85 - 1] = 1;
                      this.CheckTimeout();
                      if (runtextpos < runtextend && runtext[runtextpos++] == '\'')
                      {
                        this.CheckTimeout();
                        int num86;
                        runstack[num86 = num3 - 1] = runtextpos;
                        int num87;
                        runtrack[num87 = num14 - 1] = 1;
                        this.CheckTimeout();
                        int num88;
                        runstack[num88 = num86 - 1] = runtextpos;
                        int num89;
                        runtrack[num89 = num87 - 1] = 1;
                        this.CheckTimeout();
                        int num90;
                        runtrack[num90 = num89 - 1] = runtextpos;
                        int num91;
                        runtrack[num91 = num90 - 1] = 8;
                        this.CheckTimeout();
                        runstack[num3 = num88 - 1] = runtextpos;
                        runtrack[num14 = num91 - 1] = 1;
                        this.CheckTimeout();
                        if (1 <= runtextend - runtextpos)
                        {
                          ++runtextpos;
                          int num92 = 1;
                          do
                          {
                            if (!RegexRunner.CharInClass(runtext[runtextpos - num92--], "\0\u0002\n./\0\u0002\u0004\u0005\u0003\u0001\u0006\t\u0013\0"))
                              goto label_108;
                          }
                          while (num92 > 0);
                          this.CheckTimeout();
                          int num93;
                          int num94 = (num93 = runtextend - runtextpos) + 1;
                          while (--num94 > 0)
                          {
                            if (!RegexRunner.CharInClass(runtext[runtextpos++], "\0\u0002\n./\0\u0002\u0004\u0005\u0003\u0001\u0006\t\u0013\0"))
                            {
                              --runtextpos;
                              break;
                            }
                          }
                          if (num93 > num94)
                          {
                            int num95;
                            runtrack[num95 = num14 - 1] = num93 - num94 - 1;
                            int num96;
                            runtrack[num96 = num95 - 1] = runtextpos - 1;
                            runtrack[num14 = num96 - 1] = 9;
                            break;
                          }
                          break;
                        }
                        continue;
                      }
                      continue;
                    case 4:
                      goto label_114;
                    case 5:
                      goto label_115;
                    case 6:
                      goto label_117;
                    case 7:
                      goto label_118;
                    case 8:
                      this.CheckTimeout();
                      int[] numArray14 = runtrack;
                      int index15 = num14;
                      int num97 = index15 + 1;
                      runtextpos = numArray14[index15];
                      this.CheckTimeout();
                      runstack[--num3] = runtextpos;
                      runtrack[num14 = num97 - 1] = 1;
                      this.CheckTimeout();
                      if (runtextpos < runtextend && runtext[runtextpos++] == '[')
                      {
                        this.CheckTimeout();
                        if (1 <= runtextend - runtextpos)
                        {
                          ++runtextpos;
                          int num98 = 1;
                          do
                          {
                            if (!RegexRunner.CharInClass(runtext[runtextpos - num98--], "\0\u0001\0\0"))
                              goto label_108;
                          }
                          while (num98 > 0);
                          this.CheckTimeout();
                          int num99;
                          int num100 = (num99 = runtextend - runtextpos) + 1;
                          while (--num100 > 0)
                          {
                            if (!RegexRunner.CharInClass(runtext[runtextpos++], "\0\u0001\0\0"))
                            {
                              --runtextpos;
                              break;
                            }
                          }
                          if (num99 > num100)
                          {
                            int num101;
                            runtrack[num101 = num14 - 1] = num99 - num100 - 1;
                            int num102;
                            runtrack[num102 = num101 - 1] = runtextpos - 1;
                            runtrack[num14 = num102 - 1] = 10;
                            goto label_53;
                          }
                          else
                            goto label_53;
                        }
                        else
                          continue;
                      }
                      else
                        continue;
                    case 9:
                      this.CheckTimeout();
                      int[] numArray15 = runtrack;
                      int index16 = num14;
                      int num103 = index16 + 1;
                      runtextpos = numArray15[index16];
                      int[] numArray16 = runtrack;
                      int index17 = num103;
                      num14 = index17 + 1;
                      int num104 = numArray16[index17];
                      if (num104 > 0)
                      {
                        int num105;
                        runtrack[num105 = num14 - 1] = num104 - 1;
                        int num106;
                        runtrack[num106 = num105 - 1] = runtextpos - 1;
                        runtrack[num14 = num106 - 1] = 9;
                        break;
                      }
                      break;
                    case 10:
                      this.CheckTimeout();
                      int[] numArray17 = runtrack;
                      int index18 = num14;
                      int num107 = index18 + 1;
                      runtextpos = numArray17[index18];
                      int[] numArray18 = runtrack;
                      int index19 = num107;
                      num14 = index19 + 1;
                      int num108 = numArray18[index19];
                      if (num108 > 0)
                      {
                        int num109;
                        runtrack[num109 = num14 - 1] = num108 - 1;
                        int num110;
                        runtrack[num110 = num109 - 1] = runtextpos - 1;
                        runtrack[num14 = num110 - 1] = 10;
                        goto label_53;
                      }
                      else
                        goto label_53;
                    case 11:
                      goto label_125;
                    case 12:
                      goto label_127;
                    case 13:
                      goto label_128;
                    case 14:
                      goto label_130;
                    case 15:
                      goto label_131;
                    case 16:
                      goto label_133;
                    case 17:
                      goto label_135;
                    case 18:
                      goto label_137;
                    case 19:
                      goto label_64;
                    case 20:
                      goto label_140;
                    case 21:
                      goto label_141;
                    default:
                      goto label_109;
                  }
                  this.CheckTimeout();
                  int[] numArray19 = runstack;
                  int index20 = num3;
                  int num111 = index20 + 1;
                  int start11 = numArray19[index20];
                  this.Capture(8, start11, runtextpos);
                  int num112;
                  runtrack[num112 = num14 - 1] = start11;
                  int num113;
                  runtrack[num113 = num112 - 1] = 6;
                  this.CheckTimeout();
                  goto label_55;
label_53:
                  this.CheckTimeout();
                  if (runtextpos < runtextend && runtext[runtextpos++] == ']')
                  {
                    this.CheckTimeout();
                    int[] numArray20 = runstack;
                    int index21 = num3;
                    num111 = index21 + 1;
                    int start12 = numArray20[index21];
                    this.Capture(9, start12, runtextpos);
                    int num114;
                    runtrack[num114 = num14 - 1] = start12;
                    runtrack[num113 = num114 - 1] = 6;
                  }
                  else
                    continue;
label_55:
                  this.CheckTimeout();
                  int[] numArray21 = runstack;
                  int index22 = num111;
                  int num115 = index22 + 1;
                  int start13 = numArray21[index22];
                  this.Capture(7, start13, runtextpos);
                  int num116;
                  runtrack[num116 = num113 - 1] = start13;
                  int num117;
                  runtrack[num117 = num116 - 1] = 6;
                  this.CheckTimeout();
                  int[] numArray22 = runstack;
                  int index23 = num115;
                  num3 = index23 + 1;
                  int start14 = numArray22[index23];
                  this.Capture(14, start14, runtextpos);
                  int num118;
                  runtrack[num118 = num117 - 1] = start14;
                  runtrack[num14 = num118 - 1] = 6;
                  this.CheckTimeout();
                }
                while (runtextpos >= runtextend || runtext[runtextpos++] != '\'');
                goto label_56;
label_28:
                this.CheckTimeout();
                continue;
label_114:
                this.CheckTimeout();
                int[] numArray23 = runtrack;
                int index24 = num14;
                int num119 = index24 + 1;
                runtextpos = numArray23[index24];
                this.CheckTimeout();
                runstack[--num3] = runtextpos;
                runtrack[num14 = num119 - 1] = 1;
                this.CheckTimeout();
                if (runtextpos < runtextend && runtext[runtextpos++] == '[')
                {
                  this.CheckTimeout();
                  if (1 <= runtextend - runtextpos)
                  {
                    ++runtextpos;
                    int num120 = 1;
                    do
                    {
                      if (!RegexRunner.CharInClass(runtext[runtextpos - num120--], "\0\u0001\0\0"))
                        goto label_108;
                    }
                    while (num120 > 0);
                    this.CheckTimeout();
                    int num121;
                    int num122 = (num121 = runtextend - runtextpos) + 1;
                    while (--num122 > 0)
                    {
                      if (!RegexRunner.CharInClass(runtext[runtextpos++], "\0\u0001\0\0"))
                      {
                        --runtextpos;
                        break;
                      }
                    }
                    if (num121 > num122)
                    {
                      int num123;
                      runtrack[num123 = num14 - 1] = num121 - num122 - 1;
                      int num124;
                      runtrack[num124 = num123 - 1] = runtextpos - 1;
                      runtrack[num14 = num124 - 1] = 7;
                      goto label_28;
                    }
                    else
                      goto label_28;
                  }
                  else
                    continue;
                }
                else
                  continue;
label_118:
                this.CheckTimeout();
                int[] numArray24 = runtrack;
                int index25 = num14;
                int num125 = index25 + 1;
                runtextpos = numArray24[index25];
                int[] numArray25 = runtrack;
                int index26 = num125;
                num14 = index26 + 1;
                int num126 = numArray25[index26];
                if (num126 > 0)
                {
                  int num127;
                  runtrack[num127 = num14 - 1] = num126 - 1;
                  int num128;
                  runtrack[num128 = num127 - 1] = runtextpos - 1;
                  runtrack[num14 = num128 - 1] = 7;
                  goto label_28;
                }
                else
                  goto label_28;
              }
              while (runtextpos >= runtextend || runtext[runtextpos++] != ']');
              goto label_29;
label_86:
              this.CheckTimeout();
              int start15 = runstack[num3++];
              this.Capture(15, start15, runtextpos);
              int num129;
              int num130;
              runtrack[num130 = num129 - 1] = start15;
              runtrack[num14 = num130 - 1] = 6;
              this.CheckTimeout();
              continue;
label_130:
              this.CheckTimeout();
              int[] numArray26 = runtrack;
              int index27 = num14;
              int num131 = index27 + 1;
              runtextpos = numArray26[index27];
              this.CheckTimeout();
              runstack[--num3] = runtextpos;
              runtrack[num14 = num131 - 1] = 1;
              this.CheckTimeout();
              if (runtextpos < runtextend && runtext[runtextpos++] == '\'')
              {
                this.CheckTimeout();
                runstack[--num3] = runtextpos;
                runtrack[num129 = num14 - 1] = 1;
                this.CheckTimeout();
                int num132;
                int num133 = (num132 = runtextend - runtextpos) + 1;
                while (--num133 > 0)
                {
                  if (!RegexRunner.CharInClass(runtext[runtextpos++], "\0\u0001\0\0"))
                  {
                    --runtextpos;
                    break;
                  }
                }
                if (num132 > num133)
                {
                  int num134;
                  runtrack[num134 = num129 - 1] = num132 - num133 - 1;
                  int num135;
                  runtrack[num135 = num134 - 1] = runtextpos - 1;
                  runtrack[num129 = num135 - 1] = 16;
                  goto label_86;
                }
                else
                  goto label_86;
              }
              else
                continue;
label_133:
              this.CheckTimeout();
              int[] numArray27 = runtrack;
              int index28 = num14;
              int num136 = index28 + 1;
              runtextpos = numArray27[index28];
              int[] numArray28 = runtrack;
              int index29 = num136;
              num129 = index29 + 1;
              int num137 = numArray28[index29];
              if (num137 > 0)
              {
                int num138;
                runtrack[num138 = num129 - 1] = num137 - 1;
                int num139;
                runtrack[num139 = num138 - 1] = runtextpos - 1;
                runtrack[num129 = num139 - 1] = 16;
                goto label_86;
              }
              else
                goto label_86;
            }
            while (runtextpos >= runtextend || runtext[runtextpos++] != '\'');
            goto label_87;
label_110:
            this.CheckTimeout();
            ++num3;
            continue;
label_117:
            this.CheckTimeout();
            runstack[--num3] = runtrack[num14++];
            this.Uncapture();
            continue;
label_127:
            this.CheckTimeout();
            num3 += 2;
            continue;
label_137:
            this.CheckTimeout();
            int[] numArray29 = runstack;
            int index30 = num3;
            index12 = index30 + 1;
            if ((num84 = numArray29[index30] - 1) < 0)
            {
              runstack[index12] = runtrack[num14++];
              runstack[num3 = index12 - 1] = num84;
              continue;
            }
            goto label_138;
label_140:
            this.CheckTimeout();
            int[] numArray30 = runtrack;
            int index31 = num14;
            int num140 = index31 + 1;
            int num141 = numArray30[index31];
            int[] numArray31 = runstack;
            int index32;
            int num142 = index32 = num3 - 1;
            int[] numArray32 = runtrack;
            int index33 = num140;
            num14 = index33 + 1;
            int num143 = numArray32[index33];
            numArray31[index32] = num143;
            runstack[num3 = num142 - 1] = num141;
          }
label_29:
          this.CheckTimeout();
          int[] numArray33 = runstack;
          int index34 = num3;
          num26 = index34 + 1;
          int start16 = numArray33[index34];
          this.Capture(5, start16, runtextpos);
          int num144;
          runtrack[num144 = num14 - 1] = start16;
          runtrack[num28 = num144 - 1] = 6;
          goto label_30;
label_56:
          this.CheckTimeout();
          int[] numArray34 = runstack;
          int index35 = num3;
          num33 = index35 + 1;
          int start17 = numArray34[index35];
          this.Capture(6, start17, runtextpos);
          int num145;
          runtrack[num145 = num14 - 1] = start17;
          runtrack[num34 = num145 - 1] = 6;
          goto label_57;
label_87:
          this.CheckTimeout();
          int[] numArray35 = runstack;
          int index36 = num3;
          num59 = index36 + 1;
          int start18 = numArray35[index36];
          this.Capture(13, start18, runtextpos);
          int num146;
          runtrack[num146 = num14 - 1] = start18;
          runtrack[num60 = num146 - 1] = 6;
          goto label_88;
label_109:
          this.CheckTimeout();
          int[] numArray36 = runtrack;
          int index37 = num14;
          num81 = index37 + 1;
          runtextpos = numArray36[index37];
          goto label_107;
label_111:
          this.CheckTimeout();
          int[] numArray37 = runtrack;
          int index38 = num14;
          int num147 = index38 + 1;
          runtextpos = numArray37[index38];
          int[] numArray38 = runtrack;
          int index39 = num147;
          num4 = index39 + 1;
          num9 = numArray38[index39];
          continue;
label_115:
          this.CheckTimeout();
          int[] numArray39 = runtrack;
          int index40 = num14;
          int num148 = index40 + 1;
          runtextpos = numArray39[index40];
          int[] numArray40 = runtrack;
          int index41 = num148;
          num14 = index41 + 1;
          int num149 = numArray40[index41];
          if (num149 > 0)
          {
            int num150;
            runtrack[num150 = num14 - 1] = num149 - 1;
            int num151;
            runtrack[num151 = num150 - 1] = runtextpos - 1;
            runtrack[num14 = num151 - 1] = 5;
            goto label_17;
          }
          else
            goto label_17;
label_125:
          this.CheckTimeout();
          int[] numArray41 = runtrack;
          int index42 = num14;
          int num152 = index42 + 1;
          runtextpos = numArray41[index42];
          int[] numArray42 = runtrack;
          int index43 = num152;
          num37 = index43 + 1;
          int num153 = numArray42[index43];
          if (num153 > 0)
          {
            int num154;
            runtrack[num154 = num37 - 1] = num153 - 1;
            int num155;
            runtrack[num155 = num154 - 1] = runtextpos - 1;
            runtrack[num37 = num155 - 1] = 11;
            goto label_63;
          }
          else
            goto label_63;
label_128:
          this.CheckTimeout();
          int[] numArray43 = runtrack;
          int index44 = num14;
          int num156 = index44 + 1;
          runtextpos = numArray43[index44];
          int[] numArray44 = runtrack;
          int index45 = num156;
          num14 = index45 + 1;
          int num157 = numArray44[index45];
          if (num157 > 0)
          {
            int num158;
            runtrack[num158 = num14 - 1] = num157 - 1;
            int num159;
            runtrack[num159 = num158 - 1] = runtextpos - 1;
            runtrack[num14 = num159 - 1] = 13;
            goto label_71;
          }
          else
            goto label_71;
label_131:
          this.CheckTimeout();
          int[] numArray45 = runtrack;
          int index46 = num14;
          int num160 = index46 + 1;
          runtextpos = numArray45[index46];
          int[] numArray46 = runtrack;
          int index47 = num160;
          num53 = index47 + 1;
          int num161 = numArray46[index47];
          if (num161 > 0)
          {
            int num162;
            runtrack[num162 = num53 - 1] = num161 - 1;
            int num163;
            runtrack[num163 = num162 - 1] = runtextpos - 1;
            runtrack[num53 = num163 - 1] = 15;
            goto label_78;
          }
          else
            goto label_78;
label_135:
          this.CheckTimeout();
          int[] numArray47 = runtrack;
          int index48 = num14;
          int num164 = index48 + 1;
          runtextpos = numArray47[index48];
          int[] numArray48 = runtrack;
          int index49 = num164;
          num63 = index49 + 1;
          int num165 = numArray48[index49];
          if (num165 > 0)
          {
            int num166;
            runtrack[num166 = num63 - 1] = num165 - 1;
            int num167;
            runtrack[num167 = num166 - 1] = runtextpos - 1;
            runtrack[num63 = num167 - 1] = 17;
            goto label_94;
          }
          else
            goto label_94;
label_138:
          int[] numArray49 = runstack;
          int index50 = index12;
          num3 = index50 + 1;
          runtextpos = numArray49[index50];
          int num168;
          runtrack[num168 = num14 - 1] = num84;
          runtrack[num14 = num168 - 1] = 20;
          goto label_99;
label_141:
          this.CheckTimeout();
          int[] numArray50 = runtrack;
          int index51 = num14;
          int num169 = index51 + 1;
          runtextpos = numArray50[index51];
          int[] numArray51 = runtrack;
          int index52 = num169;
          num14 = index52 + 1;
          int num170 = numArray51[index52];
          if (num170 > 0)
          {
            int num171;
            runtrack[num171 = num14 - 1] = num170 - 1;
            int num172;
            runtrack[num172 = num171 - 1] = runtextpos - 1;
            runtrack[num14 = num172 - 1] = 21;
            goto label_105;
          }
          else
            goto label_105;
        }
        while (num9 <= 0);
        int num173;
        runtrack[num173 = num4 - 1] = num9 - 1;
        int num174;
        runtrack[num174 = num173 - 1] = runtextpos - 1;
        runtrack[num4 = num174 - 1] = 2;
      }
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
        if (RegexRunner.CharInClass(runtext[runtextpos++], "\0\u0004\u0001\"#'(d"))
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

    protected override void InitTrackCount() => this.runtrackcount = 59;
  }
}

#endif