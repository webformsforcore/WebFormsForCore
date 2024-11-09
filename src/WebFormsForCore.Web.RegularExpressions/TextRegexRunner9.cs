
#if NETFRAMEWORK

using System.Text.RegularExpressions;

#nullable disable
namespace System.Web.RegularExpressions
{
  internal class TextRegexRunner9 : RegexRunner
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
      int end;
      if (runtextpos == this.runtextstart)
      {
        this.CheckTimeout();
        if (1 <= runtextend - runtextpos)
        {
          end = runtextpos + 1;
          int num5 = 1;
          while (runtext[end - num5--] != '<')
          {
            if (num5 <= 0)
            {
              this.CheckTimeout();
              int num6;
              int num7 = (num6 = runtextend - end) + 1;
              while (--num7 > 0)
              {
                if (runtext[end++] == '<')
                {
                  --end;
                  break;
                }
              }
              if (num6 > num7)
              {
                int num8;
                runtrack[num8 = num4 - 1] = num6 - num7 - 1;
                int num9;
                runtrack[num9 = num8 - 1] = end - 1;
                runtrack[num4 = num9 - 1] = 2;
                goto label_11;
              }
              else
                goto label_11;
            }
          }
          goto label_13;
        }
        else
          goto label_13;
      }
      else
        goto label_13;
label_11:
      this.CheckTimeout();
      int[] numArray1 = runstack;
      int index1 = num3;
      int num10 = index1 + 1;
      int start = numArray1[index1];
      this.Capture(0, start, end);
      int num11;
      runtrack[num11 = num4 - 1] = start;
      int num12;
      runtrack[num12 = num11 - 1] = 3;
label_12:
      this.CheckTimeout();
      this.runtextpos = end;
      return;
label_13:
      while (true)
      {
        this.runtrackpos = num4;
        this.runstackpos = num3;
        this.EnsureStorage();
        int runtrackpos2 = this.runtrackpos;
        num3 = this.runstackpos;
        runtrack = this.runtrack;
        runstack = this.runstack;
        int[] numArray2 = runtrack;
        int index2 = runtrackpos2;
        num4 = index2 + 1;
        switch (numArray2[index2])
        {
          case 1:
            this.CheckTimeout();
            ++num3;
            continue;
          case 2:
            goto label_16;
          case 3:
            this.CheckTimeout();
            runstack[--num3] = runtrack[num4++];
            this.Uncapture();
            continue;
          default:
            goto label_14;
        }
      }
label_14:
      this.CheckTimeout();
      int[] numArray3 = runtrack;
      int index3 = num4;
      num12 = index3 + 1;
      end = numArray3[index3];
      goto label_12;
label_16:
      this.CheckTimeout();
      int[] numArray4 = runtrack;
      int index4 = num4;
      int num13 = index4 + 1;
      end = numArray4[index4];
      int[] numArray5 = runtrack;
      int index5 = num13;
      num4 = index5 + 1;
      int num14 = numArray5[index5];
      if (num14 > 0)
      {
        int num15;
        runtrack[num15 = num4 - 1] = num14 - 1;
        int num16;
        runtrack[num16 = num15 - 1] = end - 1;
        runtrack[num4 = num16 - 1] = 2;
        goto label_11;
      }
      else
        goto label_11;
    }

    protected override bool FindFirstChar()
    {
      if (this.runtextpos <= this.runtextstart)
        return true;
      this.runtextpos = this.runtextend;
      return false;
    }

    protected override void InitTrackCount() => this.runtrackcount = 4;
  }
}
#endif