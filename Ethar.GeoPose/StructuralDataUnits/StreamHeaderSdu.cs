// <copyright file="StreamHeaderSdu.cs" company="Ethar">
// Copyright (c) Ethar. All rights reserved.
// </copyright>

namespace Ethar.GeoPose.StructuralDataUnits
{
    using System;
    using Ethar.GeoPose.FrameSpecifications;
    using Ethar.GeoPose.Interfaces;
    using Ethar.GeoPose.TransitionModels;
    using Newtonsoft.Json;

    /// <summary>
    /// A construct that represents a Stream Header SDU.
    ///
    /// Requirements derived from section 8.4.8 of version 1.0.0 of the GeoPose spec http://www.opengis.net/doc/DIS/geopose/1.0.
    /// </summary>
    public struct StreamHeaderSdu : IStructuralDataUnit, IEquatable<StreamHeaderSdu>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StreamHeaderSdu"/> struct.
        /// </summary>
        /// <param name="transitionModel">The transition model.</param>
        /// <param name="outerFrame">The outer frame for the stream.</param>
        public StreamHeaderSdu(TransitionModel transitionModel, BaseFrameSpecification outerFrame)
        {
            this.TransitionModel = transitionModel;
            this.OuterFrame = outerFrame;
        }

        /// <summary>
        /// Gets the transition model.
        /// </summary>
        [JsonProperty("transitionModel")]
        public TransitionModel TransitionModel { get; }

        /// <summary>
        /// Gets the outer frame.
        /// </summary>
        [JsonProperty("outerFrame")]
        public BaseFrameSpecification OuterFrame { get; }

        /// <summary>
        /// Performs an equality comparison on two objects of type <see cref="StreamHeaderSdu"/>.
        /// </summary>
        /// <param name="a">The first item.</param>
        /// <param name="b">The second item.</param>
        /// <returns>Whether the two items are equal.</returns>
        public static bool operator ==(StreamHeaderSdu a, StreamHeaderSdu b) => a.Equals(b);

        /// <summary>
        /// Performs an equality comparison on two objects of type <see cref="StreamHeaderSdu"/>.
        /// </summary>
        /// <param name="a">The first item.</param>
        /// <param name="b">The second item.</param>
        /// <returns>Whether the two items are not equal.</returns>
        public static bool operator !=(StreamHeaderSdu a, StreamHeaderSdu b) => !(a == b);

        /// <inheritdoc/>
        public bool Equals(StreamHeaderSdu other)
        {
            return this.TransitionModel.Equals(other.TransitionModel)
                   && this.OuterFrame.Equals(other.OuterFrame);
        }

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            return obj is StreamHeaderSdu equatable && this.Equals(equatable);
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            unchecked
            {
                var hashcode = this.TransitionModel.GetHashCode();
                hashcode = (hashcode * 397) ^ this.OuterFrame.GetHashCode();
                return hashcode;
            }
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return $"TransitionModel:[{this.TransitionModel}], OuterFrame:[{this.OuterFrame}]";
        }
    }
}