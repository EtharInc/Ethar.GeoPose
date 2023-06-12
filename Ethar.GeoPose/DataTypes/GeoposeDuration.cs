// <copyright file="GeoposeDuration.cs" company="Ethar">
// Copyright (c) Ethar. All rights reserved.
// </copyright>

namespace Ethar.GeoPose.DataTypes
{
    using System;
    using Ethar.GeoPose.JsonConversion;
    using Newtonsoft.Json;

    /// <summary>
    /// A construct that represents a duration of time in milliseconds.
    ///
    /// Requirements derived from Requirement 11 in section 8.3.3 of version 1.0.0 of the GeoPose spec http://www.opengis.net/doc/DIS/geopose/1.0.
    /// </summary>
    [JsonConverter(typeof(GeoposeDurationJsonConverter))]
    public struct GeoPoseDuration : IEquatable<GeoPoseDuration>
    {
        /// <summary>
        /// Gets or sets the duration in milliseconds.
        /// </summary>
        [JsonProperty("numericDuration")]
        public long NumericDuration { get; set; }

        /// <summary>
        /// Performs an equality comparison on two objects of type <see cref="GeoPoseDuration"/>.
        /// </summary>
        /// <param name="a">The first item.</param>
        /// <param name="b">The second item.</param>
        /// <returns>Whether the two items are equal.</returns>
        public static bool operator ==(GeoPoseDuration a, GeoPoseDuration b) => a.Equals(b);

        /// <summary>
        /// Performs an equality comparison on two objects of type <see cref="GeoPoseDuration"/>.
        /// </summary>
        /// <param name="a">The first item.</param>
        /// <param name="b">The second item.</param>
        /// <returns>Whether the two items are not equal.</returns>
        public static bool operator !=(GeoPoseDuration a, GeoPoseDuration b) => !(a == b);

        /// <inheritdoc/>
        public bool Equals(GeoPoseDuration other)
        {
            return this.NumericDuration.Equals(other.NumericDuration);
        }

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            return obj is GeoPoseDuration equatable && this.Equals(equatable);
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            unchecked
            {
                var hashcode = this.NumericDuration.GetHashCode();
                return hashcode;
            }
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return $"NumericDuration:{this.NumericDuration}";
        }
    }
}