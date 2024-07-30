// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RenamedFilesLog.cs" company="Microsoft">
//   Copyright Microsoft Corporation, all rights reserved
// </copyright>
// <summary>
//   Represents a strong typed hash files log which contains a collection
//   of hashed files
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace WebGrease.Activities
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.IO;
    using System.Linq;
    using System.Xml.Linq;

    /// <summary>Represents a strong typed hash files log which contains a collection
    /// of hashed files</summary>
    internal sealed class RenamedFilesLog
    {
        /// <summary>Initializes a new instance of the <see cref="RenamedFilesLog"/> class.</summary>
        /// <param name="logFile">The log file.</param>
        internal RenamedFilesLog(string logFile)
        {
            this.RenamedFiles = new List<RenamedFile>();

            //// Expected Xml format:
            //// ====================
            ////  <RenamedFiles>
            ////      <File>
            ////          <Output>/sc/LinkListTestProduct/generic-generic/css/E7F59684F312DFE016D8312AD7AA4C74.css</Output> 
            ////          <Input>/generic-generic/css/layout_02_bluetheme.css</Input> 
            ////          </File>
            ////      <File>
            ////          <Output>/sc/LinkListTestProduct/generic-generic/css/59620BC8C211A82729559B795605CBCB.css</Output> 
            ////          <Input>/generic-generic/css/layout_02_slotlib_portalpackage.css</Input> 
            ////          <Input>/generic-generic/css/layout_02_slotlib_portalpackage_SlotLib_Page.css</Input> 
            ////      </File>
            ////      <File>
            ////          <Output>/sc/LinkListTestProduct/generic-generic/css/66A5465F6176306FA0E747794763E3D7.css</Output> 
            ////          <Input>/generic-generic/css/layout_02_slotlib_portalpackage_ie.css</Input> 
            ////          <Input>/generic-generic/css/layout_02_slotlib_portalpackage_SlotLib_Page_ie.css</Input> 
            ////      </File>
            ////  </RenamedFiles>
            if (string.IsNullOrWhiteSpace(logFile) ||
                !File.Exists(logFile))
            {
                return;
            }

            var xdocument = XDocument.Load(logFile);
            var renamedFilesElement = xdocument.Element("RenamedFiles");
            if (renamedFilesElement == null)
            {
                return;
            }
            
            foreach (var fileElement in renamedFilesElement.Elements("File"))
            {
                var file = new RenamedFile(fileElement);
                this.RenamedFiles.Add(file);
            }
        }

        /// <summary>
        /// Gets the list of renamed files
        /// </summary>
        /// <value>The list of renamed files</value>
        internal List<RenamedFile> RenamedFiles { get; private set; }
    }
}
