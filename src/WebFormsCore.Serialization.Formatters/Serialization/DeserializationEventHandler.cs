// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters;

namespace EstrellasDeEsperanza.WebFormsCore.Serialization.Formatters.Binary
{
    internal delegate void DeserializationEventHandler(object? sender);

    public delegate void SerializationEventHandler(StreamingContext context);
}
