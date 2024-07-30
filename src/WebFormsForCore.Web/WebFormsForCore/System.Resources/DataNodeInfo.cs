#if !NETFRAMEWORK

using System.Drawing;

#nullable disable
namespace System.Resources
{
  internal class DataNodeInfo
  {
    internal string Name;
    internal string Comment;
    internal string TypeName;
    internal string MimeType;
    internal string ValueData;
    internal Point ReaderPosition;

    internal DataNodeInfo Clone()
    {
      return new DataNodeInfo()
      {
        Name = this.Name,
        Comment = this.Comment,
        TypeName = this.TypeName,
        MimeType = this.MimeType,
        ValueData = this.ValueData,
        ReaderPosition = new Point(this.ReaderPosition.X, this.ReaderPosition.Y)
      };
    }
  }
}
#endif