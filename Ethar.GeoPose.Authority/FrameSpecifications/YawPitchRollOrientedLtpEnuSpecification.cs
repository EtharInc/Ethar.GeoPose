// <copyright file="YawPitchRollOrientedLtpEnuSpecification.cs" company="Ethar">
// Copyright (c) Ethar. All rights reserved.
// </copyright>

namespace Ethar.GeoPose.Authority.FrameSpecifications
{
    using System;
    using Ethar.GeoPose.DataTypes;
    using Newtonsoft.Json;

    /// <summary>
    /// A frame specification with a local tangent plane coordinate system specialized to an east-north-up system, where the X axis is aligned toward east and the Y axis toward north.
    /// Also contains a yaw, pitch, and roll describing an orientation.
    ///
    /// Derived from Figure 8 from section 7.2.1 of version 1.0.0 of the GeoPose spec http://www.opengis.net/doc/DIS/geopose/1.0.
    /// </summary>
    public class YawPitchRollOrientedLtpEnuSpecification : LtpEnuSpecification, IEquatable<YawPitchRollOrientedLtpEnuSpecification>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="YawPitchRollOrientedLtpEnuSpecification"/> class.
        /// </summary>
        public YawPitchRollOrientedLtpEnuSpecification()
            : base(EtharFrameSpecificationTypes.YprOrientedLtpEnuSpec, Constants.AuthorityName, default)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="YawPitchRollOrientedLtpEnuSpecification"/> class.
        /// </summary>
        /// <param name="position">The position in latitude, longitude, and height in meters.</param>
        /// <param name="orientation">Yaw, pitch, and roll representing an orientation.</param>
        public YawPitchRollOrientedLtpEnuSpecification(TangentPointPosition position, YawPitchRollAngles orientation)
            : base(EtharFrameSpecificationTypes.YprOrientedLtpEnuSpec, Constants.AuthorityName, position)
        {
            this.Orientation = orientation;
        }

        /// <summary>
        /// Gets the orientation.
        /// </summary>
        [JsonProperty("orientation")]
        public YawPitchRollAngles Orientation { get; }

        /// <inheritdoc/>
        public bool Equals(YawPitchRollOrientedLtpEnuSpecification other)
        {
            return this.Orientation == other.Orientation
                   && base.Equals(other);
        }

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            return obj is YawPitchRollOrientedLtpEnuSpecification equatable && this.Equals(equatable);
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            unchecked
            {
                var hashcode = base.GetHashCode();
                hashcode = (hashcode * 397) ^ this.Orientation.GetHashCode();
                return hashcode;
            }
        }
    }
}
