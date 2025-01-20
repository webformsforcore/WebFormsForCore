
using Microsoft.ReportingServices.Rendering.RPLProcessing;

#nullable disable
namespace Microsoft.ReportingServices.Rendering.HtmlRenderer
{
  internal class EnumStrings
  {
    public static string GetValue(RPLFormat.FontStyles val)
    {
      switch ((int) val)
      {
        case 0:
          return "normal";
        case 1:
          return "italic";
        default:
          return (string) null;
      }
    }

    public static string GetValue(RPLFormat.FontWeights val)
    {
      switch ((int) val)
      {
        case 0:
          return "400";
        case 1:
          return "100";
        case 2:
          return "200";
        case 3:
          return "300";
        case 4:
          return "500";
        case 5:
          return "600";
        case 6:
          return "700";
        case 7:
          return "800";
        case 8:
          return "900";
        default:
          return (string) null;
      }
    }

    public static string GetValue(RPLFormat.TextDecorations val)
    {
      switch ((int) val)
      {
        case 0:
          return "none";
        case 1:
          return "underline";
        case 2:
          return "overline";
        case 3:
          return "line-through";
        default:
          return (string) null;
      }
    }

    public static string GetValue(RPLFormat.TextAlignments val)
    {
      switch (val - 1)
      {
        case 0:
          return "left";
        case 1:
          return "center";
        case 2:
          return "right";
        default:
          return (string) null;
      }
    }

    public static string GetValue(RPLFormat.VerticalAlignments val)
    {
      switch ((int) val)
      {
        case 0:
          return "top";
        case 1:
          return "middle";
        case 2:
          return "bottom";
        default:
          return (string) null;
      }
    }

    public static string GetValue(RPLFormat.Directions val)
    {
      switch ((int) val)
      {
        case 0:
          return "ltr";
        case 1:
          return "rtl";
        default:
          return (string) null;
      }
    }

    public static string GetValue(RPLFormat.UnicodeBiDiTypes val)
    {
      switch ((int) val)
      {
        case 0:
          return "normal";
        case 1:
          return "embed";
        case 2:
          return "bidi-override";
        default:
          return (string) null;
      }
    }

    public static string GetValue(RPLFormat.BorderStyles val)
    {
      switch ((int) val)
      {
        case 0:
          return "none";
        case 1:
          return "dotted";
        case 2:
          return "dashed";
        case 3:
          return "solid";
        case 4:
          return "double";
        default:
          return (string) null;
      }
    }

    public static string GetValue(RPLFormat.BackgroundRepeatTypes val)
    {
      switch ((int) val)
      {
        case 0:
          return "repeat";
        case 1:
          return "no-repeat";
        case 2:
          return "repeat-x";
        case 3:
          return "repeat-y";
        default:
          return (string) null;
      }
    }

    public class FontStyles
    {
      public const string Normal = "normal";
      public const string Italic = "italic";
    }

    public class FontWeights
    {
      public const string Normal = "400";
      public const string Thin = "100";
      public const string ExtraLight = "200";
      public const string Light = "300";
      public const string Medium = "500";
      public const string SemiBold = "600";
      public const string Bold = "700";
      public const string ExtraBold = "800";
      public const string Heavy = "900";
    }

    public class TextDecorations
    {
      public const string None = "none";
      public const string Underline = "underline";
      public const string Overline = "overline";
      public const string LineThrough = "line-through";
    }

    public class TextAlignments
    {
      public const string General = "General";
      public const string Left = "left";
      public const string Center = "center";
      public const string Right = "right";
    }

    public class VerticalAlignments
    {
      public const string Top = "top";
      public const string Middle = "middle";
      public const string Bottom = "bottom";
    }

    public class Directions
    {
      public const string LTR = "ltr";
      public const string RTL = "rtl";
    }

    public class WritingModes
    {
      public const string Horizontal = "lr-tb";
      public const string Vertical = "tb-rl";
    }

    public class UnicodeBiDiTypes
    {
      public const string Normal = "normal";
      public const string Embed = "embed";
      public const string BiDiOverride = "bidi-override";
    }

    public class BorderStyles
    {
      public const string None = "none";
      public const string Dotted = "dotted";
      public const string Dashed = "dashed";
      public const string Solid = "solid";
      public const string Double = "double";
    }

    public class BackgroundRepeatTypes
    {
      public const string Repeat = "repeat";
      public const string NoRepeat = "no-repeat";
      public const string RepeatX = "repeat-x";
      public const string RepeatY = "repeat-y";
    }
  }
}
