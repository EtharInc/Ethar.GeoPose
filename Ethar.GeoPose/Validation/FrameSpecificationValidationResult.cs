// <copyright file="FrameSpecificationValidationResult.cs" company="Ethar">
// Copyright (c) Ethar. All rights reserved.
// </copyright>

namespace Ethar.GeoPose.Validation
{
    using Ethar.GeoPose.Interfaces;

    /// <summary>
    /// Struct that contains data pertaining to the validity of an <see cref="IFrameSpecification"/>.
    /// </summary>
    public struct FrameSpecificationValidationResult
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FrameSpecificationValidationResult"/> struct.
        /// </summary>
        /// <param name="isValid">Whether the associated <see cref="IFrameSpecification"/> is valid.</param>
        /// <param name="message">The error message, if any.</param>
        public FrameSpecificationValidationResult(bool isValid, string message)
        {
            this.IsValid = isValid;
            this.Message = message;
        }

        /// <summary>
        /// Gets a <see cref="FrameSpecificationValidationResult"/> that represents a valid <see cref="IFrameSpecification"/>.
        /// </summary>
        public static FrameSpecificationValidationResult Valid => new FrameSpecificationValidationResult(true, string.Empty);

        /// <summary>
        /// Gets a value indicating whether the associated <see cref="IFrameSpecification"/> is valid.
        /// </summary>
        public bool IsValid { get; }

        /// <summary>
        /// Gets the error message, if any.
        /// </summary>
        public string Message { get; }
    }
}