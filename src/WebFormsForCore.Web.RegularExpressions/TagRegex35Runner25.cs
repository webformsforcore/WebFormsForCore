
#if NETFRAMEWORK

using System.Text.RegularExpressions;

#nullable disable
namespace System.Web.RegularExpressions
{
  internal class TagRegex35Runner25 : RegexRunner
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
        if (num1 < runtextend && runtext[num1++] == '<')
        {
          this.CheckTimeout();
          runstack[--num4] = num1;
          runtrack[--num5] = 1;
          this.CheckTimeout();
          if (1 <= runtextend - num1)
          {
            ++num1;
            int num6 = 1;
            while (RegexRunner.CharInClass(runtext[num1 - num6--], "\0\u0004\n./:;\0\u0002\u0004\u0005\u0003\u0001\u0006\t\u0013\0"))
            {
              if (num6 <= 0)
              {
                this.CheckTimeout();
                int num7;
                int num8 = (num7 = runtextend - num1) + 1;
                while (--num8 > 0)
                {
                  if (!RegexRunner.CharInClass(runtext[num1++], "\0\u0004\n./:;\0\u0002\u0004\u0005\u0003\u0001\u0006\t\u0013\0"))
                  {
                    --num1;
                    break;
                  }
                }
                if (num7 > num8)
                {
                  int num9;
                  runtrack[num9 = num5 - 1] = num7 - num8 - 1;
                  int num10;
                  runtrack[num10 = num9 - 1] = num1 - 1;
                  runtrack[num5 = num10 - 1] = 2;
                  goto label_12;
                }
                else
                  goto label_12;
              }
            }
            goto label_132;
          }
          else
            goto label_132;
        }
        else
          goto label_132;
      }
      else
        goto label_132;
label_12:
      this.CheckTimeout();
      int[] numArray1 = runstack;
      int index1 = num4;
      int num11 = index1 + 1;
      int start1 = numArray1[index1];
      this.Capture(3, start1, num1);
      int num12;
      runtrack[num12 = num5 - 1] = start1;
      int num13;
      runtrack[num13 = num12 - 1] = 3;
      this.CheckTimeout();
      int num14;
      runstack[num14 = num11 - 1] = -1;
      int num15;
      runtrack[num15 = num13 - 1] = 1;
      this.CheckTimeout();
      goto label_112;
label_13:
      this.CheckTimeout();
      runstack[--num4] = num1;
      runtrack[--num5] = 1;
      this.CheckTimeout();
      if (1 <= runtextend - num1)
      {
        ++num1;
        int num16 = 1;
        while (RegexRunner.CharInClass(runtext[num1 - num16--], "\0\0\u0001d"))
        {
          if (num16 <= 0)
          {
            this.CheckTimeout();
            int num17;
            int num18 = (num17 = runtextend - num1) + 1;
            while (--num18 > 0)
            {
              if (!RegexRunner.CharInClass(runtext[num1++], "\0\0\u0001d"))
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
              goto label_23;
            }
            else
              goto label_23;
          }
        }
        goto label_132;
      }
      else
        goto label_132;
label_23:
      this.CheckTimeout();
      runstack[--num4] = num1;
      runtrack[--num5] = 1;
      this.CheckTimeout();
      if (num1 < runtextend && RegexRunner.CharInClass(runtext[num1++], "\0\0\n\0\u0002\u0004\u0005\u0003\u0001\u0006\t\u0013\0"))
      {
        this.CheckTimeout();
        int num21;
        int num22 = (num21 = runtextend - num1) + 1;
        while (--num22 > 0)
        {
          if (!RegexRunner.CharInClass(runtext[num1++], "\0\u0004\n-.:;\0\u0002\u0004\u0005\u0003\u0001\u0006\t\u0013\0"))
          {
            --num1;
            break;
          }
        }
        if (num21 > num22)
        {
          int num23;
          runtrack[num23 = num5 - 1] = num21 - num22 - 1;
          int num24;
          runtrack[num24 = num23 - 1] = num1 - 1;
          runtrack[num5 = num24 - 1] = 5;
        }
      }
      else
        goto label_132;
