#if NETFRAMEWORK

using System.Text.RegularExpressions;

#nullable disable
namespace System.Web.RegularExpressions
{
  internal class BrowserCapsRefRegexRunner23 : RegexRunner
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
      int end1;
      if (2 <= runtextend - runtextpos && runtext[runtextpos] == '$' && runtext[runtextpos + 1] == '{')
      {
        int num5 = runtextpos + 2;
        this.CheckTimeout();
        runstack[--num3] = num5;
        runtrack[--num4] = 1;
        this.CheckTimeout();
        if (1 <= runtextend - num5)
        {
          end1 = num5 + 1;
          int num6 = 1;
          while (RegexRunner.CharInClass(runtext[end1 - num6--], "\0\0\n\0\u0002\u0004\u0005\u0003\u0001\u0006\t\u0013\0"))
          {
            if (num6 <= 0)
            {
              this.CheckTimeout();
              int num7;
              int num8 = (num7 = runtextend - end1) + 1;
              while (--num8 > 0)
              {
                if (!RegexRunner.CharInClass(runtext[end1++], "\0\0\n\0\u0002\u0004\u0005\u0003\u0001\u0006\t\u0013\0"))
                {
                  --end1;
                  break;
                }
              }
              if (num7 > num8)
              {
                int num9;
                runtrack[num9 = num4 - 1] = num7 - num8 - 1;
                int num10;
                runtrack[num10 = num9 - 1] = end1 - 1;
                runtrack[num4 = num10 - 1] = 2;
                goto label_11;
              }
              else
                goto label_11;
            }
          }
          goto label_15;
        }
        else
          goto label_15;
      }
      else
        goto label_15;
label_11:
      this.CheckTimeout();
      int start1 = runstack[num3++];
      this.Capture(1, start1, end1);
      int num11;
      runtrack[num11 = num4 - 1] = start1;
      runtrack[num4 = num11 - 1] = 3;
      this.CheckTimeout();
      int end2;
      int num12;
      if (end1 < runtextend)
      {
        string str = runtext;
        int index1 = end1;
        end2 = index1 + 1;
        if (str[index1] == '}')
        {
          this.CheckTimeout();
          int[] numArray = runstack;
          int index2 = num3;
          int num13 = index2 + 1;
          int start2 = numArray[index2];
          this.Capture(0, start2, end2);
          int num14;
          runtrack[num14 = num4 - 1] = start2;
          runtrack[num12 = num14 - 1] = 3;
        }
        else
          goto label_15;
      }
      else
        goto label_15;
label_14:
      this.CheckTimeout();
      this.runtextpos = end2;
      return;
label_15:
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
            goto label_18;
          case 3:
            this.CheckTimeout();
            runstack[--num3] = runtrack[num4++];
            this.Uncapture();
            continue;
          default:
            goto label_16;
        }
      }
label_16:
      this.CheckTimeout();
      int[] numArray1 = runtrack;
      int index3 = num4;
      num12 = index3 + 1;
      end2 = numArray1[index3];
      goto label_14;
label_18:
      this.CheckTimeout();
      int[] numArray2 = runtrack;
      int index4 = num4;
      int num15 = index4 + 1;
      end1 = numArray2[index4];
      int[] numArray3 = runtrack;
      int index5 = num15;
      num4 = index5 + 1;
      int num16 = numArray3[index5];
      if (num16 > 0)
      {
        int num17;
        runtrack[num17 = num4 - 1] = num16 - 1;
        int num18;
        runtrack[num18 = num17 - 1] = end1 - 1;
        runtrack[num4 = num18 - 1] = 2;
        goto label_11;
      }
      else
        goto label_11;
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
        if ((num3 = (int) runtext[index]) == 123)
          goto label_95;
        else
          goto label_5;
label_2:
        num2 = index;
        continue;
