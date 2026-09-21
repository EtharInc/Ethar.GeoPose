// <copyright file="OgcTranslateRotateSpecification.cs" company="Ethar">
// Copyright (c) Ethar. All rights reserved.
// </copyright>

namespace Ethar.GeoPose.Authority.Ogc
{
    using Ethar.GeoPose.Authority.FrameSpecifications;
    using Ethar.GeoPose.DataTypes;

    /// <summary>
    /// A translate-rotate frame specification under the <c>/geopose/1.0</c> authority. The id is preserved as received
    /// (<c>/Intrinsic/Translate-Rotate</c> or <c>RotateTranslate</c>) so a payload round-trips unchanged.
    /// </summary>
    public class OgcTranslateRotateSpecification : TranslateRotateSpecification
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OgcTranslateRotateSpecification"/> class.
        /// </summary>
        /// <param name="translation">The translation.</param>
        /// <param name="rotation">The rotation.</param>
        /// <param name="id">The frame id, <see cref="OgcFrameSpecificationTypes.IntrinsicTranslateRotate"/> by default.</param>
        public OgcTranslateRotateSpecification(UnitVector3 translation, UnitQuaternion rotation, string id = OgcFrameSpecificationTypes.IntrinsicTranslateRotate)
            : base(id, OgcConstants.AuthorityName, translation, rotation)
        {
        }
    }
}
