// <copyright file="LtpNedSpecification.cs" company="Ethar">
// Copyright (c) Ethar. All rights reserved.
// </copyright>

namespace Ethar.GeoPose.Authority.FrameSpecifications
{
    using System;
    using Ethar.GeoPose.DataTypes;
    using Ethar.GeoPose.FrameSpecifications;
    using Newtonsoft.Json;

    /// <summary>
    /// A frame specification with a local tangent plane coordinate system specialized to an north-east-down system, where the X axis is aligned toward north and the Y axis toward east.
    ///
    /// Derived from Figure 8 from section 7.2.1 of version 1.0.0 of the GeoPose spec http://www.opengis.net/doc/DIS/geopose/1.0.
    /// </summary>
    public class LtpNedSpecification : BaseFrameSpecification, IEquatable<LtpNedSpecification>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LtpNedSpecification"/> class.
        /// </summary>
        public LtpNedSpecification()
            : base(EtharFrameSpecificationTypes.LtpNedSpec, Constants.AuthorityName)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LtpNedSpecification"/> class.
        /// </summary>
        /// <param name="position"> The position in latitude and longitude in decimal degrees, and height in meters.</param>
        public LtpNedSpecification(TangentPointPosition position)
            : base(EtharFrameSpecificationTypes.LtpNedSpec, Constants.AuthorityName)
        {
            this.Position = position;
        }

        /// <summary>
        ///  Gets the position in latitude and longitude in decimal degrees, and height in meters.
        /// </summary>
        [JsonProperty("position")]
        public TangentPointPosition Position { get; }

        /// <inheritdoc/>
        public bool Equals(LtpNedSpecification other)
        {
            return this.Position == other.Position
                   && base.Equals(other);
        }

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            return obj is LtpNedSpecification equatable && this.Equals(equatable);
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            unchecked
            {
                var hashcode = base.GetHashCode();
                hashcode = (hashcode * 397) ^ this.Position.GetHashCode();
                return hashcode;
            }
        }
    }
}
