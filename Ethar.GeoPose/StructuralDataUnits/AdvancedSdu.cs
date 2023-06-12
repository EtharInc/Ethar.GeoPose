// <copyright file="AdvancedSdu.cs" company="Ethar">
// Copyright (c) Ethar. All rights reserved.
// </copyright>

namespace Ethar.GeoPose.StructuralDataUnits
{
    using System;
    using Ethar.GeoPose.DataTypes;
    using Ethar.GeoPose.FrameSpecifications;
    using Ethar.GeoPose.Interfaces;
    using Newtonsoft.Json;

    /// <summary>
    /// A construct that represents an Advanced SDU.
    /// The Advanced Target has a more general structure than Basic-YPR and Basic-Quaternion, supporting flexible specification of Outer Frame and a Valid Time.
    ///
    /// Requirements derived from section 8.4.3 of version 1.0.0 of the GeoPose spec http://www.opengis.net/doc/DIS/geopose/1.0.
    /// </summary>
    public struct AdvancedSdu : IStructuralDataUnit, IEquatable<AdvancedSdu>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AdvancedSdu"/> struct.
        /// </summary>
        /// <param name="validTime">The Unix Timestamp representing the valid time.</param>
        /// <param name="quaternion">The quaternion to apply to the <paramref name="frameSpecification"/>.</param>.
        /// <param name="frameSpecification">The reference frame for the SDU, must be an extrinsic frame specification.</param>
        public AdvancedSdu(long validTime, UnitQuaternion quaternion, BaseFrameSpecification frameSpecification)
        {
            this.ValidTime = validTime;
            this.Quaternion = quaternion;
            this.FrameSpecification = frameSpecification;
        }

        /// <summary>
        /// Gets the Unix Timestamp representing the valid time.
        /// </summary>
        [JsonProperty("validTime")]
        public long ValidTime { get; }

        /// <summary>
        /// Gets the quaternion to apply to the <see cref="FrameSpecification"/>.
        /// </summary>
        [JsonProperty("quaternion")]
        public UnitQuaternion Quaternion { get; }

        /// <summary>
        /// Gets the reference frame specification for the SDU.
        /// </summary>
        [JsonProperty("frameSpecification")]
        public BaseFrameSpecification FrameSpecification { get; }

        /// <summary>
        /// Performs an equality comparison on two objects of type <see cref="AdvancedSdu"/>.
        /// </summary>
        /// <param name="a">The first item.</param>
        /// <param name="b">The second item.</param>
        /// <returns>Whether the two items are equal.</returns>
        public static bool operator ==(AdvancedSdu a, AdvancedSdu b) => a.Equals(b);

        /// <summary>
        /// Performs an equality comparison on two objects of type <see cref="AdvancedSdu"/>.
        /// </summary>
        /// <param name="a">The first item.</param>
        /// <param name="b">The second item.</param>
        /// <returns>Whether the two items are not equal.</returns>
        public static bool operator !=(AdvancedSdu a, AdvancedSdu b) => !(a == b);

        /// <inheritdoc/>
        public bool Equals(AdvancedSdu other)
        {
            return this.ValidTime.Equals(other.ValidTime)
                   && this.Quaternion.Equals(other.Quaternion)
                   && this.FrameSpecification.Equals(other.FrameSpecification);
        }

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            return obj is AdvancedSdu equatable && this.Equals(equatable);
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            unchecked
            {
                var hashcode = this.ValidTime.GetHashCode();
                hashcode = (hashcode * 397) ^ this.Quaternion.GetHashCode();
                hashcode = (hashcode * 397) ^ this.FrameSpecification.GetHashCode();
                return hashcode;
            }
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return $"ValidTime:{this.ValidTime}, Quaternion:[{this.Quaternion}], FrameSpecification:[{this.FrameSpecification}]";
        }
    }
}