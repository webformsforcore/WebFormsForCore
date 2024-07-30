// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PrinterFormatter.cs" company="Microsoft">
//   Copyright Microsoft Corporation, all rights reserved
// </copyright>
// <summary>
//   Print formatter for indentation
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace WebGrease.Css
{
    using System.Text;

    /// <summary>Print formatter for indentation</summary>
    internal sealed class PrinterFormatter
    {
        /// <summary>
        /// The buffer to write the output to.
        /// </summary>
        private readonly StringBuilder _buffer = new StringBuilder(1024);

        /// <summary>
        /// The indent scale on multiple of 0, 1, 2 etc.
        /// </summary>
        private int _indentLevel;

        /// <summary>
        /// Gets or sets a value indicating whether pretty print is true/false
        /// </summary>
        /// <value>The pretty print</value>
        public bool PrettyPrint { get; set; }

        /// <summary>
        /// Gets or sets the indent character
        /// </summary>
        public char IndentCharacter { get; set; }

        /// <summary>
        /// Gets or sets the indent size
        /// </summary>
        public int IndentSize { get; set; }

        /// <summary>The string representation of Printer Formatter</summary>
        /// <returns>The string representation</returns>
        public override string ToString()
        {
            return _buffer.ToString();
        }

        /// <summary>Appends a string</summary>
        /// <param name="content">The content to append</param>
        public void Append(string content)
        {
            _buffer.Append(content);
        }

        /// <summary>Appends a character</summary>
        /// <param name="content">The content to append</param>
        public void Append(char content)
        {
            _buffer.Append(content);
        }

        /// <summary>Appends a character</summary>
        /// <param name="content">The content to append</param>
        public void AppendLine(char content)
        {
            if (this.PrettyPrint)
            {
                _buffer.AppendLine(content.ToString());
            }
            else
            {
                _buffer.Append(content);
            }
        }

        /// <summary>Appends a line</summary>
        public void AppendLine()
        {
            if (this.PrettyPrint)
            {
                _buffer.AppendLine();
            }
        }

        /// <summary>Removes the content from buffer</summary>
        /// <param name="startIndex">The start index</param>
        /// <param name="length">The length to remove</param>
        public void Remove(int startIndex, int length)
        {
            _buffer.Remove(startIndex, length);
        }

        /// <summary>Buffer length</summary>
        /// <returns>The length of buffer</returns>
        public int Length()
        {
            return _buffer.Length;
        }

        /// <summary>Increments the indent scale</summary>
        public void IncrementIndentLevel()
        {
            _indentLevel++;
        }

        /// <summary>Decrements the indent scale</summary>
        public void DecrementIndentLevel()
        {
            if (_indentLevel > 0)
            {
                _indentLevel--;
            }
        }

        /// <summary>Writes the indent to buffer</summary>
        public void WriteIndent()
        {
            if (!this.PrettyPrint)
            {
                return;
            }

            // Add the indent to buffer now
            var indent = new string(this.IndentCharacter, _indentLevel * this.IndentSize);
            _buffer.Append(indent);
        }
    }
}