label_30:
      this.CheckTimeout();
      int[] numArray2 = runstack;
      int index2 = num4;
      int num25 = index2 + 1;
      int start2 = numArray2[index2];
      this.Capture(4, start2, num1);
      int num26;
      runtrack[num26 = num5 - 1] = start2;
      int num27;
      runtrack[num27 = num26 - 1] = 3;
      this.CheckTimeout();
      runstack[num4 = num25 - 1] = num1;
      int num28;
      runtrack[num28 = num27 - 1] = 1;
      this.CheckTimeout();
      int num29;
      runtrack[num29 = num28 - 1] = num1;
      runtrack[num5 = num29 - 1] = 6;
      this.CheckTimeout();
      int num30;
      int num31 = (num30 = runtextend - num1) + 1;
      while (--num31 > 0)
      {
        if (!RegexRunner.CharInClass(runtext[num1++], "\0\0\u0001d"))
        {
          --num1;
          break;
        }
      }
      if (num30 > num31)
      {
        int num32;
        runtrack[num32 = num5 - 1] = num30 - num31 - 1;
        int num33;
        runtrack[num33 = num32 - 1] = num1 - 1;
        runtrack[num5 = num33 - 1] = 7;
      }
label_36:
      this.CheckTimeout();
      if (num1 < runtextend && runtext[num1++] == '=')
      {
        this.CheckTimeout();
        int num34;
        int num35 = (num34 = runtextend - num1) + 1;
        while (--num35 > 0)
        {
          if (!RegexRunner.CharInClass(runtext[num1++], "\0\0\u0001d"))
          {
            --num1;
            break;
          }
        }
        if (num34 > num35)
        {
          int num36;
          runtrack[num36 = num5 - 1] = num34 - num35 - 1;
          int num37;
          runtrack[num37 = num36 - 1] = num1 - 1;
          runtrack[num5 = num37 - 1] = 8;
        }
      }
      else
        goto label_132;
label_43:
      this.CheckTimeout();
      int num38;
      if (num1 < runtextend && runtext[num1++] == '"')
      {
        this.CheckTimeout();
        runstack[--num4] = num1;
        runtrack[num38 = num5 - 1] = 1;
        this.CheckTimeout();
        int num39;
        int num40 = (num39 = runtextend - num1) + 1;
        while (--num40 > 0)
        {
          if (runtext[num1++] == '"')
          {
            --num1;
            break;
          }
        }
        if (num39 > num40)
        {
          int num41;
          runtrack[num41 = num38 - 1] = num39 - num40 - 1;
          int num42;
          runtrack[num42 = num41 - 1] = num1 - 1;
          runtrack[num38 = num42 - 1] = 9;
        }
      }
      else
        goto label_132;
label_50:
      this.CheckTimeout();
      int start3 = runstack[num4++];
      this.Capture(5, start3, num1);
      int num43;
      runtrack[num43 = num38 - 1] = start3;
      runtrack[num5 = num43 - 1] = 3;
      this.CheckTimeout();
      if (num1 < runtextend && runtext[num1++] == '"')
        this.CheckTimeout();
      else
        goto label_132;
label_111:
      this.CheckTimeout();
      int[] numArray3 = runstack;
      int index3 = num4;
      int num44 = index3 + 1;
      int start4 = numArray3[index3];
      this.Capture(2, start4, num1);
      int num45;
      runtrack[num45 = num5 - 1] = start4;
      int num46;
      runtrack[num46 = num45 - 1] = 3;
      this.CheckTimeout();
      int[] numArray4 = runstack;
      int index4 = num44;
      num14 = index4 + 1;
      int start5 = numArray4[index4];
      this.Capture(1, start5, num1);
      int num47;
      runtrack[num47 = num46 - 1] = start5;
      runtrack[num15 = num47 - 1] = 3;
label_112:
      this.CheckTimeout();
      int[] numArray5 = runstack;
      int index5 = num14;
      num4 = index5 + 1;
      int num48;
      int num49 = num48 = numArray5[index5];
      int num50;
      runtrack[num50 = num15 - 1] = num49;
      int num51 = num1;
      int num52;
      if (num48 != num51)
      {
        int num53;
        runtrack[num53 = num50 - 1] = num1;
        runstack[--num4] = num1;
        runtrack[num5 = num53 - 1] = 23;
        if (num5 <= 212 || num4 <= 159)
        {
          runtrack[--num5] = 24;
          goto label_132;
        }
        else
          goto label_13;
      }
      else
        runtrack[num52 = num50 - 1] = 25;
