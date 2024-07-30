// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Combinator.cs" company="Microsoft">
//   Copyright Microsoft Corporation, all rights reserved
// </copyright>
// <summary>
//   Combinator characters
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace WebGrease.Css.Ast
{
    /// <summary>Combinator characters</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Combinator", Justification="Chosen name.")]
    public enum Combinator
    {
        /// <summary>
        /// Plus Sign (+) or null 
        /// </summary>
        PlusSign, 

        /// <summary>
        /// Greater than sign (>) or null
        /// </summary>
        GreaterThanSign, 

        /// <summary>
        /// Tilde Sign (~) or null
        /// </summary>
        Tilde, 

        /// <summary>
        /// Zero Space (S) value
        /// </summary>
        ZeroSpace, 

        /// <summary>
        /// Space (S) value
        /// </summary>
        SingleSpace, 

        /// <summary>
        /// None type value
        /// </summary>
        None
    }
}
