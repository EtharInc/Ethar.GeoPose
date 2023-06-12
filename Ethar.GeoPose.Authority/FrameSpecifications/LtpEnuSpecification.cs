// <copyright file="LtpEnuSpecification.cs" company="Ethar">
// Copyright (c) Ethar. All rights reserved.
// </copyright>

namespace Ethar.GeoPose.Authority.FrameSpecifications
{
    using System;
    using Ethar.GeoPose.DataTypes;
    using Ethar.GeoPose.FrameSpecifications;
    using Newtonsoft.Json;

    /// <summary>
    /// A frame specification with a local tangent plane coordinate system specialized to an east-north-up system, where the X axis is aligned toward east and the Y axis toward north.
    ///
    /// Derived from Figure 8 from section 7.2.1 of version 1.0.0 of the GeoPose spec http://www.opengis.net/doc/DIS/geopose/1.0.
    /// </summary>
    public class LtpEnuSpecification : BaseFrameSpecification, IEquatable<LtpEnuSpecification>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LtpEnuSpecification"/> class.
        /// </summary>
        public LtpEnuSpecification()
            : base(EtharFrameSpecificationTypes.LtpEnuSpec, Constants.AuthorityName)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LtpEnuSpecification"/> class.
        /// </summary>
        /// <param name="position">The position in latitude and longitude in decimal degrees, and height in meters.</param>
        public LtpEnuSpecification(TangentPointPosition position)
            : base(EtharFrameSpecificationTypes.LtpEnuSpec, Constants.AuthorityName)
        {
            this.Position = position;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LtpEnuSpecification"/> class.
        /// </summary>
        /// <param name="id">An ID that uniquely defines the frame within the authority.</param>
        /// <param name="authority">A string uniquely specifying a source of reference frame specifications.</param>
        /// <param name="position">The position in latitude and longitude in decimal degrees, and height in meters.</param>
        protected LtpEnuSpecification(string id, string authority, TangentPointPosition position)
            : base(id, authority)
        {
            this.Position = position;
        }

        /// <summary>
        /// Gets the position in latitude and longitude in decimal degrees, and height in meters.
        /// </summary>
        [JsonProperty("position")]
        public TangentPointPosition Position { get; }

        /// <inheritdoc/>
        public bool Equals(LtpEnuSpecification other)
        {
            return this.Position == other.Position
                   && base.Equals(other);
        }

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            return obj is LtpEnuSpecification equatable && this.Equals(equatable);
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