label_116:
      this.CheckTimeout();
      int num54;
      int num55 = (num54 = runtextend - num1) + 1;
      while (--num55 > 0)
      {
        if (!RegexRunner.CharInClass(runtext[num1++], "\0\0\u0001d"))
        {
          --num1;
          break;
        }
      }
      if (num54 > num55)
      {
        int num56;
        runtrack[num56 = num52 - 1] = num54 - num55 - 1;
        int num57;
        runtrack[num57 = num56 - 1] = num1 - 1;
        runtrack[num52 = num57 - 1] = 26;
      }
label_122:
      this.CheckTimeout();
      int num58;
      runstack[num58 = num4 - 1] = -1;
      int num59;
      runstack[num59 = num58 - 1] = 0;
      int num60;
      runtrack[num60 = num52 - 1] = 27;
      this.CheckTimeout();
      goto label_125;
label_123:
      this.CheckTimeout();
      runstack[--num4] = num1;
      runtrack[--num5] = 1;
      this.CheckTimeout();
      if (num1 < runtextend && runtext[num1++] == '/')
      {
        this.CheckTimeout();
        int[] numArray6 = runstack;
        int index6 = num4;
        num59 = index6 + 1;
        int start6 = numArray6[index6];
        this.Capture(6, start6, num1);
        int num61;
        runtrack[num61 = num5 - 1] = start6;
        runtrack[num60 = num61 - 1] = 3;
      }
      else
        goto label_132;
label_125:
      this.CheckTimeout();
      int[] numArray7 = runstack;
      int index7 = num59;
      int num62 = index7 + 1;
      int num63 = numArray7[index7];
      int[] numArray8 = runstack;
      int index8 = num62;
      num4 = index8 + 1;
      int num64;
      int num65 = num64 = numArray8[index8];
      int num66;
      runtrack[num66 = num60 - 1] = num65;
      int num67 = num1;
      if ((num64 != num67 || num63 < 0) && num63 < 1)
      {
        int num68;
        runstack[num68 = num4 - 1] = num1;
        runstack[num4 = num68 - 1] = num63 + 1;
        runtrack[num5 = num66 - 1] = 28;
        if (num5 <= 212 || num4 <= 159)
        {
          runtrack[--num5] = 29;
          goto label_132;
        }
        else
          goto label_123;
      }
      else
      {
        int num69;
        runtrack[num69 = num66 - 1] = num63;
        runtrack[num5 = num69 - 1] = 30;
      }
label_129:
      this.CheckTimeout();
      int num70;
      if (num1 < runtextend && runtext[num1++] == '>')
      {
        this.CheckTimeout();
        int[] numArray9 = runstack;
        int index9 = num4;
        int num71 = index9 + 1;
        int start7 = numArray9[index9];
        this.Capture(0, start7, num1);
        int num72;
        runtrack[num72 = num5 - 1] = start7;
        runtrack[num70 = num72 - 1] = 3;
      }
      else
        goto label_132;
label_131:
      this.CheckTimeout();
      this.runtextpos = num1;
      return;
