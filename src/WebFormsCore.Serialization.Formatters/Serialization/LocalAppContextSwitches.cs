// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Runtime.CompilerServices;

namespace EstrellasDeEsperanza.WebFormsCore.Serialization.Formatters.Binary
{

	internal static partial class LocalAppContextSwitches
	{
		private static int s_binaryFormatterEnabled;
		public static bool BinaryFormatterEnabled
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => true; // GetCachedSwitchValue("System.Runtime.Serialization.EnableUnsafeBinaryFormatterSerialization", ref s_binaryFormatterEnabled);
		}
		public static bool SerializationGuard
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => true;
		}

	}
}