// ----------------------------------------------------------------------------------------------------
// <copyright file="StringExtensions.cs" company="Microsoft Corporation">
//   Copyright Microsoft Corporation, all rights reserved.
// </copyright>
// ----------------------------------------------------------------------------------------------------
namespace WebGrease.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;

    /// <summary>The string extensions.</summary>
    internal static class StringExtensions
    {
        #region Public Methods and Operators

        /// <summary>Return null if the string is empty or whitespace or null otherwise returns the string.</summary>
        /// <param name="value">The string</param>
        /// <returns>Null or the string of not empty or whitespace</returns>
        public static string AsNullIfWhiteSpace(this string value)
        {
            return string.IsNullOrWhiteSpace(value) ? null : value;
        }

        /// <summary>Formats the string with the InvariantCulture.</summary>
        /// <param name="format">The format</param>
        /// <param name="args">The format parameters.</param>
        /// <returns>The formatting string.</returns>
        public static string InvariantFormat(this string format, params object[] args)
        {
            if (format == null)
            {
                throw new ArgumentNullException("format");
            }

            return string.Format(CultureInfo.InvariantCulture, format, args);
        }

        #endregion

        #region Methods


        /// <summary>Tries to parse the string into the enum.
        /// This implementation does not know how to deal with flags.</summary>
        /// <param name="value">The string to parse. </param>
        /// <param name="defaultValue">The defaultValue (null if not set)</param>
        /// <typeparam name="TEnum">The type of the enum </typeparam>
        /// <returns>The parsed enum or the defaultValue for the enum if unable to parse </returns>
        public static TEnum? TryParseToEnum<TEnum>(this string value, TEnum? defaultValue = null) where TEnum : struct
        {
            TEnum result;
            return Enum.TryParse(value, true, out result) && Enum.IsDefined(typeof(TEnum), result)
                ? result
                : defaultValue;
        }

        /// <summary>Checks if the string is null or empty space.</summary>
        /// <param name="text">string to test</param>
        /// <returns>true or false</returns>
        internal static bool IsNullOrWhitespace(this string text)
        {
            return string.IsNullOrWhiteSpace(text);
        }

        /// <summary>The try parse for string to boolean.</summary>
        /// <param name="textToParse">The text to parse.</param>
        /// <returns>The try parse.</returns>
        public static bool TryParseBool(this string textToParse)
        {
            bool result;
            return !bool.TryParse(textToParse, out result) || result;
        }

        /// <summary>parses text into a number (if valid)</summary>
        /// <param name="textToParse">text to parse</param>
        /// <returns>the number</returns>
        internal static int TryParseInt32(this string textToParse)
        {
            int temp;
            return int.TryParse(textToParse, out temp) ? temp : default(int);
        }

        /// <summary>parses text into a number (if valid)</summary>
        /// <param name="textToParse">text to parse</param>
        /// <returns>the number</returns>
        internal static float? TryParseFloat(this string textToParse)
        {
            float temp;
            return float.TryParse(textToParse, NumberStyles.Float, CultureInfo.InvariantCulture, out temp) ? (float?)temp : null;
        }

        /// <summary>The safe split semi colon seperated value.</summary>
        /// <param name="semicolonSeperatedValue">The semicolon seperated value.</param>
        /// <returns>The list of items.</returns>
        internal static IEnumerable<string> SafeSplitSemiColonSeperatedValue(this string semicolonSeperatedValue)
        {
            return string.IsNullOrWhiteSpace(semicolonSeperatedValue) 
                ? new string[] { } 
                : semicolonSeperatedValue.Split(Strings.SemicolonSeparator, StringSplitOptions.RemoveEmptyEntries).Select(t => t.Trim());
        }

        #endregion
    }
}