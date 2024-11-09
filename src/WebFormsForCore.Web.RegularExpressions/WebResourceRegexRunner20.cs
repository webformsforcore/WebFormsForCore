
#if NETFRAMEWORK

using System.Text.RegularExpressions;

#nullable disable
namespace System.Web.RegularExpressions
{
  internal class WebResourceRegexRunner20 : RegexRunner
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
      if (2 <= runtextend - runtextpos && runtext[runtextpos] == '<' && runtext[runtextpos + 1] == '%')
      {
        num5 = runtextpos + 2;
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
        goto label_32;
label_7:
      this.CheckTimeout();
      int index1;
      if (num5 < runtextend)
      {
        string str = runtext;
        int index2 = num5;
        index1 = index2 + 1;
        if (str[index2] == '=')
        {
          this.CheckTimeout();
          int num10;
          int num11 = (num10 = runtextend - index1) + 1;
          while (--num11 > 0)
          {
            if (!RegexRunner.CharInClass(runtext[index1++], "\0\0\u0001d"))
            {
              --index1;
              break;
            }
          }
          if (num10 > num11)
          {
            int num12;
            runtrack[num12 = num4 - 1] = num10 - num11 - 1;
            int num13;
            runtrack[num13 = num12 - 1] = index1 - 1;
            runtrack[num4 = num13 - 1] = 3;
          }
        }
        else
          goto label_32;
      }
      else
        goto label_32;
label_15:
      this.CheckTimeout();
      int num14;
      int num15;
      if (13 <= runtextend - index1 && runtext[index1] == 'W' && runtext[index1 + 1] == 'e' && runtext[index1 + 2] == 'b' && runtext[index1 + 3] == 'R' && runtext[index1 + 4] == 'e' && runtext[index1 + 5] == 's' && runtext[index1 + 6] == 'o' && runtext[index1 + 7] == 'u' && runtext[index1 + 8] == 'r' && runtext[index1 + 9] == 'c' && runtext[index1 + 10] == 'e' && runtext[index1 + 11] == '(' && runtext[index1 + 12] == '"')
      {
        num14 = index1 + 13;
        this.CheckTimeout();
        runstack[--num3] = num14;
        runtrack[num15 = num4 - 1] = 1;
        this.CheckTimeout();
        int num16;
        int num17 = (num16 = runtextend - num14) + 1;
        while (--num17 > 0)
        {
          if (runtext[num14++] == '"')
          {
            --num14;
            break;
          }
        }
        if (num16 > num17)
        {
          int num18;
          runtrack[num18 = num15 - 1] = num16 - num17 - 1;
          int num19;
          runtrack[num19 = num18 - 1] = num14 - 1;
          runtrack[num15 = num19 - 1] = 4;
        }
      }
      else
        goto label_32;
label_22:
      this.CheckTimeout();
      int start1 = runstack[num3++];
      this.Capture(1, start1, num14);
      int num20;
      runtrack[num20 = num15 - 1] = start1;
      runtrack[num4 = num20 - 1] = 5;
      this.CheckTimeout();
      int index3;
      if (2 <= runtextend - num14 && runtext[num14] == '"' && runtext[num14 + 1] == ')')
      {
        index3 = num14 + 2;
        this.CheckTimeout();
        int num21;
        int num22 = (num21 = runtextend - index3) + 1;
        while (--num22 > 0)
        {
          if (!RegexRunner.CharInClass(runtext[index3++], "\0\0\u0001d"))
          {
            --index3;
            break;
          }
        }
        if (num21 > num22)
        {
          int num23;
          runtrack[num23 = num4 - 1] = num21 - num22 - 1;
          int num24;
          runtrack[num24 = num23 - 1] = index3 - 1;
          runtrack[num4 = num24 - 1] = 6;
        }
      }
      else
        goto label_32;
label_29:
      this.CheckTimeout();
      int end;
      int num25;
      if (2 <= runtextend - index3 && runtext[index3] == '%' && runtext[index3 + 1] == '>')
      {
        end = index3 + 2;
        this.CheckTimeout();
        int[] numArray = runstack;
        int index4 = num3;
        int num26 = index4 + 1;
        int start2 = numArray[index4];
        this.Capture(0, start2, end);
        int num27;
        runtrack[num27 = num4 - 1] = start2;
        runtrack[num25 = num27 - 1] = 5;
      }
      else
        goto label_32;
label_31:
      this.CheckTimeout();
      this.runtextpos = end;
      return;
label_32:
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
        int index5 = runtrackpos2;
        num4 = index5 + 1;
        switch (numArray[index5])
        {
          case 1:
            this.CheckTimeout();
            ++num3;
            continue;
          case 2:
            goto label_35;
          case 3:
            goto label_37;
          case 4:
            goto label_39;
          case 5:
            this.CheckTimeout();
            runstack[--num3] = runtrack[num4++];
            this.Uncapture();
            continue;
          case 6:
            goto label_42;
          default:
            goto label_33;
        }
      }
label_33:
      this.CheckTimeout();
      int[] numArray1 = runtrack;
      int index6 = num4;
      num25 = index6 + 1;
      end = numArray1[index6];
      goto label_31;
label_35:
      this.CheckTimeout();
      int[] numArray2 = runtrack;
      int index7 = num4;
      int num28 = index7 + 1;
      num5 = numArray2[index7];
      int[] numArray3 = runtrack;
      int index8 = num28;
      num4 = index8 + 1;
      int num29 = numArray3[index8];
      if (num29 > 0)
      {
        int num30;
        runtrack[num30 = num4 - 1] = num29 - 1;
        int num31;
        runtrack[num31 = num30 - 1] = num5 - 1;
        runtrack[num4 = num31 - 1] = 2;
        goto label_7;
      }
      else
        goto label_7;
label_37:
      this.CheckTimeout();
      int[] numArray4 = runtrack;
      int index9 = num4;
      int num32 = index9 + 1;
      index1 = numArray4[index9];
      int[] numArray5 = runtrack;
      int index10 = num32;
      num4 = index10 + 1;
      int num33 = numArray5[index10];
      if (num33 > 0)
      {
        int num34;
        runtrack[num34 = num4 - 1] = num33 - 1;
        int num35;
        runtrack[num35 = num34 - 1] = index1 - 1;
        runtrack[num4 = num35 - 1] = 3;
        goto label_15;
      }
      else
        goto label_15;
label_39:
      this.CheckTimeout();
      int[] numArray6 = runtrack;
      int index11 = num4;
      int num36 = index11 + 1;
      num14 = numArray6[index11];
      int[] numArray7 = runtrack;
      int index12 = num36;
      num15 = index12 + 1;
      int num37 = numArray7[index12];
      if (num37 > 0)
      {
        int num38;
        runtrack[num38 = num15 - 1] = num37 - 1;
        int num39;
        runtrack[num39 = num38 - 1] = num14 - 1;
        runtrack[num15 = num39 - 1] = 4;
        goto label_22;
      }
      else
        goto label_22;
label_42:
      this.CheckTimeout();
      int[] numArray8 = runtrack;
      int index13 = num4;
      int num40 = index13 + 1;
      index3 = numArray8[index13];
      int[] numArray9 = runtrack;
      int index14 = num40;
      num4 = index14 + 1;
      int num41 = numArray9[index14];
      if (num41 > 0)
      {
        int num42;
        runtrack[num42 = num4 - 1] = num41 - 1;
        int num43;
        runtrack[num43 = num42 - 1] = index3 - 1;
        runtrack[num4 = num43 - 1] = 6;
        goto label_29;
      }
      else
        goto label_29;
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

    protected override void InitTrackCount() => this.runtrackcount = 9;
  }
}
#endif