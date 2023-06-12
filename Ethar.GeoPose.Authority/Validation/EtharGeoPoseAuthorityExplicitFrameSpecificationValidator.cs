// <copyright file="EtharGeoPoseAuthorityExplicitFrameSpecificationValidator.cs" company="Ethar">
// Copyright (c) Ethar. All rights reserved.
// </copyright>

namespace Ethar.GeoPose.Authority.Validation
{
    using Ethar.GeoPose.Authority.FrameSpecifications;
    using Ethar.GeoPose.Extensions;
    using Ethar.GeoPose.Interfaces;
    using Ethar.GeoPose.Validation;

    /// <summary>
    /// A utility class that validates frame specifications from the /Ethar.GeoPose/1.0 authority.
    /// </summary>
    public class EtharGeoPoseAuthorityExplicitFrameSpecificationValidator : IExplicitFrameSpecificationValidator
    {
        /// <inheritdoc/>
        public FrameSpecificationValidationResult Validate(IFrameSpecification frame)
        {
            switch (frame)
            {
                case LtpNedSpecification ltpNedSpec:
                    return ltpNedSpec.Position.Validate();
                case LtpEnuSpecification ltpEnuSpec:
                    return ltpEnuSpec.Position.Validate();
                default:
                    return FrameSpecificationValidationResult.Valid;
            }
        }
    }
}