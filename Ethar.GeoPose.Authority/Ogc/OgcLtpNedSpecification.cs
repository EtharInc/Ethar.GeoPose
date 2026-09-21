// <copyright file="OgcLtpNedSpecification.cs" company="Ethar">
// Copyright (c) Ethar. All rights reserved.
// </copyright>

namespace Ethar.GeoPose.Authority.Ogc
{
    using Ethar.GeoPose.Authority.FrameSpecifications;
    using Ethar.GeoPose.DataTypes;

    /// <summary>
    /// An LTP-NED frame specification under the <c>/geopose/1.0</c> authority.
    /// </summary>
    public class OgcLtpNedSpecification : LtpNedSpecification
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OgcLtpNedSpecification"/> class.
        /// </summary>
        /// <param name="position">The tangent point.</param>
        public OgcLtpNedSpecification(TangentPointPosition position)
            : base(OgcFrameSpecificationTypes.LtpNed, OgcConstants.AuthorityName, position)
        {
        }
    }
}
