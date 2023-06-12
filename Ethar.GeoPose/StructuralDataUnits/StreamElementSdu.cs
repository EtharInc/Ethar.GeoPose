// <copyright file="StreamElementSdu.cs" company="Ethar">
// Copyright (c) Ethar. All rights reserved.
// </copyright>

namespace Ethar.GeoPose.StructuralDataUnits
{
    using System;
    using Ethar.GeoPose.DataTypes;
    using Ethar.GeoPose.Interfaces;
    using Newtonsoft.Json;

    /// <summary>
    /// A construct that represents a Stream Element SDU.
    ///
    /// Requirements derived from section 8.4.8 of version 1.0.0 of the GeoPose spec http://www.opengis.net/doc/DIS/geopose/1.0.
    /// </summary>
    public struct StreamElementSdu : IStructuralDataUnit, IEquatable<StreamElementSdu>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StreamElementSdu"/> struct.
        /// </summary>
        /// <param name="streamElement">The stream element.</param>
        public StreamElementSdu(FrameAndTimeElement streamElement)
        {
            this.StreamElement = streamElement;
        }

        /// <summary>
        /// Gets the stream element.
        /// </summary>
        [JsonProperty("streamElement")]
        public FrameAndTimeElement StreamElement { get; }

        /// <summary>
        /// Performs an equality comparison on two objects of type <see cref="StreamElementSdu"/>.
        /// </summary>
        /// <param name="a">The first item.</param>
        /// <param name="b">The second item.</param>
        /// <returns>Whether the two items are equal.</returns>
        public static bool operator ==(StreamElementSdu a, StreamElementSdu b) => a.Equals(b);

        /// <summary>
        /// Performs an equality comparison on two objects of type <see cref="StreamElementSdu"/>.
        /// </summary>
        /// <param name="a">The first item.</param>
        /// <param name="b">The second item.</param>
        /// <returns>Whether the two items are not equal.</returns>
        public static bool operator !=(StreamElementSdu a, StreamElementSdu b) => !(a == b);

        /// <inheritdoc/>
        public bool Equals(StreamElementSdu other)
        {
            return this.StreamElement.Equals(other.StreamElement);
        }

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            return obj is StreamElementSdu equatable && this.Equals(equatable);
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            unchecked
            {
                var hashcode = this.StreamElement.GetHashCode();
                return hashcode;
            }
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return $"StreamElement:[{this.StreamElement}]";
        }
    }
}