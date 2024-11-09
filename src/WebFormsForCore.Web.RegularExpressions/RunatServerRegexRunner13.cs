
#if NETFRAMEWORK

using System.Globalization;
using System.Text.RegularExpressions;

#nullable disable
namespace System.Web.RegularExpressions
{
  internal class RunatServerRegexRunner13 : RegexRunner
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
      int index1;
      if (5 <= runtextend - runtextpos && char.ToLower(runtext[runtextpos], CultureInfo.InvariantCulture) == 'r' && char.ToLower(runtext[runtextpos + 1], CultureInfo.InvariantCulture) == 'u' && char.ToLower(runtext[runtextpos + 2], CultureInfo.InvariantCulture) == 'n' && char.ToLower(runtext[runtextpos + 3], CultureInfo.InvariantCulture) == 'a' && char.ToLower(runtext[runtextpos + 4], CultureInfo.InvariantCulture) == 't')
      {
        index1 = runtextpos + 5;
        this.CheckTimeout();
        int num5;
        int num6 = (num5 = runtextend - index1) + 1;
        while (--num6 > 0)
        {
          if (!RegexRunner.CharInClass(char.ToLower(runtext[index1++], CultureInfo.InvariantCulture), "\u0001\0\n\0\u0002\u0004\u0005\u0003\u0001\u0006\t\u0013\0"))
          {
            --index1;
            break;
          }
        }
        if (num5 > num6)
        {
          int num7;
          runtrack[num7 = num4 - 1] = num5 - num6 - 1;
          int num8;
          runtrack[num8 = num7 - 1] = index1 - 1;
          runtrack[num4 = num8 - 1] = 2;
        }
      }
      else
        goto label_10;
label_7:
      this.CheckTimeout();
      int end;
      int num9;
      if (6 <= runtextend - index1 && char.ToLower(runtext[index1], CultureInfo.InvariantCulture) == 's' && char.ToLower(runtext[index1 + 1], CultureInfo.InvariantCulture) == 'e' && char.ToLower(runtext[index1 + 2], CultureInfo.InvariantCulture) == 'r' && char.ToLower(runtext[index1 + 3], CultureInfo.InvariantCulture) == 'v' && char.ToLower(runtext[index1 + 4], CultureInfo.InvariantCulture) == 'e' && char.ToLower(runtext[index1 + 5], CultureInfo.InvariantCulture) == 'r')
      {
        end = index1 + 6;
        this.CheckTimeout();
        int[] numArray = runstack;
        int index2 = num3;
        int num10 = index2 + 1;
        int start = numArray[index2];
        this.Capture(0, start, end);
        int num11;
        runtrack[num11 = num4 - 1] = start;
        runtrack[num9 = num11 - 1] = 3;
      }
      else
        goto label_10;
label_9:
      this.CheckTimeout();
      this.runtextpos = end;
      return;
label_10:
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
        int index3 = runtrackpos2;
        num4 = index3 + 1;
        switch (numArray[index3])
        {
          case 1:
            this.CheckTimeout();
            ++num3;
            continue;
          case 2:
            goto label_13;
          case 3:
            this.CheckTimeout();
            runstack[--num3] = runtrack[num4++];
            this.Uncapture();
            continue;
          default:
            goto label_11;
        }
      }
label_11:
      this.CheckTimeout();
      int[] numArray1 = runtrack;
      int index4 = num4;
      num9 = index4 + 1;
      end = numArray1[index4];
      goto label_9;
label_13:
      this.CheckTimeout();
      int[] numArray2 = runtrack;
      int index5 = num4;
      int num12 = index5 + 1;
      index1 = numArray2[index5];
      int[] numArray3 = runtrack;
      int index6 = num12;
      num4 = index6 + 1;
      int num13 = numArray3[index6];
      if (num13 > 0)
      {
        int num14;
        runtrack[num14 = num4 - 1] = num13 - 1;
        int num15;
        runtrack[num15 = num14 - 1] = index1 - 1;
        runtrack[num4 = num15 - 1] = 2;
        goto label_7;
      }
      else
        goto label_7;
    }

    protected override bool FindFirstChar()
    {
      string runtext = this.runtext;
      int runtextend = this.runtextend;
      int num1;
      int num2;
      for (int index = this.runtextpos + 4; index < runtextend; index = num1 + num2)
      {
        int lower;
        if ((lower = (int) char.ToLower(runtext[index], CultureInfo.InvariantCulture)) == 116)
          goto label_28;
        else
          goto label_5;
label_2:
        num2 = index;
        continue;
label_5:
        int num3;
        if ((uint) (num3 = lower - 97) > 20U)
        {
          num1 = 5;
          goto label_2;
        }
        else
        {
          switch (num3)
          {
            case 1:
              num1 = 5;
              goto label_2;
            case 2:
              num1 = 5;
              goto label_2;
            case 3:
              num1 = 5;
              goto label_2;
            case 4:
              num1 = 5;
              goto label_2;
            case 5:
              num1 = 5;
              goto label_2;
            case 6:
              num1 = 5;
              goto label_2;
            case 7:
              num1 = 5;
              goto label_2;
            case 8:
              num1 = 5;
              goto label_2;
            case 9:
              num1 = 5;
              goto label_2;
            case 10:
              num1 = 5;
              goto label_2;
            case 11:
              num1 = 5;
              goto label_2;
            case 12:
              num1 = 5;
              goto label_2;
            case 13:
              num1 = 2;
              goto label_2;
            case 14:
              num1 = 5;
              goto label_2;
            case 15:
              num1 = 5;
              goto label_2;
            case 16:
              num1 = 5;
              goto label_2;
            case 17:
              num1 = 4;
              goto label_2;
            case 18:
              num1 = 5;
              goto label_2;
            case 19:
              num1 = 0;
              goto label_2;
            case 20:
              num1 = 3;
              goto label_2;
            default:
              num1 = 1;
              goto label_2;
          }
        }
label_28:
        int num4 = index;
        int num5;
        if (char.ToLower(runtext[num5 = num4 - 1], CultureInfo.InvariantCulture) != 'a')
        {
          num1 = 1;
          goto label_2;
        }
        else
        {
          int num6;
          if (char.ToLower(runtext[num6 = num5 - 1], CultureInfo.InvariantCulture) != 'n')
          {
            num1 = 1;
            goto label_2;
          }
          else
          {
            int num7;
            if (char.ToLower(runtext[num7 = num6 - 1], CultureInfo.InvariantCulture) != 'u')
            {
              num1 = 1;
              goto label_2;
            }
            else
            {
              int num8;
              if (char.ToLower(runtext[num8 = num7 - 1], CultureInfo.InvariantCulture) != 'r')
              {
                num1 = 1;
                goto label_2;
              }
              else
              {
                this.runtextpos = num8;
                return true;
              }
            }
          }
        }
      }
      this.runtextpos = this.runtextend;
      return false;
    }

    protected override void InitTrackCount() => this.runtrackcount = 4;
  }
}
#endif