label_132:
      int num73;
      int num74;
      int num75 = 0;
      int index10;
      int num76;
      while (true)
      {
        string str1 = null;
        int index11 = 0;
        do
        {
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
                int num77;
                switch (numArray10[index12])
                {
                  case 1:
                    goto label_134;
                  case 2:
                    goto label_135;
                  case 3:
                    goto label_137;
                  case 4:
                    goto label_138;
                  case 5:
                    goto label_140;
                  case 6:
                    this.CheckTimeout();
                    int[] numArray11 = runtrack;
                    int index13 = num5;
                    int num78 = index13 + 1;
                    num1 = numArray11[index13];
                    this.CheckTimeout();
                    int num79;
                    runtrack[num79 = num78 - 1] = num1;
                    runtrack[num5 = num79 - 1] = 10;
                    this.CheckTimeout();
                    int num80;
                    int num81 = (num80 = runtextend - num1) + 1;
                    while (--num81 > 0)
                    {
                      if (!RegexRunner.CharInClass(runtext[num1++], "\0\0\u0001d"))
                      {
                        --num1;
                        break;
                      }
                    }
                    if (num80 > num81)
                    {
                      int num82;
                      runtrack[num82 = num5 - 1] = num80 - num81 - 1;
                      int num83;
                      runtrack[num83 = num82 - 1] = num1 - 1;
                      runtrack[num5 = num83 - 1] = 11;
                      break;
                    }
                    break;
                  case 7:
                    goto label_143;
                  case 8:
                    goto label_145;
                  case 9:
                    goto label_147;
                  case 10:
                    goto label_149;
                  case 11:
                    this.CheckTimeout();
                    int[] numArray12 = runtrack;
                    int index14 = num5;
                    int num84 = index14 + 1;
                    num1 = numArray12[index14];
                    int[] numArray13 = runtrack;
                    int index15 = num84;
                    num5 = index15 + 1;
                    int num85 = numArray13[index15];
                    if (num85 > 0)
                    {
                      int num86;
                      runtrack[num86 = num5 - 1] = num85 - 1;
                      int num87;
                      runtrack[num87 = num86 - 1] = num1 - 1;
                      runtrack[num5 = num87 - 1] = 11;
                      break;
                    }
                    break;
                  case 12:
                    this.CheckTimeout();
                    int[] numArray14 = runtrack;
                    int index16 = num5;
                    int num88 = index16 + 1;
                    num1 = numArray14[index16];
                    int[] numArray15 = runtrack;
                    int index17 = num88;
                    num5 = index17 + 1;
                    int num89 = numArray15[index17];
                    if (num89 > 0)
                    {
                      int num90;
                      runtrack[num90 = num5 - 1] = num89 - 1;
                      int num91;
                      runtrack[num91 = num90 - 1] = num1 - 1;
                      runtrack[num5 = num91 - 1] = 12;
                      goto label_64;
                    }
                    else
                      goto label_64;
                  case 13:
                    this.CheckTimeout();
                    int[] numArray16 = runtrack;
                    int index18 = num5;
                    int num92 = index18 + 1;
                    num1 = numArray16[index18];
                    int[] numArray17 = runtrack;
                    int index19 = num92;
                    num77 = index19 + 1;
                    int num93 = numArray17[index19];
                    if (num93 > 0)
                    {
                      int num94;
                      runtrack[num94 = num77 - 1] = num93 - 1;
                      int num95;
                      runtrack[num95 = num94 - 1] = num1 - 1;
                      runtrack[num77 = num95 - 1] = 13;
                      goto label_71;
                    }
                    else
                      goto label_71;
                  case 14:
                    goto label_156;
                  case 15:
                    goto label_157;
                  case 16:
                    goto label_159;
                  case 17:
                    goto label_161;
                  case 18:
                    goto label_164;
                  case 19:
                    goto label_165;
                  case 20:
                    goto label_167;
                  case 21:
                    goto label_169;
                  case 22:
                    goto label_171;
                  case 23:
                    goto label_174;
                  case 24:
                    goto label_13;
                  case 25:
                    goto label_175;
                  case 26:
                    goto label_176;
                  case 27:
                    goto label_178;
                  case 28:
                    goto label_179;
                  case 29:
                    goto label_123;
                  case 30:
                    goto label_182;
                  default:
                    goto label_133;
                }
                this.CheckTimeout();
                if (num1 < runtextend && runtext[num1++] == '=')
                {
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
                    runtrack[num5 = num99 - 1] = 12;
                  }
                }
                else
                  continue;
label_64:
                this.CheckTimeout();
                if (num1 < runtextend && runtext[num1++] == '\'')
                {
                  this.CheckTimeout();
                  runstack[--num4] = num1;
                  runtrack[num77 = num5 - 1] = 1;
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
                    runtrack[num102 = num77 - 1] = num100 - num101 - 1;
                    int num103;
                    runtrack[num103 = num102 - 1] = num1 - 1;
                    runtrack[num77 = num103 - 1] = 13;
                  }
                }
                else
                  continue;
label_71:
                this.CheckTimeout();
                int start8 = runstack[num4++];
                this.Capture(5, start8, num1);
                int num104;
                runtrack[num104 = num77 - 1] = start8;
                runtrack[num5 = num104 - 1] = 3;
                this.CheckTimeout();
              }
              while (num1 >= runtextend || runtext[num1++] != '\'');
              goto label_72;
label_78:
              this.CheckTimeout();
              if (num1 < runtextend && runtext[num1++] == '=')
              {
                this.CheckTimeout();
                int num105;
                int num106 = (num105 = runtextend - num1) + 1;
                while (--num106 > 0)
                {
                  if (!RegexRunner.CharInClass(runtext[num1++], "\0\0\u0001d"))
                  {
                    --num1;
                    break;
                  }
                }
                if (num105 > num106)
                {
                  int num107;
                  runtrack[num107 = num5 - 1] = num105 - num106 - 1;
                  int num108;
                  runtrack[num108 = num107 - 1] = num1 - 1;
                  runtrack[num5 = num108 - 1] = 16;
                }
              }
              else
                continue;
