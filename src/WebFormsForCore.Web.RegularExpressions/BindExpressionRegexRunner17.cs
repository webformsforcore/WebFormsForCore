#if NETFRAMEWORK

using System.Globalization;
using System.Text.RegularExpressions;

#nullable disable
namespace System.Web.RegularExpressions
{
  internal class BindExpressionRegexRunner17 : RegexRunner
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
      if (runtextpos <= runtextbeg || runtext[runtextpos - 1] == '\n')
      {
        this.CheckTimeout();
        int num5;
        int num6 = (num5 = runtextend - runtextpos) + 1;
        while (--num6 > 0)
        {
          if (!RegexRunner.CharInClass(char.ToLower(runtext[runtextpos++], CultureInfo.InvariantCulture), "\0\0\u0001d"))
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
        goto label_33;
label_7:
      this.CheckTimeout();
      int num9;
      if (4 <= runtextend - runtextpos && char.ToLower(runtext[runtextpos], CultureInfo.InvariantCulture) == 'b' && char.ToLower(runtext[runtextpos + 1], CultureInfo.InvariantCulture) == 'i' && char.ToLower(runtext[runtextpos + 2], CultureInfo.InvariantCulture) == 'n' && char.ToLower(runtext[runtextpos + 3], CultureInfo.InvariantCulture) == 'd')
      {
        num9 = runtextpos + 4;
        this.CheckTimeout();
        int num10;
        int num11 = (num10 = runtextend - num9) + 1;
        while (--num11 > 0)
        {
          if (!RegexRunner.CharInClass(char.ToLower(runtext[num9++], CultureInfo.InvariantCulture), "\0\0\u0001d"))
          {
            --num9;
            break;
          }
        }
        if (num10 > num11)
        {
          int num12;
          runtrack[num12 = num4 - 1] = num10 - num11 - 1;
          int num13;
          runtrack[num13 = num12 - 1] = num9 - 1;
          runtrack[num4 = num13 - 1] = 3;
        }
      }
      else
        goto label_33;
label_14:
      this.CheckTimeout();
      int end1;
      int num14;
      if (num9 < runtextend)
      {
        string str = runtext;
        int index = num9;
        end1 = index + 1;
        if (char.ToLower(str[index], CultureInfo.InvariantCulture) == '(')
        {
          this.CheckTimeout();
          runstack[--num3] = end1;
          runtrack[num14 = num4 - 1] = 1;
          this.CheckTimeout();
          int num15;
          int num16 = (num15 = runtextend - end1) + 1;
          while (--num16 > 0)
          {
            if (!RegexRunner.CharInClass(char.ToLower(runtext[end1++], CultureInfo.InvariantCulture), "\0\u0001\0\0"))
            {
              --end1;
              break;
            }
          }
          if (num15 > num16)
          {
            int num17;
            runtrack[num17 = num14 - 1] = num15 - num16 - 1;
            int num18;
            runtrack[num18 = num17 - 1] = end1 - 1;
            runtrack[num14 = num18 - 1] = 4;
          }
        }
        else
          goto label_33;
      }
      else
        goto label_33;
label_22:
      this.CheckTimeout();
      int start1 = runstack[num3++];
      this.Capture(1, start1, end1);
      int num19;
      runtrack[num19 = num14 - 1] = start1;
      runtrack[num4 = num19 - 1] = 5;
      this.CheckTimeout();
      int end2;
      if (end1 < runtextend)
      {
        string str = runtext;
        int index = end1;
        end2 = index + 1;
        if (char.ToLower(str[index], CultureInfo.InvariantCulture) == ')')
        {
          this.CheckTimeout();
          int num20;
          int num21 = (num20 = runtextend - end2) + 1;
          while (--num21 > 0)
          {
            if (!RegexRunner.CharInClass(char.ToLower(runtext[end2++], CultureInfo.InvariantCulture), "\0\0\u0001d"))
            {
              --end2;
              break;
            }
          }
          if (num20 > num21)
          {
            int num22;
            runtrack[num22 = num4 - 1] = num20 - num21 - 1;
            int num23;
            runtrack[num23 = num22 - 1] = end2 - 1;
            runtrack[num4 = num23 - 1] = 6;
          }
        }
        else
          goto label_33;
      }
      else
        goto label_33;
label_30:
      this.CheckTimeout();
      int num24;
      if (end2 >= runtextend)
      {
        this.CheckTimeout();
        int[] numArray = runstack;
        int index = num3;
        int num25 = index + 1;
        int start2 = numArray[index];
        this.Capture(0, start2, end2);
        int num26;
        runtrack[num26 = num4 - 1] = start2;
        runtrack[num24 = num26 - 1] = 5;
      }
      else
        goto label_33;
label_32:
      this.CheckTimeout();
      this.runtextpos = end2;
      return;
label_33:
      while (true)
      {
        this.runtrackpos = num4;
        this.runstackpos = num3;
        this.EnsureStorage();
        int runtrackpos2 = this.runtrackpos;
        num3 = this.runstackpos;
        runtrack = this.runtrack;
        runstack = this.runstack;
        int[] numArray = runtrack;
        int index = runtrackpos2;
        num4 = index + 1;
        switch (numArray[index])
        {
          case 1:
            this.CheckTimeout();
            ++num3;
            continue;
          case 2:
            goto label_36;
          case 3:
            goto label_38;
          case 4:
            goto label_40;
          case 5:
            this.CheckTimeout();
            runstack[--num3] = runtrack[num4++];
            this.Uncapture();
            continue;
          case 6:
            goto label_43;
          default:
            goto label_34;
        }
      }
label_34:
      this.CheckTimeout();
      int[] numArray1 = runtrack;
      int index1 = num4;
      num24 = index1 + 1;
      end2 = numArray1[index1];
      goto label_32;
label_36:
      this.CheckTimeout();
      int[] numArray2 = runtrack;
      int index2 = num4;
      int num27 = index2 + 1;
      runtextpos = numArray2[index2];
      int[] numArray3 = runtrack;
      int index3 = num27;
      num4 = index3 + 1;
      int num28 = numArray3[index3];
      if (num28 > 0)
      {
        int num29;
        runtrack[num29 = num4 - 1] = num28 - 1;
        int num30;
        runtrack[num30 = num29 - 1] = runtextpos - 1;
        runtrack[num4 = num30 - 1] = 2;
        goto label_7;
      }
      else
        goto label_7;
label_38:
      this.CheckTimeout();
      int[] numArray4 = runtrack;
      int index4 = num4;
      int num31 = index4 + 1;
      num9 = numArray4[index4];
      int[] numArray5 = runtrack;
      int index5 = num31;
      num4 = index5 + 1;
      int num32 = numArray5[index5];
      if (num32 > 0)
      {
        int num33;
        runtrack[num33 = num4 - 1] = num32 - 1;
        int num34;
        runtrack[num34 = num33 - 1] = num9 - 1;
        runtrack[num4 = num34 - 1] = 3;
        goto label_14;
      }
      else
        goto label_14;
label_40:
      this.CheckTimeout();
      int[] numArray6 = runtrack;
      int index6 = num4;
      int num35 = index6 + 1;
      end1 = numArray6[index6];
      int[] numArray7 = runtrack;
      int index7 = num35;
      num14 = index7 + 1;
      int num36 = numArray7[index7];
      if (num36 > 0)
      {
        int num37;
        runtrack[num37 = num14 - 1] = num36 - 1;
        int num38;
        runtrack[num38 = num37 - 1] = end1 - 1;
        runtrack[num14 = num38 - 1] = 4;
        goto label_22;
      }
      else
        goto label_22;
label_43:
      this.CheckTimeout();
      int[] numArray8 = runtrack;
      int index8 = num4;
      int num39 = index8 + 1;
      end2 = numArray8[index8];
      int[] numArray9 = runtrack;
      int index9 = num39;
      num4 = index9 + 1;
      int num40 = numArray9[index9];
      if (num40 > 0)
      {
        int num41;
        runtrack[num41 = num4 - 1] = num40 - 1;
        int num42;
        runtrack[num42 = num41 - 1] = end2 - 1;
        runtrack[num4 = num42 - 1] = 6;
        goto label_30;
      }
      else
        goto label_30;
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
        if (RegexRunner.CharInClass(char.ToLower(runtext[runtextpos++], CultureInfo.InvariantCulture), "\0\u0002\u0001bcd"))
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
