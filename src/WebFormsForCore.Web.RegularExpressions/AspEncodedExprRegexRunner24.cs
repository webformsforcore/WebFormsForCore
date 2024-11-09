#if NETFRAMEWORK

using System.Text.RegularExpressions;

#nullable disable
namespace System.Web.RegularExpressions
{
  internal class AspEncodedExprRegexRunner24 : RegexRunner
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
        if (3 <= runtextend - num1 && runtext[num1] == '<' && runtext[num1 + 1] == '%' && runtext[num1 + 2] == ':')
        {
          num1 += 3;
          this.CheckTimeout();
          int num8;
          runstack[num8 = num4 - 1] = -1;
          runstack[num6 = num8 - 1] = 0;
          runtrack[num7 = num5 - 1] = 2;
          this.CheckTimeout();
          goto label_6;
        }
        else
          goto label_13;
      }
      else
        goto label_13;
label_3:
      this.CheckTimeout();
      runstack[--num4] = num1;
      runtrack[--num5] = 1;
      this.CheckTimeout();
      int num9;
      if ((num9 = runtextend - num1) > 0)
      {
        int num10;
        runtrack[num10 = num5 - 1] = num9 - 1;
        int num11;
        runtrack[num11 = num10 - 1] = num1;
        runtrack[num5 = num11 - 1] = 3;
      }
label_5:
      this.CheckTimeout();
      int[] numArray1 = runstack;
      int index1 = num4;
      num6 = index1 + 1;
      int start1 = numArray1[index1];
      this.Capture(1, start1, num1);
      int num12;
      runtrack[num12 = num5 - 1] = start1;
      runtrack[num7 = num12 - 1] = 4;
label_6:
      this.CheckTimeout();
      int[] numArray2 = runstack;
      int index2 = num6;
      int num13 = index2 + 1;
      int num14 = numArray2[index2];
      int[] numArray3 = runstack;
      int index3 = num13;
      num4 = index3 + 1;
      int num15;
      int num16 = num15 = numArray3[index3];
      int num17;
      runtrack[num17 = num7 - 1] = num16;
      int num18 = num1;
      if ((num15 != num18 || num14 < 0) && num14 < 1)
      {
        int num19;
        runstack[num19 = num4 - 1] = num1;
        runstack[num4 = num19 - 1] = num14 + 1;
        runtrack[num5 = num17 - 1] = 5;
        if (num5 <= 36 || num4 <= 27)
        {
          runtrack[--num5] = 6;
          goto label_13;
        }
        else
          goto label_3;
      }
      else
      {
        int num20;
        runtrack[num20 = num17 - 1] = num14;
        runtrack[num5 = num20 - 1] = 7;
      }
label_10:
      this.CheckTimeout();
      int end;
      int num21;
      if (2 <= runtextend - num1 && runtext[num1] == '%' && runtext[num1 + 1] == '>')
      {
        end = num1 + 2;
        this.CheckTimeout();
        int[] numArray4 = runstack;
        int index4 = num4;
        int num22 = index4 + 1;
        int start2 = numArray4[index4];
        this.Capture(0, start2, end);
        int num23;
        runtrack[num23 = num5 - 1] = start2;
        runtrack[num21 = num23 - 1] = 4;
      }
      else
        goto label_13;
label_12:
      this.CheckTimeout();
      this.runtextpos = end;
      return;
label_13:
      int num24 = 0;
      int index5;
      int num25;
      while (true)
      {
        string str = null;
        int index6 = 0;
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
          int index7 = runtrackpos2;
          num5 = index7 + 1;
          switch (numArray5[index7])
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
              int[] numArray6 = runtrack;
              int index8 = num5;
              int num26 = index8 + 1;
              int num27 = numArray6[index8];
              int[] numArray7 = runtrack;
              int index9 = num26;
              num5 = index9 + 1;
              num24 = numArray7[index9];
              str = runtext;
              index6 = num27;
              num1 = index6 + 1;
              continue;
            case 4:
              goto label_20;
            case 5:
              goto label_21;
            case 6:
              goto label_3;
            case 7:
              goto label_24;
            default:
              this.CheckTimeout();
              int[] numArray8 = runtrack;
              int index10 = num5;
              num21 = index10 + 1;
              end = numArray8[index10];
              goto label_12;
          }
        }
        while (!RegexRunner.CharInClass(str[index6], "\0\u0001\0\0"));
        break;
label_20:
        this.CheckTimeout();
        runstack[--num4] = runtrack[num5++];
        this.Uncapture();
        continue;
label_21:
        this.CheckTimeout();
        int[] numArray9 = runstack;
        int index11 = num4;
        index5 = index11 + 1;
        if ((num25 = numArray9[index11] - 1) < 0)
        {
          runstack[index5] = runtrack[num5++];
          runstack[num4 = index5 - 1] = num25;
          continue;
        }
        goto label_22;
label_24:
        this.CheckTimeout();
        int[] numArray10 = runtrack;
        int index12 = num5;
        int num28 = index12 + 1;
        int num29 = numArray10[index12];
        int[] numArray11 = runstack;
        int index13;
        int num30 = index13 = num4 - 1;
        int[] numArray12 = runtrack;
        int index14 = num28;
        num5 = index14 + 1;
        int num31 = numArray12[index14];
        numArray11[index13] = num31;
        runstack[num4 = num30 - 1] = num29;
      }
      if (num24 > 0)
      {
        int num32;
        runtrack[num32 = num5 - 1] = num24 - 1;
        int num33;
        runtrack[num33 = num32 - 1] = num1;
        runtrack[num5 = num33 - 1] = 3;
        goto label_5;
      }
      else
        goto label_5;
label_22:
      int[] numArray13 = runstack;
      int index15 = index5;
      num4 = index15 + 1;
      num1 = numArray13[index15];
      int num34;
      runtrack[num34 = num5 - 1] = num25;
      runtrack[num5 = num34 - 1] = 7;
      goto label_10;
    }

    protected override bool FindFirstChar()
    {
      if (this.runtextpos <= this.runtextstart)
        return true;
      this.runtextpos = this.runtextend;
      return false;
    }

    protected override void InitTrackCount() => this.runtrackcount = 9;
  }
}

#endif