label_85:
              this.CheckTimeout();
              runstack[--num4] = num1;
              runtrack[--num5] = 1;
              this.CheckTimeout();
              if (3 <= runtextend - num1 && runtext[num1] == '<' && runtext[num1 + 1] == '%' && runtext[num1 + 2] == '#')
              {
                num1 += 3;
                this.CheckTimeout();
                int num109;
                if ((num109 = runtextend - num1) > 0)
                {
                  int num110;
                  runtrack[num110 = num5 - 1] = num109 - 1;
                  int num111;
                  runtrack[num111 = num110 - 1] = num1;
                  runtrack[num5 = num111 - 1] = 17;
                }
              }
              else
                continue;
label_88:
              this.CheckTimeout();
              continue;
label_149:
              this.CheckTimeout();
              int[] numArray18 = runtrack;
              int index20 = num5;
              int num112 = index20 + 1;
              num1 = numArray18[index20];
              this.CheckTimeout();
              int num113;
              runtrack[num113 = num112 - 1] = num1;
              runtrack[num5 = num113 - 1] = 14;
              this.CheckTimeout();
              int num114;
              int num115 = (num114 = runtextend - num1) + 1;
              while (--num115 > 0)
              {
                if (!RegexRunner.CharInClass(runtext[num1++], "\0\0\u0001d"))
                {
                  --num1;
                  break;
                }
              }
              if (num114 > num115)
              {
                int num116;
                runtrack[num116 = num5 - 1] = num114 - num115 - 1;
                int num117;
                runtrack[num117 = num116 - 1] = num1 - 1;
                runtrack[num5 = num117 - 1] = 15;
                goto label_78;
              }
              else
                goto label_78;
label_157:
              this.CheckTimeout();
              int[] numArray19 = runtrack;
              int index21 = num5;
              int num118 = index21 + 1;
              num1 = numArray19[index21];
              int[] numArray20 = runtrack;
              int index22 = num118;
              num5 = index22 + 1;
              int num119 = numArray20[index22];
              if (num119 > 0)
              {
                int num120;
                runtrack[num120 = num5 - 1] = num119 - 1;
                int num121;
                runtrack[num121 = num120 - 1] = num1 - 1;
                runtrack[num5 = num121 - 1] = 15;
                goto label_78;
              }
              else
                goto label_78;
label_159:
              this.CheckTimeout();
              int[] numArray21 = runtrack;
              int index23 = num5;
              int num122 = index23 + 1;
              num1 = numArray21[index23];
              int[] numArray22 = runtrack;
              int index24 = num122;
              num5 = index24 + 1;
              int num123 = numArray22[index24];
              if (num123 > 0)
              {
                int num124;
                runtrack[num124 = num5 - 1] = num123 - 1;
                int num125;
                runtrack[num125 = num124 - 1] = num1 - 1;
                runtrack[num5 = num125 - 1] = 16;
                goto label_85;
              }
              else
                goto label_85;
label_161:
              this.CheckTimeout();
              int[] numArray23 = runtrack;
              int index25 = num5;
              int num126 = index25 + 1;
              int num127 = numArray23[index25];
              int[] numArray24 = runtrack;
              int index26 = num126;
              num5 = index26 + 1;
              int num128 = numArray24[index26];
              string str2 = runtext;
              int index27 = num127;
              num1 = index27 + 1;
              if (RegexRunner.CharInClass(str2[index27], "\0\u0001\0\0"))
              {
                if (num128 > 0)
                {
                  int num129;
                  runtrack[num129 = num5 - 1] = num128 - 1;
                  int num130;
                  runtrack[num130 = num129 - 1] = num1;
                  runtrack[num5 = num130 - 1] = 17;
                  goto label_88;
                }
                else
                  goto label_88;
              }
            }
            while (2 > runtextend - num1 || runtext[num1] != '%' || runtext[num1 + 1] != '>');
            goto label_89;
label_95:
            this.CheckTimeout();
            continue;
