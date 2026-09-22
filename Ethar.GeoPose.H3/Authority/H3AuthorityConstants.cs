// <copyright file="H3AuthorityConstants.cs" company="Ethar">
// Copyright (c) Ethar. All rights reserved.
// </copyright>

namespace Ethar.GeoPose.H3.Authority
{
    /// <summary>
    /// Names used by the H3 frame specification authority.
    /// </summary>
    public static class H3AuthorityConstants
    {
        /// <summary>
        /// The authority name written into the <c>authority</c> field of a frame specification.
        /// </summary>
        public const string AuthorityName = "/Ethar.GeoPose.H3/1.0";

        /// <summary>
        /// The frame specification id of a frame anchored at the centre of an H3 cell.
        /// </summary>
        public const string H3CellSpecificationId = "H3-CELL";

        /// <summary>
        /// The parameter that carries the 15 character hexadecimal cell index.
        /// </summary>
        public const string CellParameter = "cell";

        /// <summary>
        /// The parameter that carries the height of the frame origin above the WGS84 ellipsoid in metres.
        /// </summary>
        public const string HeightParameter = "heightInMeters";
    }
}
