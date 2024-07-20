// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#if !Legacy
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters;

namespace EstrellasDeEsperanza.WebFormsCore.Serialization.Formatters.Binary
{
    public sealed partial class BinaryFormatter : IFormatter
    {
        [Obsolete(Obsoletions.BinaryFormatterMessage, DiagnosticId = Obsoletions.BinaryFormatterDiagId, UrlFormat = Obsoletions.SharedUrlFormat)]
        [RequiresUnreferencedCode(IFormatter.RequiresUnreferencedCodeMessage)]
        public object Deserialize(Stream serializationStream)
            => throw new PlatformNotSupportedException(SR.BinaryFormatter_SerializationNotSupportedOnThisPlatform);

        [Obsolete(Obsoletions.BinaryFormatterMessage, DiagnosticId = Obsoletions.BinaryFormatterDiagId, UrlFormat = Obsoletions.SharedUrlFormat)]
        [RequiresUnreferencedCode(IFormatter.RequiresUnreferencedCodeMessage)]
        public void Serialize(Stream serializationStream, object graph)
            => throw new PlatformNotSupportedException(SR.BinaryFormatter_SerializationNotSupportedOnThisPlatform);
    }
}
#endif