label_156:
            this.CheckTimeout();
            int[] numArray25 = runtrack;
            int index28 = num5;
            int num131 = index28 + 1;
            num1 = numArray25[index28];
            this.CheckTimeout();
            int num132;
            runtrack[num132 = num131 - 1] = num1;
            runtrack[num5 = num132 - 1] = 18;
            this.CheckTimeout();
            int num133;
            int num134 = (num133 = runtextend - num1) + 1;
            while (--num134 > 0)
            {
              if (!RegexRunner.CharInClass(runtext[num1++], "\0\0\u0001d"))
              {
                --num1;
                break;
              }
            }
            if (num133 > num134)
            {
              int num135;
              runtrack[num135 = num5 - 1] = num133 - num134 - 1;
              int num136;
              runtrack[num136 = num135 - 1] = num1 - 1;
              runtrack[num5 = num136 - 1] = 19;
              goto label_95;
            }
            else
              goto label_95;
label_165:
            this.CheckTimeout();
            int[] numArray26 = runtrack;
            int index29 = num5;
            int num137 = index29 + 1;
            num1 = numArray26[index29];
            int[] numArray27 = runtrack;
            int index30 = num137;
            num5 = index30 + 1;
            int num138 = numArray27[index30];
            if (num138 > 0)
            {
              int num139;
              runtrack[num139 = num5 - 1] = num138 - 1;
              int num140;
              runtrack[num140 = num139 - 1] = num1 - 1;
              runtrack[num5 = num140 - 1] = 19;
              goto label_95;
            }
            else
              goto label_95;
          }
          while (num1 >= runtextend || runtext[num1++] != '=');
          goto label_96;
label_133:
          this.CheckTimeout();
          int[] numArray28 = runtrack;
          int index31 = num5;
          num70 = index31 + 1;
          num1 = numArray28[index31];
          goto label_131;
label_134:
          this.CheckTimeout();
          ++num4;
          continue;
label_135:
          this.CheckTimeout();
          int[] numArray29 = runtrack;
          int index32 = num5;
          int num141 = index32 + 1;
          num1 = numArray29[index32];
          int[] numArray30 = runtrack;
          int index33 = num141;
          num5 = index33 + 1;
          int num142 = numArray30[index33];
          if (num142 > 0)
          {
            int num143;
            runtrack[num143 = num5 - 1] = num142 - 1;
            int num144;
            runtrack[num144 = num143 - 1] = num1 - 1;
            runtrack[num5 = num144 - 1] = 2;
            goto label_12;
          }
          else
            goto label_12;
label_137:
          this.CheckTimeout();
          runstack[--num4] = runtrack[num5++];
          this.Uncapture();
          continue;
label_138:
          this.CheckTimeout();
          int[] numArray31 = runtrack;
          int index34 = num5;
          int num145 = index34 + 1;
          num1 = numArray31[index34];
          int[] numArray32 = runtrack;
          int index35 = num145;
          num5 = index35 + 1;
          int num146 = numArray32[index35];
          if (num146 > 0)
          {
            int num147;
            runtrack[num147 = num5 - 1] = num146 - 1;
            int num148;
            runtrack[num148 = num147 - 1] = num1 - 1;
            runtrack[num5 = num148 - 1] = 4;
            goto label_23;
          }
          else
            goto label_23;
label_140:
          this.CheckTimeout();
          int[] numArray33 = runtrack;
          int index36 = num5;
          int num149 = index36 + 1;
          num1 = numArray33[index36];
          int[] numArray34 = runtrack;
          int index37 = num149;
          num5 = index37 + 1;
          int num150 = numArray34[index37];
          if (num150 > 0)
          {
            int num151;
            runtrack[num151 = num5 - 1] = num150 - 1;
            int num152;
            runtrack[num152 = num151 - 1] = num1 - 1;
            runtrack[num5 = num152 - 1] = 5;
            goto label_30;
          }
          else
            goto label_30;
label_143:
          this.CheckTimeout();
          int[] numArray35 = runtrack;
          int index38 = num5;
          int num153 = index38 + 1;
          num1 = numArray35[index38];
          int[] numArray36 = runtrack;
          int index39 = num153;
          num5 = index39 + 1;
          int num154 = numArray36[index39];
          if (num154 > 0)
          {
            int num155;
            runtrack[num155 = num5 - 1] = num154 - 1;
            int num156;
            runtrack[num156 = num155 - 1] = num1 - 1;
            runtrack[num5 = num156 - 1] = 7;
            goto label_36;
          }
          else
            goto label_36;
