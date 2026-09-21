// <copyright file="OgcLtpEnuSpecification.cs" company="Ethar">
// Copyright (c) Ethar. All rights reserved.
// </copyright>

namespace Ethar.GeoPose.Authority.Ogc
{
    using Ethar.GeoPose.Authority.FrameSpecifications;
    using Ethar.GeoPose.DataTypes;

    /// <summary>
    /// An LTP-ENU frame specification under the <c>/geopose/1.0</c> authority. The id is preserved as received
    /// (<c>LTP-ENU</c> or <c>/Extrinsic/LTP-ENU</c>) so a payload round-trips unchanged.
    /// </summary>
    public class OgcLtpEnuSpecification : LtpEnuSpecification
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OgcLtpEnuSpecification"/> class.
        /// </summary>
        /// <param name="position">The tangent point.</param>
        /// <param name="id">The frame id, <see cref="OgcFrameSpecificationTypes.LtpEnu"/> by default.</param>
        public OgcLtpEnuSpecification(TangentPointPosition position, string id = OgcFrameSpecificationTypes.LtpEnu)
            : base(id, OgcConstants.AuthorityName, position)
        {
        }
    }
}
