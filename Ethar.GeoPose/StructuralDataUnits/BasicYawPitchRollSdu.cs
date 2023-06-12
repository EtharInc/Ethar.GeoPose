// <copyright file="BasicYawPitchRollSdu.cs" company="Ethar">
// Copyright (c) Ethar. All rights reserved.
// </copyright>

namespace Ethar.GeoPose.StructuralDataUnits
{
    using System;
    using Ethar.GeoPose.DataTypes;
    using Ethar.GeoPose.Interfaces;
    using Newtonsoft.Json;

    /// <summary>
    /// A construct that represents a Basic YPR SDU.
    /// The Basic-YPR Target has a simple structure with no options. Position is specified as a point in an LTP-ENU frame and rotation is specified by yaw, pitch, and roll angles specified in decimal degrees.
    ///
    /// Requirements derived from section 8.4.1 of version 1.0.0 of the GeoPose spec http://www.opengis.net/doc/DIS/geopose/1.0.
    /// </summary>
    public struct BasicYawPitchRollSdu : IStructuralDataUnit, IEquatable<BasicYawPitchRollSdu>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BasicYawPitchRollSdu"/> struct.
        /// </summary>
        /// <param name="angles">A representation of the orientation in yaw, pitch, and roll.</param>
        /// <param name="position"> The position in latitude and longitude in decimal degrees, and height in meters.</param>
        public BasicYawPitchRollSdu(YawPitchRollAngles angles, TangentPointPosition position)
        {
            this.Angles = angles;
            this.Position = position;
        }

        /// <summary>
        ///  Gets the position in latitude and longitude in decimal degrees, and height in meters.
        /// </summary>
        [JsonProperty("position")]
        public TangentPointPosition Position { get; }

        /// <summary>
        /// Gets the orientation represented in yaw, pitch, and roll in decimal degrees.
        /// </summary>
        [JsonProperty("angles")]
        public YawPitchRollAngles Angles { get; }

        /// <summary>
        /// Performs an equality comparison on two objects of type <see cref="BasicYawPitchRollSdu"/>.
        /// </summary>
        /// <param name="a">The first item.</param>
        /// <param name="b">The second item.</param>
        /// <returns>Whether the two items are equal.</returns>
        public static bool operator ==(BasicYawPitchRollSdu a, BasicYawPitchRollSdu b) => a.Equals(b);

        /// <summary>
        /// Performs an equality comparison on two objects of type <see cref="BasicYawPitchRollSdu"/>.
        /// </summary>
        /// <param name="a">The first item.</param>
        /// <param name="b">The second item.</param>
        /// <returns>Whether the two items are not equal.</returns>
        public static bool operator !=(BasicYawPitchRollSdu a, BasicYawPitchRollSdu b) => !(a == b);

        /// <inheritdoc/>
        public bool Equals(BasicYawPitchRollSdu other)
        {
            return this.Position.Equals(other.Position)
                && this.Angles.Equals(other.Angles);
        }

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            return obj is BasicYawPitchRollSdu equatable && this.Equals(equatable);
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            unchecked
            {
                var hashcode = this.Position.GetHashCode();
                hashcode = (hashcode * 397) ^ this.Angles.GetHashCode();
                return hashcode;
            }
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return $"Position:[{this.Position}], Angles:[{this.Angles}]";
        }
    }
}