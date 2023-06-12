// <copyright file="EtharFrameSpecificationTypes.cs" company="Ethar">
// Copyright (c) Ethar. All rights reserved.
// </copyright>

namespace Ethar.GeoPose.Authority.FrameSpecifications
{
    /// <summary>
    /// A class that contains the names of the frame specification types included in the /Ethar.GeoPose/1.0 authority.
    /// </summary>
    public static class EtharFrameSpecificationTypes
    {
        /// <summary>
        /// Identifier for an <see cref="LtpEnuSpecification"/>.
        /// </summary>
        public const string LtpEnuSpec = "LTP-ENU";

        /// <summary>
        /// Identifier for an <see cref="LtpNedSpecification"/>.
        /// </summary>
        public const string LtpNedSpec = "LTP-NED";

        /// <summary>
        /// Identifier for a <see cref="YawPitchRollOrientedLtpEnuSpecification"/>.
        /// </summary>
        public const string YprOrientedLtpEnuSpec = "YPR-LTP-ENU";

        /// <summary>
        /// Identifier for a <see cref="QuaternionOrientedLtpEnuSpecification"/>.
        /// </summary>
        public const string QuaternionOrientedLtpEnuSpec = "Quaternion-LTP-ENU";

        /// <summary>
        /// Identifier for a <see cref="TranslateRotateSpecification"/>.
        /// </summary>
        public const string TranslateRotateSpec = "Translate-Rotate";
    }
}
