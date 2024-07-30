// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UnicodeDecoder.cs" company="Microsoft">
//   Copyright Microsoft Corporation, all rights reserved
// </copyright>
// <summary>
//   The The decoder for unicode characters.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace WebGrease.Css
{
    using System.Diagnostics.Contracts;
    using System.IO;
    using System.Text;
    using Ast;

    /// <summary>The decoder for unicode characters.</summary>
    public class UnicodeDecoder
    {
        /// <summary>The text reader in question.</summary>
        private readonly TextReader _reader;

        /// <summary>The current character.</summary>
        private char _currentChar;

        /// <summary>The read ahead buffer.</summary>
        private string _readAhead;

        /// <summary>Initializes a new instance of the <see cref="UnicodeDecoder"/> class.</summary>
        /// <param name="reader">The text reader in question.</param>
        private UnicodeDecoder(TextReader reader)
        {
            Contract.Requires(reader != null);
            _reader = reader;
        }

        /// <summary>The unicode.</summary>
        /// <param name="text">The text.</param>
        /// <returns>The unicode encoded string.</returns>
        public static string Decode(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return text;
            }

            using (var reader = new StringReader(text))
            {
                var decoder = new UnicodeDecoder(reader);
                return decoder.GetUnicode();
            }
        }

        /// <summary>Returns the corresponding hex value.</summary>
        /// <param name="ch">The character for which hex value is required.</param>
        /// <returns>The hex value.</returns>
        private static int HValue(char ch)
        {
            var hexValue = 0;
            if ('0' <= ch && ch <= '9')
            {
                hexValue = ch - '0';
            }
            else if ('a' <= ch && ch <= 'f')
            {
                hexValue = (ch - 'a') + 10;
            }
            else if ('A' <= ch && ch <= 'F')
            {
                hexValue = (ch - 'A') + 10;
            }

            return hexValue;
        }

        /// <summary>Determines if char is a hex value.</summary>
        /// <param name="ch">The character to test.</param>
        /// <returns>Whether the char is hex.</returns>
        private static bool IsH(char ch)
        {
            return ('0' <= ch && ch <= '9')
              || ('a' <= ch && ch <= 'f')
              || ('A' <= ch && ch <= 'F');
        }

        /// <summary>Determines if the character is a space.</summary>
        /// <param name="ch">The character.</param>
        /// <returns>If the character is space.</returns>
        private static bool IsSpace(char ch)
        {
            switch (ch)
            {
                case ' ':
                case '\t':
                case '\r':
                case '\n':
                case '\f':
                    return true;

                default:
                    return false;
            }
        }

        /// <summary>Gets the unicode decoded string.</summary>
        /// <returns>The unicode decoded string.</returns>
        private string GetUnicode()
        {
            this.NextChar();
            var stringBuilder = new StringBuilder();
            while (_currentChar != '\0')
            {
                if (_currentChar == '\\' && IsH(this.PeekChar()))
                {
                    // Decode the hexadecimal digits at the current character point,
                    // up to six characters
                    var unicodeValue = this.GetUnicodeEncodingValue();

                    // The Unicode Standard defines a surrogate pair as a coded character representation
                    // for a single abstract character that consists of a sequence of two code units. The
                    // first value of the surrogate pair is the high surrogate, a 16-bit code value in the
                    // range of U+D800 through U+DBFF. The second value of the pair is the low surrogate, in
                    // the range of U+DC00 through U+DFFF. The Unicode Standard defines a combining character
                    // sequence as a combination of a base character and one or more combining characters.
                    // A surrogate pair can represent a base character or a combining character. 
                    // For more information on surrogate pairs and combining character sequences, see The 
                    // Unicode Standard at the Unicode home page. The key point to remember is that surrogate
                    // pairs represent 32-bit single characters. You cannot assume that one 16-bit Unicode encoding
                    // value maps to exactly one character. By using surrogate pairs, a 16-bit Unicode encoded system
                    // can address an additional one million code points to which characters will be assigned
                    // by the Unicode Standard.
                    if (unicodeValue >= 0xd800 && unicodeValue <= 0xdbff)
                    {
                        // Safely read the next char since it should be unicode as well.
                        this.NextChar();

                        // This is a high-surrogate value.
                        var hi = unicodeValue;

                        // The next encoding better be a unicode value
                        if (_currentChar == '\\' && IsH(this.PeekChar()))
                        {
                            // Get the low value
                            var lo = this.GetUnicodeEncodingValue();
                            if (lo >= 0xdc00 && lo <= 0xdfff)
                            {
                                // combine the hi/lo pair into one character value
                                unicodeValue = 0x10000
                                  + ((hi - 0xd800) * 0x400)
                                  + (lo - 0xdc00);
                            }
                            else
                            {
                                // TODO - Add context here.
                                throw new AstException("Invalid low surrogate.");
                            }
                        }
                        else
                        {
                            // TODO - Add context here.
                            throw new AstException("High surrogate should be followed by the low surrogate.");
                        }
                    }

                    // get the unicode character.
                    stringBuilder.Append(char.ConvertFromUtf32(unicodeValue));
                }
                else
                {
                    stringBuilder.Append(_currentChar);
                }

                this.NextChar();
            }

            return stringBuilder.ToString();
        }

        /// <summary>Returns the value of a unicode number, up to six hex digits</summary>
        /// <returns>Int value representing up to 6 hex digits</returns>
        private int GetUnicodeEncodingValue()
        {
            var unicodeValue = 0;

            // Loop for no more than 6 hex characters. The idea here is to do the following conversion every iteration:
            // Say unicode in question is "\abcdef"
            // let u = 0
            // u = u*16 + 10 = 10
            // u = 10*16 + 11
            // u = (10*16 + 11)*16 + 12 = 10*16^2+11*16+12
            // ...
            // ...
            // So \0026 (decimal 0 + 0 + 2 * 16^1 + 6 * 16^0 = 38)
            var count = 0;
            while (count++ < 6 && IsH(this.PeekChar()))
            {
                this.NextChar();
                unicodeValue = (unicodeValue * 16) + HValue(_currentChar);
            }

            // if there is a space character afterwards, skip it
            // (but only skip one space character if present)
            if (IsSpace(this.PeekChar()))
            {
                this.NextChar();
            }

            return unicodeValue;
        }

        /// <summary>Reads the next character from the stream.</summary>
        private void NextChar()
        {
            if (_readAhead != null)
            {
                _currentChar = _readAhead[0];
                _readAhead = _readAhead.Length == 1 ? null : _readAhead.Substring(1);
            }
            else
            {
                var ch = _reader.Read();
                if (ch < 0)
                {
                    _currentChar = '\0';
                }
                else
                {
                    _currentChar = (char)ch;
                }
            }
        }

        /// <summary>Peeks the next character from the stream.</summary>
        /// <returns>The peeked character.</returns>
        private char PeekChar()
        {
            if (_readAhead != null)
            {
                return _readAhead[0];
            }
            
            var ch = _reader.Peek();
            if (ch < 0)
            {
                return '\0';
            }
            
            return (char)ch;
        }
    }
}
