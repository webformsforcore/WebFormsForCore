// ----------------------------------------------------------------------------------------------------
// <copyright file="JsonExtensions.cs" company="Microsoft Corporation">
//   Copyright Microsoft Corporation, all rights reserved.
// </copyright>
// ----------------------------------------------------------------------------------------------------
namespace WebGrease.Extensions
{
    using System;
    using System.Reflection;

    using Newtonsoft.Json;
    using Newtonsoft.Json.Serialization;

    /// <summary>Json extensions.</summary>
    internal static class JsonExtensions
    {
        #region Static Fields

        /// <summary>The default json serializer settings.</summary>
        private static readonly JsonSerializerSettings DefaultJsonSerializerSettings = new JsonSerializerSettings();

        /// <summary>The json serializer settings.</summary>
        private static readonly Lazy<JsonSerializerSettings> JsonSerializerSettings = new Lazy<JsonSerializerSettings>(
            () =>
                {
                    var contractResolver = new DefaultContractResolver();
                    contractResolver.DefaultMembersSearchFlags |= BindingFlags.NonPublic;
                    return new JsonSerializerSettings { ContractResolver = contractResolver };
                });

        #endregion

        #region Methods

        /// <summary>Creates/Loads an Object of type T from a JSON string.</summary>
        /// <param name="json">The json.</param>
        /// <param name="nonPublic">If it should also set non-public (internal/private) properties.</param>
        /// <typeparam name="T">The type of object to create</typeparam>
        /// <returns>The created object.</returns>
        internal static T FromJson<T>(this string json, bool nonPublic = false)
        {
            return JsonConvert.DeserializeObject<T>(json, GetJsonSerializationSettings(nonPublic));
        }

        /// <summary>Converts an object to a json string representation.</summary>
        /// <param name="value">The object.</param>
        /// <param name="nonPublic">If it should also set non-public (internal/private) properties.</param>
        /// <returns>The json string.</returns>
        internal static string ToJson(this object value, bool nonPublic = false)
        {
            return JsonConvert.SerializeObject(value, Formatting.None, GetJsonSerializationSettings(nonPublic));
        }

        /// <summary>Gets the json serialization settings.</summary>
        /// <param name="nonPublic">If it should also set non-public (internal/private) properties.</param>
        /// <returns>The serialization settings.</returns>
        private static JsonSerializerSettings GetJsonSerializationSettings(bool nonPublic)
        {
            return nonPublic ? JsonSerializerSettings.Value : DefaultJsonSerializerSettings;
        }

        #endregion
    }
}