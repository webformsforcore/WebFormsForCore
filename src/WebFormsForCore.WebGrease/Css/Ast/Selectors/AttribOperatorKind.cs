// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AttribOperatorKind.cs" company="Microsoft">
//   Copyright Microsoft Corporation, all rights reserved
// </copyright>
// <summary>
//   Attribute Operators
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace WebGrease.Css.Ast.Selectors
{
    /// <summary>Attribute Operators</summary>
    public enum AttribOperatorKind
    {
        /// <summary>
        /// Prefix '^=' value
        /// </summary>
        Prefix, 

        /// <summary>
        /// Equal '$=' value
        /// </summary>
        Suffix, 

        /// <summary>
        /// SubString '*=' value
        /// </summary>
        Substring, 

        /// <summary>
        /// Equal '=' value
        /// </summary>
        Equal, 

        /// <summary>
        /// Includes '~=' value
        /// </summary>
        Includes, 

        /// <summary>
        /// Dash Match '|=' value
        /// </summary>
        DashMatch, 

        /// <summary>
        /// None "" value
        /// </summary>
        None
    }
}
