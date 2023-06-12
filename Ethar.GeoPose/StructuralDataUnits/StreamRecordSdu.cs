// <copyright file="StreamRecordSdu.cs" company="Ethar">
// Copyright (c) Ethar. All rights reserved.
// </copyright>

namespace Ethar.GeoPose.StructuralDataUnits
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Ethar.GeoPose.Extensions;
    using Ethar.GeoPose.Interfaces;
    using Newtonsoft.Json;

    /// <summary>
    /// A construct that represents an Stream Record SDU.
    ///
    /// Requirements derived from section 9.2.9 of version 1.0.0 of the GeoPose spec http://www.opengis.net/doc/DIS/geopose/1.0.
    /// </summary>
    public struct StreamRecordSdu : IStructuralDataUnit, IEquatable<StreamRecordSdu>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StreamRecordSdu"/> struct.
        /// </summary>
        /// <param name="streamHeader">The stream header.</param>
        /// <param name="streamElements">The stream elements.</param>
        public StreamRecordSdu(StreamHeaderSdu streamHeader, IEnumerable<StreamElementSdu> streamElements)
        {
            this.StreamElements = streamElements;
            this.StreamHeader = streamHeader;
        }

        /// <summary>
        /// Gets the stream header.
        /// </summary>
        [JsonProperty("header")]
        public StreamHeaderSdu StreamHeader { get; }

        /// <summary>
        /// Gets the stream elements.
        /// </summary>
        [JsonProperty("streamElements")]
        public IEnumerable<StreamElementSdu> StreamElements { get; }

        /// <summary>
        /// Performs an equality comparison on two objects of type <see cref="StreamRecordSdu"/>.
        /// </summary>
        /// <param name="a">The first item.</param>
        /// <param name="b">The second item.</param>
        /// <returns>Whether the two items are equal.</returns>
        public static bool operator ==(StreamRecordSdu a, StreamRecordSdu b) => a.Equals(b);

        /// <summary>
        /// Performs an equality comparison on two objects of type <see cref="StreamRecordSdu"/>.
        /// </summary>
        /// <param name="a">The first item.</param>
        /// <param name="b">The second item.</param>
        /// <returns>Whether the two items are not equal.</returns>
        public static bool operator !=(StreamRecordSdu a, StreamRecordSdu b) => !(a == b);

        /// <inheritdoc/>
        public bool Equals(StreamRecordSdu other)
        {
            return this.StreamHeader.Equals(other.StreamHeader)
                   && this.StreamElements.All(x => other.StreamElements.Any(y => y.Equals(x)));
        }

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            return obj is StreamRecordSdu equatable && this.Equals(equatable);
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            unchecked
            {
                var hashcode = this.StreamHeader.GetHashCode();
                hashcode = (hashcode * 397) ^ this.StreamElements.GetHashCodeForCollection();
                return hashcode;
            }
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return $"StreamHeader:[{this.StreamHeader}], StreamElementsCount:{this.StreamElements?.Count() ?? 0}";
        }
    }
}