// <copyright file="RegularSeriesSdu.cs" company="Ethar">
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
    /// A construct that represents a Regular Series SDU.
    ///
    /// Requirements derived from section 8.4.6 of version 1.0.0 of the GeoPose spec http://www.opengis.net/doc/DIS/geopose/1.0.
    /// </summary>
    public struct RegularSeriesSdu : IStructuralDataUnit, IEquatable<RegularSeriesSdu>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RegularSeriesSdu"/> struct.
        /// </summary>
        /// <param name="header">The series header.</param>
        /// <param name="interPoseDuration">The inter pose duration.</param>
        /// <param name="outerFrame">The outer frame for the series.</param>
        /// <param name="innerFrameSeries">The inner frames.</param>
        /// <param name="trailer">The series trailer.</param>
        public RegularSeriesSdu(SeriesHeader header, GeoPoseDuration interPoseDuration, BaseFrameSpecification outerFrame, IEnumerable<BaseFrameSpecification> innerFrameSeries, SeriesTrailer trailer)
        {
            this.Header = header;
            this.InterPoseDuration = interPoseDuration;
            this.OuterFrame = outerFrame;
            this.InnerFrameSeries = innerFrameSeries;
            this.Trailer = trailer;
        }

        /// <summary>
        /// Gets the series header.
        /// </summary>
        [JsonProperty("header")]
        public SeriesHeader Header { get; }

        /// <summary>
        /// Gets the inter pose duration.
        /// </summary>
        [JsonProperty("interPoseDuration")]
        public GeoPoseDuration InterPoseDuration { get; }

        /// <summary>
        /// Gets the outer frame for the series.
        /// </summary>
        [JsonProperty("outerFrame")]
        public BaseFrameSpecification OuterFrame { get; }

        /// <summary>
        /// Gets the inner frame series.
        /// </summary>
        [JsonProperty("innerFrameSeries")]
        public IEnumerable<BaseFrameSpecification> InnerFrameSeries { get; }

        /// <summary>
        /// Gets the series trailer.
        /// </summary>
        [JsonProperty("trailer")]
        public SeriesTrailer Trailer { get; }

        /// <summary>
        /// Performs an equality comparison on two objects of type <see cref="RegularSeriesSdu"/>.
        /// </summary>
        /// <param name="a">The first item.</param>
        /// <param name="b">The second item.</param>
        /// <returns>Whether the two items are equal.</returns>
        public static bool operator ==(RegularSeriesSdu a, RegularSeriesSdu b) => a.Equals(b);

        /// <summary>
        /// Performs an equality comparison on two objects of type <see cref="RegularSeriesSdu"/>.
        /// </summary>
        /// <param name="a">The first item.</param>
        /// <param name="b">The second item.</param>
        /// <returns>Whether the two items are not equal.</returns>
        public static bool operator !=(RegularSeriesSdu a, RegularSeriesSdu b) => !(a == b);

        /// <inheritdoc/>
        public bool Equals(RegularSeriesSdu other)
        {
            return this.Header.Equals(other.Header)
                   && this.InterPoseDuration.Equals(other.InterPoseDuration)
                   && this.OuterFrame.Equals(other.OuterFrame)
                   && this.InnerFrameSeries.SequenceEqual(other.InnerFrameSeries)
                   && this.Trailer.Equals(other.Trailer);
        }

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            return obj is RegularSeriesSdu equatable && this.Equals(equatable);
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            unchecked
            {
                var hashcode = this.Header.GetHashCode();
                hashcode = (hashcode * 397) ^ this.OuterFrame.GetHashCode();
                hashcode = (hashcode * 397) ^ this.InnerFrameSeries.GetHashCodeForCollection();
                hashcode = (hashcode * 397) ^ this.InterPoseDuration.GetHashCode();
                hashcode = (hashcode * 397) ^ this.Trailer.GetHashCode();
                return hashcode;
            }
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return $"Header:[{this.Header}], InterPoseDuration:[{this.InterPoseDuration}], OuterFrame:[{this.OuterFrame}], InnerFrameSeriesCount:{this.InnerFrameSeries?.Count() ?? 0}, Trailer:[{this.Trailer}]";
        }
    }
}