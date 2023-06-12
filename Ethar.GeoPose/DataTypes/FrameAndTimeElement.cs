// <copyright file="FrameAndTimeElement.cs" company="Ethar">
// Copyright (c) Ethar. All rights reserved.
// </copyright>

namespace Ethar.GeoPose.DataTypes
{
    using System;
    using Ethar.GeoPose.FrameSpecifications;
    using Newtonsoft.Json;

    /// <summary>
    /// A construct that represents a frame and time used in a chain of poses.
    ///
    /// Requirements derived from Figure 13 in section 7.2.4 of version 1.0.0 of the GeoPose spec http://www.opengis.net/doc/DIS/geopose/1.0.
    /// </summary>
    public struct FrameAndTimeElement : IEquatable<FrameAndTimeElement>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FrameAndTimeElement"/> struct.
        /// </summary>
        /// <param name="frame">The <see cref="BaseFrameSpecification"/>.</param>
        /// <param name="validTime">The valid time.</param>
        public FrameAndTimeElement(BaseFrameSpecification frame, long validTime)
        {
            this.Frame = frame;
            this.ValidTime = validTime;
        }

        /// <summary>
        /// Gets or sets the frame specification.
        /// </summary>
        [JsonProperty("frame")]
        public BaseFrameSpecification Frame { get; set; }

        /// <summary>
        /// Gets or sets the valid time.
        /// </summary>
        [JsonProperty("validTime")]
        public long ValidTime { get; set; }

        /// <summary>
        /// Performs an equality comparison on two objects of type <see cref="FrameAndTimeElement"/>.
        /// </summary>
        /// <param name="a">The first item.</param>
        /// <param name="b">The second item.</param>
        /// <returns>Whether the two items are equal.</returns>
        public static bool operator ==(FrameAndTimeElement a, FrameAndTimeElement b) => a.Equals(b);

        /// <summary>
        /// Performs an equality comparison on two objects of type <see cref="FrameAndTimeElement"/>.
        /// </summary>
        /// <param name="a">The first item.</param>
        /// <param name="b">The second item.</param>
        /// <returns>Whether the two items are not equal.</returns>
        public static bool operator !=(FrameAndTimeElement a, FrameAndTimeElement b) => !(a == b);

        /// <inheritdoc/>
        public bool Equals(FrameAndTimeElement other)
        {
            return this.ValidTime.Equals(other.ValidTime)
                && this.Frame.Equals(other.Frame);
        }

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            return obj is FrameAndTimeElement equatable && this.Equals(equatable);
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            unchecked
            {
                var hashcode = this.ValidTime.GetHashCode();
                hashcode = (hashcode * 397) ^ this.Frame.GetHashCode();
                return hashcode;
            }
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return $"Frame:[{this.Frame}], ValidTime:{this.ValidTime}";
        }
    }
}