// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CacheFileCategories.cs" company="Microsoft">
//   Copyright Microsoft Corporation, all rights reserved
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace WebGrease
{
    /// <summary>The cache keys.</summary>
    internal static class CacheFileCategories
    {
        #region Constants

        /// <summary>The assembler result.</summary>
        internal const string AssemblerResult = "AssemblerResult";

        /// <summary>The hashed image.</summary>
        internal const string HashedImage = "HashedImage";

        /// <summary>The hashed sprite image.</summary>
        internal const string HashedSpriteImage = "HashedSpriteImage";

        /// <summary>The hashed minified js result.</summary>
        internal const string HashedMinifiedJsResult = "HashedMinifiedJsResult";

        /// <summary>The hashed minified css result.</summary>
        internal const string HashedMinifiedCssResult = "HashedMinifiedCssResult";

        /// <summary>The minified css result.</summary>
        internal const string MinifiedCssResult = "MinifyCssResult";

        /// <summary>The relative file names.</summary>
        internal const string RelativeFileNames = "RelativeFileNames";

        /// <summary>The minify js result.</summary>
        internal const string MinifiedJsResult = "MinifiedJsResult";

        /// <summary>The preprocessing result.</summary>
        internal const string PreprocessingResult = "PreprocessingResult";

        /// <summary>The sprite log file.</summary>
        internal const string SpriteLogFile = "SpriteLogFile";

        /// <summary>The sprite log file xml.</summary>
        internal const string SpriteLogFileXml = "SpriteLogFileXml";

        /// <summary>The solution cache config.</summary>
        internal const string SolutionCacheConfig = "SolutionCacheConfig";

        #endregion
    }
}