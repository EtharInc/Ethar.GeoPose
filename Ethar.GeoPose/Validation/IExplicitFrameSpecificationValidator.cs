// <copyright file="IExplicitFrameSpecificationValidator.cs" company="Ethar">
// Copyright (c) Ethar. All rights reserved.
// </copyright>

namespace Ethar.GeoPose.Validation
{
    using Ethar.GeoPose.Interfaces;

    /// <summary>
    /// Interface that represents an <see cref="IFrameSpecification"/> validator.
    /// </summary>
    public interface IExplicitFrameSpecificationValidator
    {
        /// <summary>
        /// Determines if the <see cref="IFrameSpecification"/> is valid.
        /// </summary>
        /// <param name="frame">The <see cref="IFrameSpecification"/> to validate.</param>
        /// <returns>A <see cref="FrameSpecificationValidationResult"/> with details on the validity of the <see cref="IFrameSpecification"/>.</returns>
        FrameSpecificationValidationResult Validate(IFrameSpecification frame);
    }
}