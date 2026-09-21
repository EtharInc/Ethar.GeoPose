// <copyright file="OgcFrameSpecificationTypes.cs" company="Ethar">
// Copyright (c) Ethar. All rights reserved.
// </copyright>

namespace Ethar.GeoPose.Authority.Ogc
{
    /// <summary>
    /// Frame specification ids used by the OGC GeoPose 1.0 example instances.
    /// </summary>
    public static class OgcFrameSpecificationTypes
    {
        /// <summary>
        /// Local tangent plane East-North-Up, the extrinsic frame used as the outer frame of the Advanced, Series and Stream instances.
        /// Parameters: <c>longitude=…&amp;latitude=…&amp;height=…</c>.
        /// </summary>
        public const string LtpEnu = "LTP-ENU";

        /// <summary>
        /// Path form of <see cref="LtpEnu"/> used by the Chain and Graph instances. Same parameters.
        /// </summary>
        public const string ExtrinsicLtpEnu = "/Extrinsic/LTP-ENU";

        /// <summary>
        /// Local tangent plane North-East-Down, named in the OGC reviewers guide. Same parameters as <see cref="LtpEnu"/>.
        /// </summary>
        public const string LtpNed = "LTP-NED";

        /// <summary>
        /// Intrinsic translate-then-rotate frame used by the Chain and Graph instances.
        /// Parameters: <c>translation=[x, y, z]&amp;rotation=[w, x, y, z]</c>.
        /// </summary>
        public const string IntrinsicTranslateRotate = "/Intrinsic/Translate-Rotate";

        /// <summary>
        /// Intrinsic frame used by the Series and Stream instances. Same parameters as <see cref="IntrinsicTranslateRotate"/>.
        /// </summary>
        public const string RotateTranslate = "RotateTranslate";
    }
}
