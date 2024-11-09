#if NETFRAMEWORK

using System.Text.RegularExpressions;

#nullable disable
namespace System.Web.RegularExpressions
{
  internal class AspExprRegexRunner5 : RegexRunner
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
        if (2 <= runtextend - num1 && runtext[num1] == '<' && runtext[num1 + 1] == '%')
        {
          num1 += 2;
          this.CheckTimeout();
          int num6;
          if ((num6 = runtextend - num1) > 0)
          {
            int num7;
            runtrack[num7 = num5 - 1] = num6 - 1;
            int num8;
            runtrack[num8 = num7 - 1] = num1;
            runtrack[num5 = num8 - 1] = 2;
          }
        }
        else
          goto label_16;
      }
      else
        goto label_16;
label_4:
      this.CheckTimeout();
      int num9;
      int num10;
      if (num1 < runtextend && runtext[num1++] == '=')
      {
        this.CheckTimeout();
        int num11;
        runstack[num11 = num4 - 1] = -1;
        runstack[num9 = num11 - 1] = 0;
        runtrack[num10 = num5 - 1] = 3;
        this.CheckTimeout();
        goto label_9;
      }
      else
        goto label_16;
label_6:
      this.CheckTimeout();
      runstack[--num4] = num1;
      runtrack[--num5] = 1;
      this.CheckTimeout();
      int num12;
      if ((num12 = runtextend - num1) > 0)
      {
        int num13;
        runtrack[num13 = num5 - 1] = num12 - 1;
        int num14;
        runtrack[num14 = num13 - 1] = num1;
        runtrack[num5 = num14 - 1] = 4;
      }
label_8:
      this.CheckTimeout();
      int[] numArray1 = runstack;
      int index1 = num4;
      num9 = index1 + 1;
      int start1 = numArray1[index1];
      this.Capture(1, start1, num1);
      int num15;
      runtrack[num15 = num5 - 1] = start1;
      runtrack[num10 = num15 - 1] = 5;
label_9:
      this.CheckTimeout();
      int[] numArray2 = runstack;
      int index2 = num9;
      int num16 = index2 + 1;
      int num17 = numArray2[index2];
      int[] numArray3 = runstack;
      int index3 = num16;
      num4 = index3 + 1;
      int num18;
      int num19 = num18 = numArray3[index3];
      int num20;
      runtrack[num20 = num10 - 1] = num19;
      int num21 = num1;
      if ((num18 != num21 || num17 < 0) && num17 < 1)
      {
        int num22;
        runstack[num22 = num4 - 1] = num1;
        runstack[num4 = num22 - 1] = num17 + 1;
        runtrack[num5 = num20 - 1] = 6;
        if (num5 <= 40 || num4 <= 30)
        {
          runtrack[--num5] = 7;
          goto label_16;
        }
        else
          goto label_6;
      }
      else
      {
        int num23;
        runtrack[num23 = num20 - 1] = num17;
        runtrack[num5 = num23 - 1] = 8;
      }
label_13:
      this.CheckTimeout();
      int end;
      int num24;
      if (2 <= runtextend - num1 && runtext[num1] == '%' && runtext[num1 + 1] == '>')
      {
        end = num1 + 2;
        this.CheckTimeout();
        int[] numArray4 = runstack;
        int index4 = num4;
        int num25 = index4 + 1;
        int start2 = numArray4[index4];
        this.Capture(0, start2, end);
        int num26;
        runtrack[num26 = num5 - 1] = start2;
        runtrack[num24 = num26 - 1] = 5;
      }
      else
        goto label_16;
label_15:
      this.CheckTimeout();
      this.runtextpos = end;
      return;
