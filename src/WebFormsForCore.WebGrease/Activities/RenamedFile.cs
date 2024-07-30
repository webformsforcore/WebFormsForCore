// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RenamedFile.cs" company="Microsoft">
//   Copyright Microsoft Corporation, all rights reserved
// </copyright>
// <summary>
//   Represents a strong typed node in renamed file log
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace WebGrease.Activities
{
    using System;
    using System.Collections.Generic;
    using System.Xml.Linq;

    /// <summary>Represents a strong typed node in renamed file log</summary>
    internal sealed class RenamedFile
    {
        /// <summary>Initializes a new instance of the <see cref="RenamedFile"/> class.</summary>
        /// <param name="fileElement">The file element.</param>
        public RenamedFile(XContainer fileElement)
        {
            //// Expected Xml for File Element:
            //// ==============================
            //// <File>
            ////    <Output>/sc/LinkListTestProduct/generic-generic/css/66A5465F6176306FA0E747794763E3D7.css</Output> 
            ////    <Input>/generic-generic/css/layout_02_slotlib_portalpackage_ie.css</Input> 
            ////    <Input>/generic-generic/css/layout_02_slotlib_portalpackage_SlotLib_Page_ie.css</Input> 
            //// </File>
            ////
            if (fileElement == null)
            {
                throw new ArgumentNullException("fileElement", "The fileElement cannot be null.");
            }

            this.InputNames = new List<string>();

            // Populate output name
            var outputElement = fileElement.Element("Output");
            if (outputElement != null)
            {
                this.OutputName = outputElement.Value;
            }

            // Populate input name
            foreach (var inputElement in fileElement.Elements("Input"))
            {
                this.InputNames.Add(inputElement.Value);
            }
        }

        /// <summary>
        /// Gets Output hashed path
        /// </summary>
        /// <value>The name of the output.</value>
        public string OutputName { get; private set; }

        /// <summary>
        /// Gets the input names.
        /// </summary>
        /// <value>The input names.</value>
        public List<string> InputNames { get; private set; }
    }
}
