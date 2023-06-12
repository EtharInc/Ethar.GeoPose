// <copyright file="ChainSdu.cs" company="Ethar">
// Copyright (c) Ethar. All rights reserved.
// </copyright>

namespace Ethar.GeoPose.StructuralDataUnits
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Ethar.GeoPose.DataTypes;
    using Ethar.GeoPose.Extensions;
    using Ethar.GeoPose.FrameSpecifications;
    using Ethar.GeoPose.Interfaces;
    using Newtonsoft.Json;

    /// <summary>
    /// A construct that represents a Chain SDU.
    ///
    /// Requirements derived from section 8.4.5 of version 1.0.0 of the GeoPose spec http://www.opengis.net/doc/DIS/geopose/1.0.
    /// </summary>
    public struct ChainSdu : IStructuralDataUnit, IEquatable<ChainSdu>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ChainSdu"/> struct.
        /// </summary>
        /// <param name="validTime">The Unix Timestamp representing the valid time.</param>
        /// <param name="outerFrame">The extrinsic frame specification for this SDU.</param>
        /// <param name="frameChain">Ordered chain of <see cref="BaseFrameSpecification"/>.</param>
        public ChainSdu(long validTime, BaseFrameSpecification outerFrame, IList<BaseFrameSpecification> frameChain)
        {
            this.ValidTime = validTime;
            this.OuterFrame = outerFrame;
            this.FrameChain = frameChain;
        }

        /// <summary>
        /// Gets the Unix Timestamp representing the valid time.
        /// </summary>
        [JsonProperty("validTime")]
        public long ValidTime { get; }

        /// <summary>
        /// Gets the outer frame.
        /// </summary>
        [JsonProperty("outerFrame")]
        public BaseFrameSpecification OuterFrame { get; }

        /// <summary>
        /// Gets the frame chain.
        /// </summary>
        [JsonProperty("frameChain")]
        public IList<BaseFrameSpecification> FrameChain { get; }

        /// <summary>
        /// Performs an equality comparison on two objects of type <see cref="ChainSdu"/>.
        /// </summary>
        /// <param name="a">The first item.</param>
        /// <param name="b">The second item.</param>
        /// <returns>Whether the two items are equal.</returns>
        public static bool operator ==(ChainSdu a, ChainSdu b) => a.Equals(b);

        /// <summary>
        /// Performs an equality comparison on two objects of type <see cref="ChainSdu"/>.
        /// </summary>
        /// <param name="a">The first item.</param>
        /// <param name="b">The second item.</param>
        /// <returns>Whether the two items are not equal.</returns>
        public static bool operator !=(ChainSdu a, ChainSdu b) => !(a == b);

        /// <inheritdoc/>
        public bool Equals(ChainSdu other)
        {
            return this.ValidTime.Equals(other.ValidTime)
                   && this.OuterFrame.Equals(other.OuterFrame)
                   && this.FrameChain.SequenceEqual(other.FrameChain);
        }

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            return obj is ChainSdu equatable && this.Equals(equatable);
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            unchecked
            {
                var hashcode = this.ValidTime.GetHashCode();
                hashcode = (hashcode * 397) ^ this.FrameChain.GetHashCodeForCollection();
                hashcode = (hashcode * 397) ^ this.OuterFrame.GetHashCode();
                return hashcode;
            }
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return $"ValidTime:{this.ValidTime}, OuterFrame:[{this.OuterFrame}], FrameChainCount:{this.FrameChain?.Count ?? 0}";
        }
    }
}