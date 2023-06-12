// <copyright file="FrameTransformIndexPairJsonConverter.cs" company="Ethar">
// Copyright (c) Ethar. All rights reserved.
// </copyright>

namespace Ethar.GeoPose.JsonConversion
{
    using System;
    using System.Linq;
    using Ethar.GeoPose.DataTypes;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    /// <summary>
    /// A utility class that converts json into a <see cref="FrameTransformIndexPair"/> and vice versa.
    /// </summary>
    public class FrameTransformIndexPairJsonConverter : JsonConverter<FrameTransformIndexPair>
    {
        /// <inheritdoc/>
        public override FrameTransformIndexPair ReadJson(JsonReader reader, Type objectType, FrameTransformIndexPair existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            var jsonObject = JObject.Load(reader);
            var link = (JArray)jsonObject["link"];

            var links = link.Select(x => (int)x);

            return new FrameTransformIndexPair(links);
        }

        /// <inheritdoc/>
        public override void WriteJson(JsonWriter writer, FrameTransformIndexPair value, JsonSerializer serializer)
        {
            var jObj = new JObject();

            var arr = new JArray
            {
                value.OuterFrameIndex,
                value.InnerFrameIndex,
            };

            jObj.Add("link", arr);

            jObj.WriteTo(writer);
        }
    }
}
