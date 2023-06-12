// <copyright file="GeoposeDurationJsonConverter.cs" company="Ethar">
// Copyright (c) Ethar. All rights reserved.
// </copyright>

namespace Ethar.GeoPose.JsonConversion
{
    using System;
    using Ethar.GeoPose.DataTypes;
    using Newtonsoft.Json;

    /// <summary>
    /// A utility class that converts json into <see cref="GeoPoseDuration"/> and vice versa.
    /// </summary>
    public class GeoposeDurationJsonConverter : JsonConverter<GeoPoseDuration>
    {
        /// <inheritdoc/>
        public override GeoPoseDuration ReadJson(JsonReader reader, Type objectType, GeoPoseDuration existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            var time = (long)(reader.Value ?? throw new NullReferenceException(nameof(reader.Value)));

            return new GeoPoseDuration()
            {
                NumericDuration = time,
            };
        }

        /// <inheritdoc/>
        public override void WriteJson(JsonWriter writer, GeoPoseDuration value, JsonSerializer serializer)
        {
            var toWrite = value.NumericDuration;
            writer.WriteValue(toWrite);
        }
    }
}
