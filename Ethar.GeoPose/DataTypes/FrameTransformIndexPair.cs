// <copyright file="FrameTransformIndexPair.cs" company="Ethar">
// Copyright (c) Ethar. All rights reserved.
// </copyright>

namespace Ethar.GeoPose.DataTypes
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Ethar.GeoPose.JsonConversion;
    using Newtonsoft.Json;

    /// <summary>
    /// A construct that represents a outer frame index and inner frame index pair in a graph.
    ///
    /// Requirements derived from Figure 12 of section 7.2.4 of version 1.0.0 of the GeoPose spec http://www.opengis.net/doc/DIS/geopose/1.0.
    /// </summary>
    [JsonConverter(typeof(FrameTransformIndexPairJsonConverter))]
    public struct FrameTransformIndexPair : IEquatable<FrameTransformIndexPair>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FrameTransformIndexPair"/> struct.
        /// </summary>
        /// <param name="link">Array where the first item is the outer frame index and the last item is the inner frame index.</param>
        public FrameTransformIndexPair(IEnumerable<int> link)
        {
            this.OuterFrameIndex = link.First();
            this.InnerFrameIndex = link.Last();
        }

        /// <summary>
        /// Gets or sets the outer frame index.
        /// </summary>
        [JsonProperty("outerFrameIndex")]
        public int OuterFrameIndex { get; set; }

        /// <summary>
        /// Gets or sets the inner frame index.
        /// </summary>
        [JsonProperty("innerFrameIndex")]
        public int InnerFrameIndex { get; set; }

        /// <summary>
        /// Performs an equality comparison on two objects of type <see cref="FrameTransformIndexPair"/>.
        /// </summary>
        /// <param name="a">The first item.</param>
        /// <param name="b">The second item.</param>
        /// <returns>Whether the two items are equal.</returns>
        public static bool operator ==(FrameTransformIndexPair a, FrameTransformIndexPair b) => a.Equals(b);

        /// <summary>
        /// Performs an equality comparison on two objects of type <see cref="FrameTransformIndexPair"/>.
        /// </summary>
        /// <param name="a">The first item.</param>
        /// <param name="b">The second item.</param>
        /// <returns>Whether the two items are not equal.</returns>
        public static bool operator !=(FrameTransformIndexPair a, FrameTransformIndexPair b) => !(a == b);

        /// <inheritdoc/>
        public bool Equals(FrameTransformIndexPair other)
        {
            return this.OuterFrameIndex.Equals(other.OuterFrameIndex)
                && this.InnerFrameIndex.Equals(other.InnerFrameIndex);
        }

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            return obj is FrameTransformIndexPair equatable && this.Equals(equatable);
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            unchecked
            {
                var hashcode = this.OuterFrameIndex.GetHashCode();
                hashcode = (hashcode * 397) ^ this.InnerFrameIndex.GetHashCode();
                return hashcode;
            }
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return $"OuterFrameIndex:{this.OuterFrameIndex}, InnerFrameIndex:{this.InnerFrameIndex}";
        }
    }
}
