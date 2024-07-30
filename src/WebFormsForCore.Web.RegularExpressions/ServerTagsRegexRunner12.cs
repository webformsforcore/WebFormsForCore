// Decompiled with JetBrains decompiler
// Type: System.Web.RegularExpressions.ServerTagsRegexRunner12
// Assembly: System.Web.RegularExpressions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2D011335-7300-40B5-8CDC-1E3EA0A75C61
// Assembly location: C:\Windows\Microsoft.NET\assembly\GAC_MSIL\System.Web.RegularExpressions\v4.0_4.0.0.0__b03f5f7f11d50a3a\System.Web.RegularExpressions.dll
// XML documentation location: C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.8.1\System.Web.RegularExpressions.xml

#if NETFRAMEWORK

using System.Text.RegularExpressions;

#nullable disable
namespace System.Web.RegularExpressions
{
  internal class ServerTagsRegexRunner12 : RegexRunner
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
      int index1;
      runstack1[index1 = runstackpos - 1] = runtextpos;
      int num3;
      runtrack1[num3 = num2 - 1] = 1;
      this.CheckTimeout();
      if (2 <= runtextend - runtextpos && runtext[runtextpos] == '<' && runtext[runtextpos + 1] == '%')
      {
        int num4 = runtextpos + 2;
        this.CheckTimeout();
        int num5;
        runstack1[num5 = index1 - 1] = this.runtrack.Length - num3;
        runstack1[index1 = num5 - 1] = this.Crawlpos();
        int num6;
        runtrack1[num6 = num3 - 1] = 2;
        this.CheckTimeout();
        int num7;
        runtrack1[num7 = num6 - 1] = num4;
        runtrack1[num3 = num7 - 1] = 3;
        this.CheckTimeout();
        if (num4 < runtextend)
        {
          string str = runtext;
          int index2 = num4;
          int num8 = index2 + 1;
          if (RegexRunner.CharInClass(str[index2], "\0\u0002\0#%"))
          {
            this.CheckTimeout();
            int[] numArray1 = runstack1;
            int index3 = index1;
            int num9 = index3 + 1;
            int num10 = numArray1[index3];
            int length = this.runtrack.Length;
            int[] numArray2 = runstack1;
            int index4 = num9;
            index1 = index4 + 1;
            int num11 = numArray2[index4];
            num3 = length - num11;
            if (num10 != this.Crawlpos())
            {
              do
              {
                this.Uncapture();
              }
              while (num10 != this.Crawlpos());
            }
          }
        }
      }
      int[] runstack2;
      int[] runtrack2;
      int end1;
      int num12;
      while (true)
      {
        int num13;
        string str1;
        int index5;
        do
        {
          int end2 = 0;
          do
          {
            this.runtrackpos = num3;
            this.runstackpos = index1;
            this.EnsureStorage();
            int runtrackpos2 = this.runtrackpos;
            index1 = this.runstackpos;
            runtrack2 = this.runtrack;
            runstack2 = this.runstack;
            int[] numArray3 = runtrack2;
            int index6 = runtrackpos2;
            num3 = index6 + 1;
            int num14;
            int num15;
            int end3;
            int num16;
            switch (numArray3[index6])
            {
              case 1:
                goto label_24;
              case 2:
                goto label_25;
              case 3:
                this.CheckTimeout();
                int[] numArray4 = runtrack2;
                int index7 = num3;
                num12 = index7 + 1;
                end2 = numArray4[index7];
                this.CheckTimeout();
                int[] numArray5 = runstack2;
                int index8 = index1;
                int num17 = index8 + 1;
                int num18 = numArray5[index8];
                int length = this.runtrack.Length;
                int[] numArray6 = runstack2;
                int index9 = num17;
                int num19 = index9 + 1;
                int num20 = numArray6[index9];
                int num21 = length - num20;
                int num22;
                runtrack2[num22 = num21 - 1] = num18;
                int num23;
                runtrack2[num23 = num22 - 1] = 4;
                this.CheckTimeout();
                runstack2[num14 = num19 - 1] = -1;
                runtrack2[num15 = num23 - 1] = 1;
                this.CheckTimeout();
                goto label_15;
              case 4:
                goto label_27;
              case 5:
                this.CheckTimeout();
                int[] numArray7 = runtrack2;
                int index10 = num3;
                int num24 = index10 + 1;
                end3 = numArray7[index10];
                int[] numArray8 = runtrack2;
                int index11 = num24;
                num16 = index11 + 1;
                int num25 = numArray8[index11];
                if (num25 > 0)
                {
                  int num26;
                  runtrack2[num26 = num16 - 1] = num25 - 1;
                  int num27;
                  runtrack2[num27 = num26 - 1] = end3 - 1;
                  runtrack2[num16 = num27 - 1] = 5;
                  break;
                }
                break;
              case 6:
                goto label_32;
              case 7:
                this.CheckTimeout();
                int[] numArray9 = runtrack2;
                int index12 = num3;
                int num28 = index12 + 1;
                end3 = numArray9[index12];
                runstack2[--index1] = end3;
                runtrack2[num13 = num28 - 1] = 8;
                if (num13 > 56 && index1 > 42)
                {
                  this.CheckTimeout();
                  int num29;
                  runstack2[num29 = index1 - 1] = end3;
                  int num30;
                  runtrack2[num30 = num13 - 1] = 1;
                  this.CheckTimeout();
                  runstack2[index1 = num29 - 1] = end3;
                  runtrack2[num16 = num30 - 1] = 1;
                  this.CheckTimeout();
                  int num31;
                  int num32 = (num31 = runtextend - end3) + 1;
                  while (--num32 > 0)
                  {
                    if (runtext[end3++] == '%')
                    {
                      --end3;
                      break;
                    }
                  }
                  if (num31 > num32)
                  {
                    int num33;
                    runtrack2[num33 = num16 - 1] = num31 - num32 - 1;
                    int num34;
                    runtrack2[num34 = num33 - 1] = end3 - 1;
                    runtrack2[num16 = num34 - 1] = 5;
                    break;
                  }
                  break;
                }
                goto label_34;
              case 8:
                goto label_35;
              default:
                goto label_23;
            }
            this.CheckTimeout();
            int start1 = runstack2[index1++];
            this.Capture(2, start1, end3);
            int num35;
            runtrack2[num35 = num16 - 1] = start1;
            runtrack2[num3 = num35 - 1] = 6;
            this.CheckTimeout();
            if (end3 < runtextend)
            {
              string str2 = runtext;
              int index13 = end3;
              end2 = index13 + 1;
              if (str2[index13] == '%')
              {
                this.CheckTimeout();
                int[] numArray10 = runstack2;
                int index14 = index1;
                num14 = index14 + 1;
                int start2 = numArray10[index14];
                this.Capture(1, start2, end2);
                int num36;
                runtrack2[num36 = num3 - 1] = start2;
                runtrack2[num15 = num36 - 1] = 6;
              }
              else
                continue;
            }
            else
              continue;
label_15:
            this.CheckTimeout();
            int[] numArray11 = runstack2;
            int index15 = num14;
            index1 = index15 + 1;
            int num37;
            int num38 = num37 = numArray11[index15];
            int num39;
            runtrack2[num39 = num15 - 1] = num38 == -1 ? end2 : num38;
            int num40 = end2;
            if (num37 != num40)
            {
              int num41;
              runtrack2[num41 = num39 - 1] = end2;
              runtrack2[num3 = num41 - 1] = 7;
            }
            else
            {
              runstack2[--index1] = num38;
              runtrack2[num3 = num39 - 1] = 8;
            }
            this.CheckTimeout();
          }
          while (end2 >= runtextend);
          str1 = runtext;
          index5 = end2;
          end1 = index5 + 1;
        }
        while (str1[index5] != '>');
        break;
label_24:
        this.CheckTimeout();
        ++index1;
        continue;
label_25:
        this.CheckTimeout();
        index1 += 2;
        continue;
label_27:
        this.CheckTimeout();
        int num42 = runtrack2[num3++];
        if (num42 != this.Crawlpos())
        {
          do
          {
            this.Uncapture();
          }
          while (num42 != this.Crawlpos());
        }
        continue;
label_32:
        this.CheckTimeout();
        runstack2[--index1] = runtrack2[num3++];
        this.Uncapture();
        continue;
label_34:
        runtrack2[num3 = num13 - 1] = 9;
        continue;
label_35:
        this.CheckTimeout();
        runstack2[index1] = runtrack2[num3++];
      }
      this.CheckTimeout();
      int[] numArray12 = runstack2;
      int index16 = index1;
      int num43 = index16 + 1;
      int start = numArray12[index16];
      this.Capture(0, start, end1);
      int num44;
      runtrack2[num44 = num3 - 1] = start;
      runtrack2[num12 = num44 - 1] = 6;
label_21:
      this.CheckTimeout();
      this.runtextpos = end1;
      return;
label_23:
      this.CheckTimeout();
      int[] numArray13 = runtrack2;
      int index17 = num3;
      num12 = index17 + 1;
      end1 = numArray13[index17];
      goto label_21;
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

    protected override void InitTrackCount() => this.runtrackcount = 14;
  }
}
#endif