// <copyright file="TangentPointPosition.cs" company="Ethar">
// Copyright (c) Ethar. All rights reserved.
// </copyright>

namespace Ethar.GeoPose.DataTypes
{
    using System;
    using Newtonsoft.Json;

    /// <summary>
    /// A construct that represents a location represented by latitude and longitude in decimal degrees and a height in meters.
    ///
    /// Requirements derived from figure 13 in section 7.2.4 of version 1.0.0 of the GeoPose spec http://www.opengis.net/doc/DIS/geopose/1.0.
    /// </summary>
    public struct TangentPointPosition : IEquatable<TangentPointPosition>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TangentPointPosition"/> struct.
        /// </summary>
        /// <param name="latitude">The latitude in decimal degrees. Valid range is from -90 to 90.</param>
        /// <param name="longitude">The longitude in decimal degrees. Valid range is from -180 to 180.</param>
        /// <param name="heightInMeters">The height in meters.</param>
        public TangentPointPosition(float latitude, float longitude, float heightInMeters)
        {
            this.Latitude = latitude;
            this.Longitude = longitude;
            this.HeightInMeters = heightInMeters;
        }

        /// <summary>
        /// Gets or sets the latitude in decimal degrees.
        /// </summary>
        [JsonProperty("lat")]
        public float Latitude { get; set; }

        /// <summary>
        /// Gets or sets the longitude in decimal degrees.
        /// </summary>
        [JsonProperty("lon")]
        public float Longitude { get; set; }

        /// <summary>
        /// Gets or sets the height in meters.
        /// </summary>
        [JsonProperty("h")]
        public float HeightInMeters { get; set; }

        /// <summary>
        /// Performs an equality comparison on two objects of type <see cref="TangentPointPosition"/>.
        /// </summary>
        /// <param name="a">The first item.</param>
        /// <param name="b">The second item.</param>
        /// <returns>Whether the two items are equal.</returns>
        public static bool operator ==(TangentPointPosition a, TangentPointPosition b) => a.Equals(b);

        /// <summary>
        /// Performs an equality comparison on two objects of type <see cref="TangentPointPosition"/>.
        /// </summary>
        /// <param name="a">The first item.</param>
        /// <param name="b">The second item.</param>
        /// <returns>Whether the two items are not equal.</returns>
        public static bool operator !=(TangentPointPosition a, TangentPointPosition b) => !(a == b);

        /// <inheritdoc/>
        public bool Equals(TangentPointPosition other)
        {
            return this.Latitude.Equals(other.Latitude)
                && this.Longitude.Equals(other.Longitude)
                && this.HeightInMeters.Equals(other.HeightInMeters);
        }

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            return obj is TangentPointPosition equatable && this.Equals(equatable);
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            unchecked
            {
                var hashcode = this.Longitude.GetHashCode();
                hashcode = (hashcode * 397) ^ this.Latitude.GetHashCode();
                hashcode = (hashcode * 397) ^ this.HeightInMeters.GetHashCode();
                return hashcode;
            }
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return $"Latitude:{this.Latitude}, Longitude:{this.Longitude}, HeightInMeters:{this.HeightInMeters}";
        }
    }
}