label_5:
        int num4;
        if ((uint) (num4 = num3 - 36) > 87U)
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
              num1 = 2;
              goto label_2;
            case 24:
              num1 = 2;
              goto label_2;
            case 25:
              num1 = 2;
              goto label_2;
            case 26:
              num1 = 2;
              goto label_2;
            case 27:
              num1 = 2;
              goto label_2;
            case 28:
              num1 = 2;
              goto label_2;
            case 29:
              num1 = 2;
              goto label_2;
            case 30:
              num1 = 2;
              goto label_2;
            case 31:
              num1 = 2;
              goto label_2;
            case 32:
              num1 = 2;
              goto label_2;
            case 33:
              num1 = 2;
              goto label_2;
            case 34:
              num1 = 2;
              goto label_2;
            case 35:
              num1 = 2;
              goto label_2;
            case 36:
              num1 = 2;
              goto label_2;
            case 37:
              num1 = 2;
              goto label_2;
            case 38:
              num1 = 2;
              goto label_2;
            case 39:
              num1 = 2;
              goto label_2;
            case 40:
              num1 = 2;
              goto label_2;
            case 41:
              num1 = 2;
              goto label_2;
            case 42:
              num1 = 2;
              goto label_2;
            case 43:
              num1 = 2;
              goto label_2;
            case 44:
              num1 = 2;
              goto label_2;
            case 45:
              num1 = 2;
              goto label_2;
            case 46:
              num1 = 2;
              goto label_2;
            case 47:
              num1 = 2;
              goto label_2;
            case 48:
              num1 = 2;
              goto label_2;
            case 49:
              num1 = 2;
              goto label_2;
            case 50:
              num1 = 2;
              goto label_2;
            case 51:
              num1 = 2;
              goto label_2;
            case 52:
              num1 = 2;
              goto label_2;
            case 53:
              num1 = 2;
              goto label_2;
            case 54:
              num1 = 2;
              goto label_2;
            case 55:
              num1 = 2;
              goto label_2;
            case 56:
              num1 = 2;
              goto label_2;
            case 57:
              num1 = 2;
              goto label_2;
            case 58:
              num1 = 2;
              goto label_2;
            case 59:
              num1 = 2;
              goto label_2;
            case 60:
              num1 = 2;
              goto label_2;
            case 61:
              num1 = 2;
              goto label_2;
            case 62:
              num1 = 2;
              goto label_2;
            case 63:
              num1 = 2;
              goto label_2;
            case 64:
              num1 = 2;
              goto label_2;
            case 65:
              num1 = 2;
              goto label_2;
            case 66:
              num1 = 2;
              goto label_2;
            case 67:
              num1 = 2;
              goto label_2;
            case 68:
              num1 = 2;
              goto label_2;
            case 69:
              num1 = 2;
              goto label_2;
            case 70:
              num1 = 2;
              goto label_2;
            case 71:
              num1 = 2;
              goto label_2;
            case 72:
              num1 = 2;
              goto label_2;
            case 73:
              num1 = 2;
              goto label_2;
            case 74:
              num1 = 2;
              goto label_2;
            case 75:
              num1 = 2;
              goto label_2;
            case 76:
              num1 = 2;
              goto label_2;
            case 77:
              num1 = 2;
              goto label_2;
            case 78:
              num1 = 2;
              goto label_2;
            case 79:
              num1 = 2;
              goto label_2;
            case 80:
              num1 = 2;
              goto label_2;
            case 81:
              num1 = 2;
              goto label_2;
            case 82:
              num1 = 2;
              goto label_2;
            case 83:
              num1 = 2;
              goto label_2;
            case 84:
              num1 = 2;
              goto label_2;
            case 85:
              num1 = 2;
              goto label_2;
            case 86:
              num1 = 2;
              goto label_2;
            case 87:
              num1 = 0;
              goto label_2;
            default:
              num1 = 1;
              goto label_2;
          }
        }
label_95:
        int num5 = index;
        int num6;
        if (runtext[num6 = num5 - 1] != '$')
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

    protected override void InitTrackCount() => this.runtrackcount = 6;
  }
}

#endif