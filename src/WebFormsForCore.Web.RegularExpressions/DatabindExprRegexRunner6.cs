#if NETFRAMEWORK

using System.Text.RegularExpressions;

#nullable disable
namespace System.Web.RegularExpressions
{
  internal class DatabindExprRegexRunner6 : RegexRunner
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
      int num6;
      int num7;
      if (num1 == this.runtextstart)
      {
        this.CheckTimeout();
        if (3 <= runtextend - num1 && runtext[num1] == '<' && runtext[num1 + 1] == '%' && runtext[num1 + 2] == '#')
        {
          num1 += 3;
          this.CheckTimeout();
          int num8;
          runstack[num8 = num4 - 1] = -1;
          runstack[num6 = num8 - 1] = 0;
          runtrack[num7 = num5 - 1] = 2;
          this.CheckTimeout();
          goto label_5;
        }
        else
          goto label_20;
      }
      else
        goto label_20;
label_3:
      this.CheckTimeout();
      runstack[--num4] = num1;
      runtrack[--num5] = 1;
      this.CheckTimeout();
      if (num1 < runtextend && runtext[num1++] == ':')
      {
        this.CheckTimeout();
        int[] numArray = runstack;
        int index = num4;
        num6 = index + 1;
        int start = numArray[index];
        this.Capture(1, start, num1);
        int num9;
        runtrack[num9 = num5 - 1] = start;
        runtrack[num7 = num9 - 1] = 3;
      }
      else
        goto label_20;
label_5:
      this.CheckTimeout();
      int[] numArray1 = runstack;
      int index1 = num6;
      int num10 = index1 + 1;
      int num11 = numArray1[index1];
      int[] numArray2 = runstack;
      int index2 = num10;
      int num12 = index2 + 1;
      int num13;
      int num14 = num13 = numArray2[index2];
      int num15;
      runtrack[num15 = num7 - 1] = num14;
      int num16 = num1;
      int num17;
      if ((num13 != num16 || num11 < 0) && num11 < 1)
      {
        int num18;
        runstack[num18 = num12 - 1] = num1;
        runstack[num4 = num18 - 1] = num11 + 1;
        runtrack[num5 = num15 - 1] = 4;
        if (num5 <= 56 || num4 <= 42)
        {
          runtrack[--num5] = 5;
          goto label_20;
        }
        else
          goto label_3;
      }
      else
      {
        int num19;
        runtrack[num19 = num15 - 1] = num11;
        runtrack[num17 = num19 - 1] = 6;
      }
label_9:
      this.CheckTimeout();
      int num20;
      runstack[num20 = num12 - 1] = -1;
      int num21;
      runstack[num21 = num20 - 1] = 0;
      int num22;
      runtrack[num22 = num17 - 1] = 2;
      this.CheckTimeout();
      goto label_13;
label_10:
      this.CheckTimeout();
      runstack[--num4] = num1;
      runtrack[--num5] = 1;
      this.CheckTimeout();
      int num23;
      if ((num23 = runtextend - num1) > 0)
      {
        int num24;
        runtrack[num24 = num5 - 1] = num23 - 1;
        int num25;
        runtrack[num25 = num24 - 1] = num1;
        runtrack[num5 = num25 - 1] = 7;
      }
label_12:
      this.CheckTimeout();
      int[] numArray3 = runstack;
      int index3 = num4;
      num21 = index3 + 1;
      int start1 = numArray3[index3];
      this.Capture(2, start1, num1);
      int num26;
      runtrack[num26 = num5 - 1] = start1;
      runtrack[num22 = num26 - 1] = 3;
label_13:
      this.CheckTimeout();
      int[] numArray4 = runstack;
      int index4 = num21;
      int num27 = index4 + 1;
      int num28 = numArray4[index4];
      int[] numArray5 = runstack;
      int index5 = num27;
      num4 = index5 + 1;
      int num29;
      int num30 = num29 = numArray5[index5];
      int num31;
      runtrack[num31 = num22 - 1] = num30;
      int num32 = num1;
      if ((num29 != num32 || num28 < 0) && num28 < 1)
      {
        int num33;
        runstack[num33 = num4 - 1] = num1;
        runstack[num4 = num33 - 1] = num28 + 1;
        runtrack[num5 = num31 - 1] = 8;
        if (num5 <= 56 || num4 <= 42)
        {
          runtrack[--num5] = 9;
          goto label_20;
        }
        else
          goto label_10;
      }
      else
      {
        int num34;
        runtrack[num34 = num31 - 1] = num28;
        runtrack[num5 = num34 - 1] = 6;
      }
label_17:
      this.CheckTimeout();
      int end;
      int num35;
      if (2 <= runtextend - num1 && runtext[num1] == '%' && runtext[num1 + 1] == '>')
      {
        end = num1 + 2;
        this.CheckTimeout();
        int[] numArray6 = runstack;
        int index6 = num4;
        int num36 = index6 + 1;
        int start2 = numArray6[index6];
        this.Capture(0, start2, end);
        int num37;
        runtrack[num37 = num5 - 1] = start2;
        runtrack[num35 = num37 - 1] = 3;
      }
      else
        goto label_20;
label_19:
      this.CheckTimeout();
      this.runtextpos = end;
      return;
label_20:
      int num38 = 0;
      int index7;
      int num39;
      while (true)
      {
        string str = null;
        int index8 = 0;
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
          int index9 = runtrackpos2;
          num5 = index9 + 1;
          switch (numArray7[index9])
          {
            case 1:
              this.CheckTimeout();
              ++num4;
              continue;
            case 2:
              this.CheckTimeout();
              num4 += 2;
              continue;
            case 3:
              this.CheckTimeout();
              runstack[--num4] = runtrack[num5++];
              this.Uncapture();
              continue;
            case 4:
              this.CheckTimeout();
              int[] numArray8 = runstack;
              int index10 = num4;
              int index11 = index10 + 1;
              int num40;
              if ((num40 = numArray8[index10] - 1) >= 0)
              {
                int[] numArray9 = runstack;
                int index12 = index11;
                num12 = index12 + 1;
                num1 = numArray9[index12];
                int num41;
                runtrack[num41 = num5 - 1] = num40;
                runtrack[num17 = num41 - 1] = 6;
                goto label_9;
              }
              else
              {
                runstack[index11] = runtrack[num5++];
                runstack[num4 = index11 - 1] = num40;
                continue;
              }
            case 5:
              goto label_3;
            case 6:
              this.CheckTimeout();
              int[] numArray10 = runtrack;
              int index13 = num5;
              int num42 = index13 + 1;
              int num43 = numArray10[index13];
              int[] numArray11 = runstack;
              int index14;
              int num44 = index14 = num4 - 1;
              int[] numArray12 = runtrack;
              int index15 = num42;
              num5 = index15 + 1;
              int num45 = numArray12[index15];
              numArray11[index14] = num45;
              runstack[num4 = num44 - 1] = num43;
              continue;
            case 7:
              this.CheckTimeout();
              int[] numArray13 = runtrack;
              int index16 = num5;
              int num46 = index16 + 1;
              int num47 = numArray13[index16];
              int[] numArray14 = runtrack;
              int index17 = num46;
              num5 = index17 + 1;
              num38 = numArray14[index17];
              str = runtext;
              index8 = num47;
              num1 = index8 + 1;
              continue;
            case 8:
              goto label_32;
            case 9:
              goto label_10;
            default:
              this.CheckTimeout();
              int[] numArray15 = runtrack;
              int index18 = num5;
              num35 = index18 + 1;
              end = numArray15[index18];
              goto label_19;
          }
        }
        while (!RegexRunner.CharInClass(str[index8], "\0\u0001\0\0"));
        break;
label_32:
        this.CheckTimeout();
        int[] numArray16 = runstack;
        int index19 = num4;
        index7 = index19 + 1;
        if ((num39 = numArray16[index19] - 1) < 0)
        {
          runstack[index7] = runtrack[num5++];
          runstack[num4 = index7 - 1] = num39;
        }
        else
          goto label_33;
      }
      if (num38 > 0)
      {
        int num48;
        runtrack[num48 = num5 - 1] = num38 - 1;
        int num49;
        runtrack[num49 = num48 - 1] = num1;
        runtrack[num5 = num49 - 1] = 7;
        goto label_12;
      }
      else
        goto label_12;
label_33:
      int[] numArray17 = runstack;
      int index20 = index7;
      num4 = index20 + 1;
      num1 = numArray17[index20];
      int num50;
      runtrack[num50 = num5 - 1] = num39;
      runtrack[num5 = num50 - 1] = 6;
      goto label_17;
    }

    protected override bool FindFirstChar()
    {
      if (this.runtextpos <= this.runtextstart)
        return true;
      this.runtextpos = this.runtextend;
      return false;
    }

    protected override void InitTrackCount() => this.runtrackcount = 14;
  }
}
#endif