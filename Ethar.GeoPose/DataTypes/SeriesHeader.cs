// <copyright file="SeriesHeader.cs" company="Ethar">
// Copyright (c) Ethar. All rights reserved.
// </copyright>

namespace Ethar.GeoPose.DataTypes
{
    using System;
    using Ethar.GeoPose.TransitionModels;
    using Newtonsoft.Json;

    /// <summary>
    /// A construct that represents the series trailer.
    ///
    /// Requirements derived from figure 13 in section 7.2.4 of version 1.0.0 of the GeoPose spec http://www.opengis.net/doc/DIS/geopose/1.0.
    /// </summary>
    public struct SeriesHeader : IEquatable<SeriesHeader>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SeriesHeader"/> struct.
        /// </summary>
        /// <param name="transitionModel">The transition model.</param>
        /// <param name="poseCount">The pose count.</param>
        /// <param name="startInstant">The Unix timestamp that represents the start instant.</param>
        /// <param name="stopInstant">The Unix timestamp that represents the stop instant.</param>
        /// <param name="integrityCheck">The integrity check.</param>
        public SeriesHeader(TransitionModel transitionModel, int poseCount, long startInstant, long stopInstant, string integrityCheck)
        {
            this.TransitionModel = transitionModel;
            this.PoseCount = poseCount;
            this.StartInstant = startInstant;
            this.StopInstant = stopInstant;
            this.IntegrityCheck = integrityCheck;
        }

        /// <summary>
        /// Gets or sets the transition model.
        /// </summary>
        [JsonProperty("transitionModel")]
        public TransitionModel TransitionModel { get; set; }

        /// <summary>
        /// Gets or sets the number of poses in the series.
        /// </summary>
        [JsonProperty("poseCount")]
        public int PoseCount { get; set; }

        /// <summary>
        /// Gets or sets the Unix timestamp that represents the start instant.
        /// </summary>
        [JsonProperty("startInstant")]
        public long StartInstant { get; set; }

        /// <summary>
        /// Gets or sets the Unix timestamp that represents the stop instant.
        /// </summary>
        [JsonProperty("stopInstant")]
        public long StopInstant { get; set; }

        /// <summary>
        /// Gets or sets the integrity check.
        /// </summary>
        [JsonProperty("integrityCheck")]
        public string IntegrityCheck { get; set; }

        /// <summary>
        /// Performs an equality comparison on two objects of type <see cref="SeriesHeader"/>.
        /// </summary>
        /// <param name="a">The first item.</param>
        /// <param name="b">The second item.</param>
        /// <returns>Whether the two items are equal.</returns>
        public static bool operator ==(SeriesHeader a, SeriesHeader b) => a.Equals(b);

        /// <summary>
        /// Performs an equality comparison on two objects of type <see cref="SeriesHeader"/>.
        /// </summary>
        /// <param name="a">The first item.</param>
        /// <param name="b">The second item.</param>
        /// <returns>Whether the two items are not equal.</returns>
        public static bool operator !=(SeriesHeader a, SeriesHeader b) => !(a == b);

        /// <inheritdoc/>
        public bool Equals(SeriesHeader other)
        {
            return this.TransitionModel.Equals(other.TransitionModel)
                && this.PoseCount.Equals(other.PoseCount)
                && this.StartInstant.Equals(other.StartInstant)
                && this.StopInstant.Equals(other.StopInstant)
                && this.IntegrityCheck.Equals(other.IntegrityCheck);
        }

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            return obj is SeriesHeader equatable && this.Equals(equatable);
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            unchecked
            {
                var hashcode = this.PoseCount.GetHashCode();
                hashcode = (hashcode * 397) ^ this.IntegrityCheck.GetHashCode();
                hashcode = (hashcode * 397) ^ this.StartInstant.GetHashCode();
                hashcode = (hashcode * 397) ^ this.StopInstant.GetHashCode();
                hashcode = (hashcode * 397) ^ this.TransitionModel.GetHashCode();
                return hashcode;
            }
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return $"TransitionModel:[{this.TransitionModel}], PoseCount:{this.PoseCount}, StartInstant:{this.StartInstant}, StopInstant:{this.StopInstant}, IntegrityCheck:{this.IntegrityCheck}";
        }
    }
}
