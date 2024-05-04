// Decompiled with JetBrains decompiler
// Type: System.Web.RegularExpressions.BindItemExpressionRegexRunner26
// Assembly: System.Web.RegularExpressions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2D011335-7300-40B5-8CDC-1E3EA0A75C61
// Assembly location: C:\Windows\Microsoft.NET\assembly\GAC_MSIL\System.Web.RegularExpressions\v4.0_4.0.0.0__b03f5f7f11d50a3a\System.Web.RegularExpressions.dll
// XML documentation location: C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.8.1\System.Web.RegularExpressions.xml

#if NETFRAMEWORK

using System.Globalization;
using System.Text.RegularExpressions;

#nullable disable
namespace System.Web.RegularExpressions
{
  internal class BindItemExpressionRegexRunner26 : RegexRunner
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
        goto label_23;
label_7:
      this.CheckTimeout();
      int end;
      int num9;
      if (9 <= runtextend - runtextpos && char.ToLower(runtext[runtextpos], CultureInfo.InvariantCulture) == 'b' && char.ToLower(runtext[runtextpos + 1], CultureInfo.InvariantCulture) == 'i' && char.ToLower(runtext[runtextpos + 2], CultureInfo.InvariantCulture) == 'n' && char.ToLower(runtext[runtextpos + 3], CultureInfo.InvariantCulture) == 'd' && char.ToLower(runtext[runtextpos + 4], CultureInfo.InvariantCulture) == 'i' && char.ToLower(runtext[runtextpos + 5], CultureInfo.InvariantCulture) == 't' && char.ToLower(runtext[runtextpos + 6], CultureInfo.InvariantCulture) == 'e' && char.ToLower(runtext[runtextpos + 7], CultureInfo.InvariantCulture) == 'm' && char.ToLower(runtext[runtextpos + 8], CultureInfo.InvariantCulture) == '.')
      {
        end = runtextpos + 9;
        this.CheckTimeout();
        runstack[--num3] = end;
        runtrack[num9 = num4 - 1] = 1;
        this.CheckTimeout();
        int num10;
        int num11 = (num10 = runtextend - end) + 1;
        while (--num11 > 0)
        {
          if (!RegexRunner.CharInClass(char.ToLower(runtext[end++], CultureInfo.InvariantCulture), "\0\u0001\0\0"))
          {
            --end;
            break;
          }
        }
        if (num10 > num11)
        {
          int num12;
          runtrack[num12 = num9 - 1] = num10 - num11 - 1;
          int num13;
          runtrack[num13 = num12 - 1] = end - 1;
          runtrack[num9 = num13 - 1] = 3;
        }
      }
      else
        goto label_23;
label_14:
      this.CheckTimeout();
      int start1 = runstack[num3++];
      this.Capture(1, start1, end);
      int num14;
      runtrack[num14 = num9 - 1] = start1;
      runtrack[num4 = num14 - 1] = 4;
      this.CheckTimeout();
      int num15;
      int num16 = (num15 = runtextend - end) + 1;
      while (--num16 > 0)
      {
        if (!RegexRunner.CharInClass(char.ToLower(runtext[end++], CultureInfo.InvariantCulture), "\0\0\u0001d"))
        {
          --end;
          break;
        }
      }
      if (num15 > num16)
      {
        int num17;
        runtrack[num17 = num4 - 1] = num15 - num16 - 1;
        int num18;
        runtrack[num18 = num17 - 1] = end - 1;
        runtrack[num4 = num18 - 1] = 5;
      }
label_20:
      this.CheckTimeout();
      int num19;
      if (end >= runtextend)
      {
        this.CheckTimeout();
        int[] numArray = runstack;
        int index = num3;
        int num20 = index + 1;
        int start2 = numArray[index];
        this.Capture(0, start2, end);
        int num21;
        runtrack[num21 = num4 - 1] = start2;
        runtrack[num19 = num21 - 1] = 4;
      }
      else
        goto label_23;
label_22:
      this.CheckTimeout();
      this.runtextpos = end;
      return;
label_23:
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
            goto label_26;
          case 3:
            goto label_28;
          case 4:
            this.CheckTimeout();
            runstack[--num3] = runtrack[num4++];
            this.Uncapture();
            continue;
          case 5:
            goto label_31;
          default:
            goto label_24;
        }
      }
label_24:
      this.CheckTimeout();
      int[] numArray1 = runtrack;
      int index1 = num4;
      num19 = index1 + 1;
      end = numArray1[index1];
      goto label_22;
label_26:
      this.CheckTimeout();
      int[] numArray2 = runtrack;
      int index2 = num4;
      int num22 = index2 + 1;
      runtextpos = numArray2[index2];
      int[] numArray3 = runtrack;
      int index3 = num22;
      num4 = index3 + 1;
      int num23 = numArray3[index3];
      if (num23 > 0)
      {
        int num24;
        runtrack[num24 = num4 - 1] = num23 - 1;
        int num25;
        runtrack[num25 = num24 - 1] = runtextpos - 1;
        runtrack[num4 = num25 - 1] = 2;
        goto label_7;
      }
      else
        goto label_7;
label_28:
      this.CheckTimeout();
      int[] numArray4 = runtrack;
      int index4 = num4;
      int num26 = index4 + 1;
      end = numArray4[index4];
      int[] numArray5 = runtrack;
      int index5 = num26;
      num9 = index5 + 1;
      int num27 = numArray5[index5];
      if (num27 > 0)
      {
        int num28;
        runtrack[num28 = num9 - 1] = num27 - 1;
        int num29;
        runtrack[num29 = num28 - 1] = end - 1;
        runtrack[num9 = num29 - 1] = 3;
        goto label_14;
      }
      else
        goto label_14;
label_31:
      this.CheckTimeout();
      int[] numArray6 = runtrack;
      int index6 = num4;
      int num30 = index6 + 1;
      end = numArray6[index6];
      int[] numArray7 = runtrack;
      int index7 = num30;
      num4 = index7 + 1;
      int num31 = numArray7[index7];
      if (num31 > 0)
      {
        int num32;
        runtrack[num32 = num4 - 1] = num31 - 1;
        int num33;
        runtrack[num33 = num32 - 1] = end - 1;
        runtrack[num4 = num33 - 1] = 5;
        goto label_20;
      }
      else
        goto label_20;
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

    protected override void InitTrackCount() => this.runtrackcount = 8;
  }
}

#endif