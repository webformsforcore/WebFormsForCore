//------------------------------------------------------------------------------
// <copyright file="VersionUtil.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>                                                                
//------------------------------------------------------------------------------

/*
 * Helper class for performing common operations on Version objects
 * 
 * Copyright (c) 2009 Microsoft Corporation
 */

namespace System.Web.Util {
    using System;

    internal static class VersionUtil {

        public static readonly Version Framework00 = new Version(0, 0);
        public static readonly Version Framework20 = new Version(2, 0);
        public static readonly Version Framework35 = new Version(3, 5);
        public static readonly Version Framework40 = new Version(4, 0);
        public static readonly Version Framework45 = new Version(4, 5);
        public static readonly Version Framework451 = new Version(4, 5, 1);
        public static readonly Version Framework452 = new Version(4, 5, 2);
        public static readonly Version Framework46 = new Version(4, 6);
        public static readonly Version Framework461 = new Version(4, 6, 1);
        public static readonly Version Framework463 = new Version(4, 6, 3);
        public static readonly Version Framework472 = new Version(4, 7, 2);
		public static readonly Version Framework48 = new Version(4, 8);
		public static readonly Version Net5 = new Version(5, 0);
		public static readonly Version Net6 = new Version(6, 0);
		public static readonly Version Net7 = new Version(7, 0);
		public static readonly Version Net8 = new Version(8, 0);
		public static readonly Version Net9 = new Version(9, 0);
        public static readonly Version Net10 = new Version(10, 0);

        // Convenience accessor for the "default" framework version; various configuration
        // switches can use this as a default value. This value must only be bumped during
        // SxS releases of the .NET Framework.
        public static readonly Version FrameworkDefault = Framework40;
        public const string FrameworkDefaultString = "4.0";

    }
}
