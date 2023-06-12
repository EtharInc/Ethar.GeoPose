// <copyright file="BasicQuaternionSdu.cs" company="Ethar">
// Copyright (c) Ethar. All rights reserved.
// </copyright>

namespace Ethar.GeoPose.StructuralDataUnits
{
    using System;
    using Ethar.GeoPose.DataTypes;
    using Ethar.GeoPose.Interfaces;
    using Newtonsoft.Json;

    /// <summary>
    /// A construct that represents a Basic Quaternion SDU.
    /// The Basic-Quaternion Target has a simple structure with no options. Position is specified as a point in an LTP-ENU frame and rotation is specified as a unit quaternion.
    ///
    /// Requirements derived from section 8.4.2 of version 1.0.0 of the GeoPose spec http://www.opengis.net/doc/DIS/geopose/1.0.
    /// </summary>
    public struct BasicQuaternionSdu : IStructuralDataUnit, IEquatable<BasicQuaternionSdu>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BasicQuaternionSdu"/> struct.
        /// </summary>
        /// <param name="position"> The position in latitude and longitude in decimal degrees, and height in meters.</param>
        /// <param name="quaternion">A quaternion representing the orientation.</param>
        public BasicQuaternionSdu(TangentPointPosition position, UnitQuaternion quaternion)
        {
            this.Position = position;
            this.Quaternion = quaternion;
        }

        /// <summary>
        /// Gets the position in latitude and longitude in decimal degrees, and height in meters.
        /// </summary>
        [JsonProperty("position")]
        public TangentPointPosition Position { get; }

        /// <summary>
        /// Gets a quaternion representing the orientation.
        /// </summary>
        [JsonProperty("quaternion")]
        public UnitQuaternion Quaternion { get; }

        /// <summary>
        /// Performs an equality comparison on two objects of type <see cref="BasicQuaternionSdu"/>.
        /// </summary>
        /// <param name="left">The first item.</param>
        /// <param name="right">The second item.</param>
        /// <returns>Whether the two items are equal.</returns>
        public static bool operator ==(BasicQuaternionSdu left, BasicQuaternionSdu right) => left.Equals(right);

        /// <summary>
        /// Performs an equality comparison on two objects of type <see cref="BasicQuaternionSdu"/>.
        /// </summary>
        /// <param name="left">The first item.</param>
        /// <param name="right">The second item.</param>
        /// <returns>Whether the two items are not equal.</returns>
        public static bool operator !=(BasicQuaternionSdu left, BasicQuaternionSdu right) => !(left == right);

        /// <inheritdoc/>
        public bool Equals(BasicQuaternionSdu other)
        {
            return this.Position.Equals(other.Position)
                && this.Quaternion.Equals(other.Quaternion);
        }

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            return obj is BasicQuaternionSdu equatable && this.Equals(equatable);
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            unchecked
            {
                var hashcode = this.Position.GetHashCode();
                hashcode = (hashcode * 397) ^ this.Quaternion.GetHashCode();
                return hashcode;
            }
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return $"Position:[{this.Position}], Quaternion:[{this.Quaternion}]";
        }
    }
}