label_16:
      int num27 = 0;
      int index5;
      int num28;
      while (true)
      {
        string str1 = null;
        int index6 = 0;
        do
        {
          int num29 = 0;
          string str2 = null;
          int index7 = 0;
          do
          {
            this.runtrackpos = num5;
            this.runstackpos = num4;
            this.EnsureStorage();
            int runtrackpos2 = this.runtrackpos;
            num4 = this.runstackpos;
            runtrack = this.runtrack;
            runstack = this.runstack;
            int[] numArray5 = runtrack;
            int index8 = runtrackpos2;
            num5 = index8 + 1;
            switch (numArray5[index8])
            {
              case 1:
                this.CheckTimeout();
                ++num4;
                continue;
              case 2:
                this.CheckTimeout();
                int[] numArray6 = runtrack;
                int index9 = num5;
                int num30 = index9 + 1;
                int num31 = numArray6[index9];
                int[] numArray7 = runtrack;
                int index10 = num30;
                num5 = index10 + 1;
                num29 = numArray7[index10];
                str2 = runtext;
                index7 = num31;
                num1 = index7 + 1;
                continue;
              case 3:
                goto label_22;
              case 4:
                goto label_23;
              case 5:
                goto label_26;
              case 6:
                goto label_27;
              case 7:
                goto label_6;
              case 8:
                goto label_30;
              default:
                this.CheckTimeout();
                int[] numArray8 = runtrack;
                int index11 = num5;
                num24 = index11 + 1;
                end = numArray8[index11];
                goto label_15;
            }
          }
          while (!RegexRunner.CharInClass(str2[index7], "\0\0\u0001d"));
          if (num29 > 0)
          {
            int num32;
            runtrack[num32 = num5 - 1] = num29 - 1;
            int num33;
            runtrack[num33 = num32 - 1] = num1;
            runtrack[num5 = num33 - 1] = 2;
            goto label_4;
          }
          else
            goto label_4;
label_22:
          this.CheckTimeout();
          num4 += 2;
          continue;
label_23:
          this.CheckTimeout();
          int[] numArray9 = runtrack;
          int index12 = num5;
          int num34 = index12 + 1;
          int num35 = numArray9[index12];
          int[] numArray10 = runtrack;
          int index13 = num34;
          num5 = index13 + 1;
          num27 = numArray10[index13];
          str1 = runtext;
          index6 = num35;
          num1 = index6 + 1;
        }
        while (!RegexRunner.CharInClass(str1[index6], "\0\u0001\0\0"));
        break;
label_26:
        this.CheckTimeout();
        runstack[--num4] = runtrack[num5++];
        this.Uncapture();
        continue;
label_27:
        this.CheckTimeout();
        int[] numArray11 = runstack;
        int index14 = num4;
        index5 = index14 + 1;
        if ((num28 = numArray11[index14] - 1) < 0)
        {
          runstack[index5] = runtrack[num5++];
          runstack[num4 = index5 - 1] = num28;
          continue;
        }
        goto label_28;
label_30:
        this.CheckTimeout();
        int[] numArray12 = runtrack;
        int index15 = num5;
        int num36 = index15 + 1;
        int num37 = numArray12[index15];
        int[] numArray13 = runstack;
        int index16;
        int num38 = index16 = num4 - 1;
        int[] numArray14 = runtrack;
        int index17 = num36;
        num5 = index17 + 1;
        int num39 = numArray14[index17];
        numArray13[index16] = num39;
        runstack[num4 = num38 - 1] = num37;
      }
      if (num27 > 0)
      {
        int num40;
        runtrack[num40 = num5 - 1] = num27 - 1;
        int num41;
        runtrack[num41 = num40 - 1] = num1;
        runtrack[num5 = num41 - 1] = 4;
        goto label_8;
      }
      else
        goto label_8;
label_28:
      int[] numArray15 = runstack;
      int index18 = index5;
      num4 = index18 + 1;
      num1 = numArray15[index18];
      int num42;
      runtrack[num42 = num5 - 1] = num28;
      runtrack[num5 = num42 - 1] = 8;
      goto label_13;
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