label_145:
          this.CheckTimeout();
          int[] numArray37 = runtrack;
          int index40 = num5;
          int num157 = index40 + 1;
          num1 = numArray37[index40];
          int[] numArray38 = runtrack;
          int index41 = num157;
          num5 = index41 + 1;
          int num158 = numArray38[index41];
          if (num158 > 0)
          {
            int num159;
            runtrack[num159 = num5 - 1] = num158 - 1;
            int num160;
            runtrack[num160 = num159 - 1] = num1 - 1;
            runtrack[num5 = num160 - 1] = 8;
            goto label_43;
          }
          else
            goto label_43;
label_147:
          this.CheckTimeout();
          int[] numArray39 = runtrack;
          int index42 = num5;
          int num161 = index42 + 1;
          num1 = numArray39[index42];
          int[] numArray40 = runtrack;
          int index43 = num161;
          num38 = index43 + 1;
          int num162 = numArray40[index43];
          if (num162 > 0)
          {
            int num163;
            runtrack[num163 = num38 - 1] = num162 - 1;
            int num164;
            runtrack[num164 = num163 - 1] = num1 - 1;
            runtrack[num38 = num164 - 1] = 9;
            goto label_50;
          }
          else
            goto label_50;
label_164:
          this.CheckTimeout();
          int[] numArray41 = runtrack;
          int index44 = num5;
          int num165 = index44 + 1;
          num1 = numArray41[index44];
          this.CheckTimeout();
          runstack[--num4] = num1;
          runtrack[num5 = num165 - 1] = 1;
          this.CheckTimeout();
          if ((num74 = runtextend - num1) <= 0)
            goto label_110;
          else
            goto label_109;
label_167:
          this.CheckTimeout();
          int[] numArray42 = runtrack;
          int index45 = num5;
          int num166 = index45 + 1;
          num1 = numArray42[index45];
          int[] numArray43 = runtrack;
          int index46 = num166;
          num5 = index46 + 1;
          int num167 = numArray43[index46];
          if (num167 > 0)
          {
            int num168;
            runtrack[num168 = num5 - 1] = num167 - 1;
            int num169;
            runtrack[num169 = num168 - 1] = num1 - 1;
            runtrack[num5 = num169 - 1] = 20;
            goto label_102;
          }
          else
            goto label_102;
label_169:
          this.CheckTimeout();
          int[] numArray44 = runtrack;
          int index47 = num5;
          int num170 = index47 + 1;
          num1 = numArray44[index47];
          int[] numArray45 = runtrack;
          int index48 = num170;
          num73 = index48 + 1;
          int num171 = numArray45[index48];
          if (num171 > 0)
          {
            int num172;
            runtrack[num172 = num73 - 1] = num171 - 1;
            int num173;
            runtrack[num173 = num172 - 1] = num1 - 1;
            runtrack[num73 = num173 - 1] = 21;
            goto label_108;
          }
          else
            goto label_108;
label_171:
          this.CheckTimeout();
          int[] numArray46 = runtrack;
          int index49 = num5;
          int num174 = index49 + 1;
          int num175 = numArray46[index49];
          int[] numArray47 = runtrack;
          int index50 = num174;
          num5 = index50 + 1;
          num75 = numArray47[index50];
          str1 = runtext;
          index11 = num175;
          num1 = index11 + 1;
        }
        while (!RegexRunner.CharInClass(str1[index11], "\0\0\u0001d"));
        goto label_172;
label_175:
        this.CheckTimeout();
        runstack[--num4] = runtrack[num5++];
        continue;
label_178:
        this.CheckTimeout();
        num4 += 2;
        continue;
label_179:
        this.CheckTimeout();
        int[] numArray48 = runstack;
        int index51 = num4;
        index10 = index51 + 1;
        if ((num76 = numArray48[index51] - 1) < 0)
        {
          runstack[index10] = runtrack[num5++];
          runstack[num4 = index10 - 1] = num76;
          continue;
        }
        goto label_180;
label_182:
        this.CheckTimeout();
        int[] numArray49 = runtrack;
        int index52 = num5;
        int num176 = index52 + 1;
        int num177 = numArray49[index52];
        int[] numArray50 = runstack;
        int index53;
        int num178 = index53 = num4 - 1;
        int[] numArray51 = runtrack;
        int index54 = num176;
        num5 = index54 + 1;
        int num179 = numArray51[index54];
        numArray50[index53] = num179;
        runstack[num4 = num178 - 1] = num177;
      }
