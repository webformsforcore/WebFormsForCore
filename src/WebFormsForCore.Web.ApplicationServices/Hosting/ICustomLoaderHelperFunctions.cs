#nullable disable
namespace System.Web.Hosting
{
  internal interface ICustomLoaderHelperFunctions
  {
    string AppPhysicalPath { get; }

    bool? CustomLoaderIsEnabled { get; }

    string GetTrustLevel(string appConfigMetabasePath);

    string MapPath(string relativePath);
  }
}
