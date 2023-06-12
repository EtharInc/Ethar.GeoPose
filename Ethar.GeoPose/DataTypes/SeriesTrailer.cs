// <copyright file="SeriesTrailer.cs" company="Ethar">
// Copyright (c) Ethar. All rights reserved.
// </copyright>

namespace Ethar.GeoPose.DataTypes
{
    using System;
    using Newtonsoft.Json;

    /// <summary>
    /// A construct that represents the series trailer.
    ///
    /// Requirements derived from figure 13 in section 7.2.4 of version 1.0.0 of the GeoPose spec http://www.opengis.net/doc/DIS/geopose/1.0.
    /// </summary>
    public struct SeriesTrailer : IEquatable<SeriesTrailer>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SeriesTrailer"/> struct.
        /// </summary>
        /// <param name="poseCount">The pose count.</param>
        /// <param name="integrityCheck">The integrity check.</param>
        public SeriesTrailer(int poseCount, string integrityCheck)
        {
            this.PoseCount = poseCount;
            this.IntegrityCheck = integrityCheck;
        }

        /// <summary>
        /// Gets or sets the number of poses in the series.
        /// </summary>
        [JsonProperty("poseCount")]
        public int PoseCount { get; set; }

        /// <summary>
        /// Gets or sets the integrity check.
        /// </summary>
        [JsonProperty("integrityCheck")]
        public string IntegrityCheck { get; set; }

        /// <summary>
        /// Performs an equality comparison on two objects of type <see cref="SeriesTrailer"/>.
        /// </summary>
        /// <param name="a">The first item.</param>
        /// <param name="b">The second item.</param>
        /// <returns>Whether the two items are equal.</returns>
        public static bool operator ==(SeriesTrailer a, SeriesTrailer b) => a.Equals(b);

        /// <summary>
        /// Performs an equality comparison on two objects of type <see cref="SeriesTrailer"/>.
        /// </summary>
        /// <param name="a">The first item.</param>
        /// <param name="b">The second item.</param>
        /// <returns>Whether the two items are not equal.</returns>
        public static bool operator !=(SeriesTrailer a, SeriesTrailer b) => !(a == b);

        /// <inheritdoc/>
        public bool Equals(SeriesTrailer other)
        {
            return this.PoseCount.Equals(other.PoseCount)
                && this.IntegrityCheck.Equals(other.IntegrityCheck);
        }

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            return obj is SeriesTrailer equatable && this.Equals(equatable);
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            unchecked
            {
                var hashcode = this.PoseCount.GetHashCode();
                hashcode = (hashcode * 397) ^ this.IntegrityCheck.GetHashCode();
                return hashcode;
            }
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return $"PoseCount:{this.PoseCount}, IntegrityCheck:{this.IntegrityCheck}";
        }
    }
}