label_72:
      this.CheckTimeout();
      goto label_111;
label_89:
      num1 += 2;
      this.CheckTimeout();
      int start9 = runstack[num4++];
      this.Capture(5, start9, num1);
      int num180;
      runtrack[num180 = num5 - 1] = start9;
      runtrack[num5 = num180 - 1] = 3;
      this.CheckTimeout();
      goto label_111;
label_96:
      this.CheckTimeout();
      int num181;
      int num182 = (num181 = runtextend - num1) + 1;
      while (--num182 > 0)
      {
        if (!RegexRunner.CharInClass(runtext[num1++], "\0\0\u0001d"))
        {
          --num1;
          break;
        }
      }
      if (num181 > num182)
      {
        int num183;
        runtrack[num183 = num5 - 1] = num181 - num182 - 1;
        int num184;
        runtrack[num184 = num183 - 1] = num1 - 1;
        runtrack[num5 = num184 - 1] = 20;
      }
label_102:
      this.CheckTimeout();
      runstack[--num4] = num1;
      runtrack[num73 = num5 - 1] = 1;
      this.CheckTimeout();
      int num185;
      int num186 = (num185 = runtextend - num1) + 1;
      while (--num186 > 0)
      {
        if (!RegexRunner.CharInClass(runtext[num1++], "\u0001\u0004\u0001/0=?d"))
        {
          --num1;
          break;
        }
      }
      if (num185 > num186)
      {
        int num187;
        runtrack[num187 = num73 - 1] = num185 - num186 - 1;
        int num188;
        runtrack[num188 = num187 - 1] = num1 - 1;
        runtrack[num73 = num188 - 1] = 21;
      }
label_108:
      this.CheckTimeout();
      int start10 = runstack[num4++];
      this.Capture(5, start10, num1);
      int num189;
      runtrack[num189 = num73 - 1] = start10;
      runtrack[num5 = num189 - 1] = 3;
      this.CheckTimeout();
      goto label_111;
label_109:
      int num190;
      runtrack[num190 = num5 - 1] = num74 - 1;
      int num191;
      runtrack[num191 = num190 - 1] = num1;
      runtrack[num5 = num191 - 1] = 22;
label_110:
      this.CheckTimeout();
      int start11 = runstack[num4++];
      this.Capture(5, start11, num1);
      int num192;
      runtrack[num192 = num5 - 1] = start11;
      runtrack[num5 = num192 - 1] = 3;
      goto label_111;
label_172:
      if (num75 > 0)
      {
        int num193;
        runtrack[num193 = num5 - 1] = num75 - 1;
        int num194;
        runtrack[num194 = num193 - 1] = num1;
        runtrack[num5 = num194 - 1] = 22;
        goto label_110;
      }
      else
        goto label_110;
label_174:
      this.CheckTimeout();
      int[] numArray52 = runtrack;
      int index55 = num5;
      int num195 = index55 + 1;
      num1 = numArray52[index55];
      int num196 = runstack[num4++];
      runtrack[num52 = num195 - 1] = 25;
      goto label_116;
label_176:
      this.CheckTimeout();
      int[] numArray53 = runtrack;
      int index56 = num5;
      int num197 = index56 + 1;
      num1 = numArray53[index56];
      int[] numArray54 = runtrack;
      int index57 = num197;
      num52 = index57 + 1;
      int num198 = numArray54[index57];
      if (num198 > 0)
      {
        int num199;
        runtrack[num199 = num52 - 1] = num198 - 1;
        int num200;
        runtrack[num200 = num199 - 1] = num1 - 1;
        runtrack[num52 = num200 - 1] = 26;
        goto label_122;
      }
      else
        goto label_122;
label_180:
      int[] numArray55 = runstack;
      int index58 = index10;
      num4 = index58 + 1;
      num1 = numArray55[index58];
      int num201;
      runtrack[num201 = num5 - 1] = num76;
      runtrack[num5 = num201 - 1] = 30;
      goto label_129;
    }

    protected override bool FindFirstChar()
    {
      if (this.runtextpos <= this.runtextstart)
        return true;
      this.runtextpos = this.runtextend;
      return false;
    }

    protected override void InitTrackCount() => this.runtrackcount = 53;
  }
}
#endif