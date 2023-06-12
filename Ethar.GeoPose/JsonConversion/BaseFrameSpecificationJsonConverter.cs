// <copyright file="BaseFrameSpecificationJsonConverter.cs" company="Ethar">
// Copyright (c) Ethar. All rights reserved.
// </copyright>

namespace Ethar.GeoPose.JsonConversion
{
    using System;
    using Ethar.GeoPose.Authority;
    using Ethar.GeoPose.Exceptions;
    using Ethar.GeoPose.FrameSpecifications;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    /// <summary>
    /// A utility class that converts json into <see cref="BaseFrameSpecification"/> and vice versa.
    /// </summary>
    public class BaseFrameSpecificationJsonConverter : JsonConverter<BaseFrameSpecification>
    {
        /// <inheritdoc/>
        /// <exception cref="AuthorityNotSupportedException">Thrown when the authority that the json belongs to is not supported.</exception>
        public override BaseFrameSpecification ReadJson(JsonReader reader, Type objectType, BaseFrameSpecification existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            var jsonObject = JObject.Load(reader);
            var authorityName = (string)jsonObject["authority"] ?? string.Empty;

            if (string.IsNullOrEmpty(authorityName) || jsonObject == null)
            {
                // TODO: create exception type for this
                throw new Exception();
            }

            var authority = AuthorityProvider.GetAuthority(authorityName);
            return authority.ConvertJsonToFrameSpec(jsonObject) as BaseFrameSpecification;
        }

        /// <inheritdoc/>
        public override void WriteJson(JsonWriter writer, BaseFrameSpecification value, JsonSerializer serializer)
        {
            var authorityName = value.Authority ?? throw new NullReferenceException(nameof(value));
            var authority = AuthorityProvider.GetAuthority(authorityName);
            var jObj = authority.ConvertFrameSpecToJson(value);
            jObj.WriteTo(writer);
        }
    }
}
