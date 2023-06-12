// <copyright file="GraphSdu.cs" company="Ethar">
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
    /// A construct that represents a Graph SDU.
    /// The Graph Target supports a network of object relative poses. The graph is a directed acyclic graph, each node must either be an Extrinsic Frame or reachable from an Extrinsic Frame.
    ///
    /// Requirements derived from section 8.4.4 of version 1.0.0 of the GeoPose spec http://www.opengis.net/doc/DIS/geopose/1.0.
    /// </summary>
    public struct GraphSdu : IStructuralDataUnit, IEquatable<GraphSdu>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GraphSdu"/> struct.
        /// </summary>
        /// <param name="validTime">The Unix Timestamp representing the valid time.</param>
        /// <param name="frameList">List of frame specifications in the graph.</param>
        /// <param name="transformList">List of <see cref="FrameTransformIndexPair"/> that represent edges in the graph.</param>
        public GraphSdu(long validTime, IList<BaseFrameSpecification> frameList, IList<FrameTransformIndexPair> transformList)
        {
            this.ValidTime = validTime;
            this.FrameList = frameList;
            this.TransformList = transformList;
        }

        /// <summary>
        /// Gets the Unix Timestamp representing the valid time.
        /// </summary>
        [JsonProperty("validTime")]
        public long ValidTime { get; }

        /// <summary>
        /// Gets a list of frame specifications in the graph.
        /// </summary>
        [JsonProperty("frameList")]
        public IList<BaseFrameSpecification> FrameList { get; }

        /// <summary>
        /// Gets a list of <see cref="FrameTransformIndexPair"/> that represent edges in the graph.
        /// </summary>
        [JsonProperty("transformList")]
        public IList<FrameTransformIndexPair> TransformList { get; }

        /// <summary>
        /// Performs an equality comparison on two objects of type <see cref="GraphSdu"/>.
        /// </summary>
        /// <param name="a">The first item.</param>
        /// <param name="b">The second item.</param>
        /// <returns>Whether the two items are equal.</returns>
        public static bool operator ==(GraphSdu a, GraphSdu b) => a.Equals(b);

        /// <summary>
        /// Performs an equality comparison on two objects of type <see cref="GraphSdu"/>.
        /// </summary>
        /// <param name="a">The first item.</param>
        /// <param name="b">The second item.</param>
        /// <returns>Whether the two items are not equal.</returns>
        public static bool operator !=(GraphSdu a, GraphSdu b) => !(a == b);

        /// <inheritdoc/>
        public bool Equals(GraphSdu other)
        {
            return this.ValidTime.Equals(other.ValidTime)
                && this.FrameList.SequenceEqual(other.FrameList)
                && this.TransformList.SequenceEqual(other.TransformList);
        }

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            return obj is GraphSdu equatable && this.Equals(equatable);
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            unchecked
            {
                var hashcode = this.ValidTime.GetHashCode();
                hashcode = (hashcode * 397) ^ this.FrameList.GetHashCodeForCollection();
                hashcode = (hashcode * 397) ^ this.TransformList.GetHashCodeForCollection();
                return hashcode;
            }
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return $"ValidTime:{this.ValidTime}, FrameListCount:{this.FrameList?.Count ?? 0}, TransformListCount:{this.TransformList?.Count ?? 0}";
        }
    }
}