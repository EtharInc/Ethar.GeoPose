// <copyright file="TransitionModelJsonConverter.cs" company="Ethar">
// Copyright (c) Ethar. All rights reserved.
// </copyright>

namespace Ethar.GeoPose.JsonConversion
{
    using System;
    using Ethar.GeoPose.Authority;
    using Ethar.GeoPose.TransitionModels;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    /// <summary>
    /// A utility class that converts transition models to json and vice versa.
    /// </summary>
    public class TransitionModelJsonConverter : JsonConverter<TransitionModel>
    {
        /// <inheritdoc/>
        public override TransitionModel ReadJson(JsonReader reader, Type objectType, TransitionModel existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            var jsonObject = JObject.Load(reader);
            var authorityName = (string)jsonObject["authority"] ?? string.Empty;

            if (string.IsNullOrEmpty(authorityName) || jsonObject == null)
            {
                // TODO: create exception type for this
                throw new Exception();
            }

            var authority = AuthorityProvider.GetAuthority(authorityName);
            return authority.ConvertJsonToTransitionModel(jsonObject);
        }

        /// <inheritdoc/>
        public override void WriteJson(JsonWriter writer, TransitionModel value, JsonSerializer serializer)
        {
            var authorityName = value.Authority ?? throw new NullReferenceException(nameof(value));
            var authority = AuthorityProvider.GetAuthority(authorityName);
            var jObj = authority.ConvertTransitionModelToJson(value);
            jObj.WriteTo(writer);
        }
    }
}
