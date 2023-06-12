// <copyright file="YawPitchRollAngles.cs" company="Ethar">
// Copyright (c) Ethar. All rights reserved.
// </copyright>

namespace Ethar.GeoPose.DataTypes
{
    using System;
    using Newtonsoft.Json;

    /// <summary>
    /// A construct that represents yaw, pitch, and roll angles specified in decimal degrees.
    ///
    /// Requirements derived from figure 8 in section 7.2.1 of version 1.0.0 of the GeoPose spec http://www.opengis.net/doc/DIS/geopose/1.0.
    /// </summary>
    public struct YawPitchRollAngles : IEquatable<YawPitchRollAngles>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="YawPitchRollAngles"/> struct.
        /// </summary>
        /// <param name="yaw">The yaw in decimal degrees.</param>
        /// <param name="pitch">The pitch in decimal degrees.</param>
        /// <param name="roll">The roll in decimal degrees.</param>
        public YawPitchRollAngles(float yaw, float pitch, float roll)
        {
            this.Yaw = yaw;
            this.Pitch = pitch;
            this.Roll = roll;
        }

        /// <summary>
        /// Gets or sets yaw specified in decimal degrees.
        /// </summary>
        [JsonProperty("yaw")]
        public float Yaw { get; set; }

        /// <summary>
        /// Gets or sets pitch specified in decimal degrees.
        /// </summary>
        [JsonProperty("pitch")]
        public float Pitch { get; set; }

        /// <summary>
        /// Gets or sets roll specified in decimal degrees.
        /// </summary>
        [JsonProperty("roll")]
        public float Roll { get; set; }

        /// <summary>
        /// Performs an equality comparison on two objects of type <see cref="YawPitchRollAngles"/>.
        /// </summary>
        /// <param name="a">The first item.</param>
        /// <param name="b">The second item.</param>
        /// <returns>Whether the two items are equal.</returns>
        public static bool operator ==(YawPitchRollAngles a, YawPitchRollAngles b) => a.Equals(b);

        /// <summary>
        /// Performs an equality comparison on two objects of type <see cref="YawPitchRollAngles"/>.
        /// </summary>
        /// <param name="a">The first item.</param>
        /// <param name="b">The second item.</param>
        /// <returns>Whether the two items are not equal.</returns>
        public static bool operator !=(YawPitchRollAngles a, YawPitchRollAngles b) => !(a == b);

        /// <inheritdoc/>
        public bool Equals(YawPitchRollAngles other)
        {
            return this.Yaw.Equals(other.Yaw)
                && this.Pitch.Equals(other.Pitch)
                && this.Roll.Equals(other.Roll);
        }

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            return obj is YawPitchRollAngles equatable && this.Equals(equatable);
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            unchecked
            {
                var hashcode = this.Yaw.GetHashCode();
                hashcode = (hashcode * 397) ^ this.Pitch.GetHashCode();
                hashcode = (hashcode * 397) ^ this.Roll.GetHashCode();
                return hashcode;
            }
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return $"Yaw:{this.Yaw}, Pitch:{this.Pitch}, Roll:{this.Roll}";
        }
    }
}
