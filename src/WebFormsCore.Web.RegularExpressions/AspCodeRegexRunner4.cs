// Decompiled with JetBrains decompiler
// Type: System.Web.RegularExpressions.AspCodeRegexRunner4
// Assembly: System.Web.RegularExpressions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2D011335-7300-40B5-8CDC-1E3EA0A75C61
// Assembly location: C:\Windows\Microsoft.NET\assembly\GAC_MSIL\System.Web.RegularExpressions\v4.0_4.0.0.0__b03f5f7f11d50a3a\System.Web.RegularExpressions.dll
// XML documentation location: C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.8.1\System.Web.RegularExpressions.xml

#if NETFRAMEWORK

using System.Text.RegularExpressions;

#nullable disable
namespace System.Web.RegularExpressions
{
  internal class AspCodeRegexRunner4 : RegexRunner
  {
    protected override void Go()
    {
      string runtext = this.runtext;
      int runtextstart = this.runtextstart;
      int runtextbeg = this.runtextbeg;
      int runtextend = this.runtextend;
      int runtextpos = this.runtextpos;
      int[] runtrack1 = this.runtrack;
      int runtrackpos1 = this.runtrackpos;
      int[] runstack1 = this.runstack;
      int runstackpos = this.runstackpos;
      this.CheckTimeout();
      int num1;
      runtrack1[num1 = runtrackpos1 - 1] = runtextpos;
      int num2;
      runtrack1[num2 = num1 - 1] = 0;
      this.CheckTimeout();
      int num3;
      runstack1[num3 = runstackpos - 1] = runtextpos;
      int num4;
      runtrack1[num4 = num2 - 1] = 1;
      this.CheckTimeout();
      if (runtextpos == this.runtextstart)
      {
        this.CheckTimeout();
        if (2 <= runtextend - runtextpos && runtext[runtextpos] == '<' && runtext[runtextpos + 1] == '%')
        {
          int num5 = runtextpos + 2;
          this.CheckTimeout();
          int num6;
          runstack1[num6 = num3 - 1] = this.runtrack.Length - num4;
          runstack1[num3 = num6 - 1] = this.Crawlpos();
          int num7;
          runtrack1[num7 = num4 - 1] = 2;
          this.CheckTimeout();
          int num8;
          runtrack1[num8 = num7 - 1] = num5;
          runtrack1[num4 = num8 - 1] = 3;
          this.CheckTimeout();
          if (num5 < runtextend)
          {
            string str = runtext;
            int index1 = num5;
            int num9 = index1 + 1;
            if (str[index1] == '@')
            {
              this.CheckTimeout();
              int[] numArray1 = runstack1;
              int index2 = num3;
              int num10 = index2 + 1;
              int num11 = numArray1[index2];
              int length = this.runtrack.Length;
              int[] numArray2 = runstack1;
              int index3 = num10;
              num3 = index3 + 1;
              int num12 = numArray2[index3];
              num4 = length - num12;
              if (num11 != this.Crawlpos())
              {
                do
                {
                  this.Uncapture();
                }
                while (num11 != this.Crawlpos());
              }
            }
          }
        }
      }
      int[] runstack2;
      int[] runtrack2;
      int num13;
      int num14;
      while (true)
      {
        do
        {
          this.runtrackpos = num4;
          this.runstackpos = num3;
          this.EnsureStorage();
          int runtrackpos2 = this.runtrackpos;
          num3 = this.runstackpos;
          runtrack2 = this.runtrack;
          runstack2 = this.runstack;
          int[] numArray3 = runtrack2;
          int index4 = runtrackpos2;
          num4 = index4 + 1;
          switch (numArray3[index4])
          {
            case 1:
              goto label_13;
            case 2:
              goto label_14;
            case 3:
              this.CheckTimeout();
              int[] numArray4 = runtrack2;
              int index5 = num4;
              num14 = index5 + 1;
              num13 = numArray4[index5];
              this.CheckTimeout();
              int[] numArray5 = runstack2;
              int index6 = num3;
              int num15 = index6 + 1;
              int num16 = numArray5[index6];
              int length = this.runtrack.Length;
              int[] numArray6 = runstack2;
              int index7 = num15;
              int num17 = index7 + 1;
              int num18 = numArray6[index7];
              int num19 = length - num18;
              int num20;
              runtrack2[num20 = num19 - 1] = num16;
              int num21;
              runtrack2[num21 = num20 - 1] = 4;
              this.CheckTimeout();
              runstack2[num3 = num17 - 1] = num13;
              runtrack2[num4 = num21 - 1] = 1;
              this.CheckTimeout();
              int num22;
              if ((num22 = runtextend - num13) > 0)
              {
                int num23;
                runtrack2[num23 = num4 - 1] = num22 - 1;
                int num24;
                runtrack2[num24 = num23 - 1] = num13;
                runtrack2[num4 = num24 - 1] = 5;
                break;
              }
              break;
            case 4:
              goto label_16;
            case 5:
              this.CheckTimeout();
              int[] numArray7 = runtrack2;
              int index8 = num4;
              int num25 = index8 + 1;
              int num26 = numArray7[index8];
              int[] numArray8 = runtrack2;
              int index9 = num25;
              num4 = index9 + 1;
              int num27 = numArray8[index9];
              string str = runtext;
              int index10 = num26;
              num13 = index10 + 1;
              if (RegexRunner.CharInClass(str[index10], "\0\u0001\0\0"))
              {
                if (num27 > 0)
                {
                  int num28;
                  runtrack2[num28 = num4 - 1] = num27 - 1;
                  int num29;
                  runtrack2[num29 = num28 - 1] = num13;
                  runtrack2[num4 = num29 - 1] = 5;
                  break;
                }
                break;
              }
              continue;
            case 6:
              goto label_22;
            default:
              goto label_12;
          }
          this.CheckTimeout();
          int start = runstack2[num3++];
          this.Capture(1, start, num13);
          int num30;
          runtrack2[num30 = num4 - 1] = start;
          runtrack2[num4 = num30 - 1] = 6;
          this.CheckTimeout();
        }
        while (2 > runtextend - num13 || runtext[num13] != '%' || runtext[num13 + 1] != '>');
        break;
label_13:
        this.CheckTimeout();
        ++num3;
        continue;
label_14:
        this.CheckTimeout();
        num3 += 2;
        continue;
label_16:
        this.CheckTimeout();
        int num31 = runtrack2[num4++];
        if (num31 != this.Crawlpos())
        {
          do
          {
            this.Uncapture();
          }
          while (num31 != this.Crawlpos());
        }
        continue;
label_22:
        this.CheckTimeout();
        runstack2[--num3] = runtrack2[num4++];
        this.Uncapture();
      }
      int end = num13 + 2;
      this.CheckTimeout();
      int[] numArray9 = runstack2;
      int index11 = num3;
      int num32 = index11 + 1;
      int start1 = numArray9[index11];
      this.Capture(0, start1, end);
      int num33;
      runtrack2[num33 = num4 - 1] = start1;
      runtrack2[num14 = num33 - 1] = 6;
label_10:
      this.CheckTimeout();
      this.runtextpos = end;
      return;
label_12:
      this.CheckTimeout();
      int[] numArray10 = runtrack2;
      int index12 = num4;
      num14 = index12 + 1;
      end = numArray10[index12];
      goto label_10;
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