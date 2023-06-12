// <copyright file="IrregularSeriesSdu.cs" company="Ethar">
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
    /// A construct that represents an Irregular Series SDU.
    ///
    /// Requirements derived from section 8.4.7 of version 1.0.0 of the GeoPose spec http://www.opengis.net/doc/DIS/geopose/1.0.
    /// </summary>
    public struct IrregularSeriesSdu : IStructuralDataUnit, IEquatable<IrregularSeriesSdu>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="IrregularSeriesSdu"/> struct.
        /// </summary>
        /// <param name="header">The series header.</param>
        /// <param name="outerFrame">The outer frame for the series.</param>
        /// <param name="trailer">The series trailer.</param>
        /// <param name="innerFrameAndTimeSeries">The inner frames and valid times associated with them.</param>
        public IrregularSeriesSdu(SeriesHeader header, BaseFrameSpecification outerFrame, SeriesTrailer trailer, IEnumerable<FrameAndTimeElement> innerFrameAndTimeSeries)
        {
            this.Header = header;
            this.OuterFrame = outerFrame;
            this.Trailer = trailer;
            this.InnerFrameAndTimeSeries = innerFrameAndTimeSeries;
        }

        /// <summary>
        /// Gets the series header.
        /// </summary>
        [JsonProperty("header")]
        public SeriesHeader Header { get; }

        /// <summary>
        /// Gets the outer frame.
        /// </summary>
        [JsonProperty("outerFrame")]
        public BaseFrameSpecification OuterFrame { get; }

        /// <summary>
        /// Gets the inner frames and their valid times.
        /// </summary>
        [JsonProperty("innerFrameAndTimeSeries")]
        public IEnumerable<FrameAndTimeElement> InnerFrameAndTimeSeries { get; }

        /// <summary>
        /// Gets the series trailer.
        /// </summary>
        [JsonProperty("trailer")]
        public SeriesTrailer Trailer { get; }

        /// <summary>
        /// Performs an equality comparison on two objects of type <see cref="IrregularSeriesSdu"/>.
        /// </summary>
        /// <param name="a">The first item.</param>
        /// <param name="b">The second item.</param>
        /// <returns>Whether the two items are equal.</returns>
        public static bool operator ==(IrregularSeriesSdu a, IrregularSeriesSdu b) => a.Equals(b);

        /// <summary>
        /// Performs an equality comparison on two objects of type <see cref="IrregularSeriesSdu"/>.
        /// </summary>
        /// <param name="a">The first item.</param>
        /// <param name="b">The second item.</param>
        /// <returns>Whether the two items are not equal.</returns>
        public static bool operator !=(IrregularSeriesSdu a, IrregularSeriesSdu b) => !(a == b);

        /// <inheritdoc/>
        public bool Equals(IrregularSeriesSdu other)
        {
            return this.Header.Equals(other.Header)
                   && this.OuterFrame.Equals(other.OuterFrame)
                   && this.InnerFrameAndTimeSeries.SequenceEqual(other.InnerFrameAndTimeSeries)
                   && this.Trailer.Equals(other.Trailer);
        }

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            return obj is IrregularSeriesSdu equatable && this.Equals(equatable);
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            unchecked
            {
                var hashcode = this.Header.GetHashCode();
                hashcode = (hashcode * 397) ^ this.OuterFrame.GetHashCode();
                hashcode = (hashcode * 397) ^ this.InnerFrameAndTimeSeries.GetHashCodeForCollection();
                hashcode = (hashcode * 397) ^ this.Trailer.GetHashCode();
                return hashcode;
            }
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return $"Header:[{this.Header}], OuterFrame:[{this.OuterFrame}], InnerFrameAndTimeSeriesCount:{this.InnerFrameAndTimeSeries?.Count() ?? 0}, Trailer:[{this.Trailer}]";
        }
    }
}