// <copyright file="TranslateRotateSpecification.cs" company="Ethar">
// Copyright (c) Ethar. All rights reserved.
// </copyright>

namespace Ethar.GeoPose.Authority.FrameSpecifications
{
    using System;
    using Ethar.GeoPose.DataTypes;
    using Ethar.GeoPose.FrameSpecifications;
    using Newtonsoft.Json;

    /// <summary>
    /// A frame specification that specifies a translation and rotation relative to another frame specification.
    /// This type of frame specification cannot be the outermost frame in a chain or graph.
    ///
    /// Derived from Figure 8 from section 7.2.1 of version 1.0.0 of the GeoPose spec http://www.opengis.net/doc/DIS/geopose/1.0.
    /// </summary>
    public class TranslateRotateSpecification : BaseFrameSpecification, IEquatable<TranslateRotateSpecification>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TranslateRotateSpecification"/> class.
        /// </summary>
        public TranslateRotateSpecification()
            : base(EtharFrameSpecificationTypes.TranslateRotateSpec, Constants.AuthorityName)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TranslateRotateSpecification"/> class.
        /// </summary>
        /// <param name="translation">The translation.</param>
        /// <param name="rotation">The rotation.</param>
        public TranslateRotateSpecification(UnitVector3 translation, UnitQuaternion rotation)
            : base(EtharFrameSpecificationTypes.TranslateRotateSpec, Constants.AuthorityName)
        {
            this.Translation = translation;
            this.Rotation = rotation;
        }

        /// <summary>
        /// Gets the translation.
        /// </summary>
        [JsonProperty("translation")]
        public UnitVector3 Translation { get; }

        /// <summary>
        /// Gets the rotation.
        /// </summary>
        [JsonProperty("rotation")]
        public UnitQuaternion Rotation { get; }

        /// <inheritdoc/>
        public bool Equals(TranslateRotateSpecification other)
        {
            return this.Translation == other.Translation
                   && this.Rotation == other.Rotation
                   && base.Equals(other);
        }

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            return obj is TranslateRotateSpecification equatable && this.Equals(equatable);
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            unchecked
            {
                var hashcode = base.GetHashCode();
                hashcode = (hashcode * 397) ^ this.Translation.GetHashCode();
                hashcode = (hashcode * 397) ^ this.Rotation.GetHashCode();
                return hashcode;
            }
        }
    }
}
