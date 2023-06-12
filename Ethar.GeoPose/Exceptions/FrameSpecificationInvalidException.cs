// <copyright file="FrameSpecificationInvalidException.cs" company="Ethar">
// Copyright (c) Ethar. All rights reserved.
// </copyright>

namespace Ethar.GeoPose.Exceptions
{
    using System;
    using Ethar.GeoPose.Interfaces;

    /// <summary>
    /// Exception thrown when a <see cref="IFrameSpecification"/> has invalid parameters.
    /// </summary>
    public class FrameSpecificationInvalidException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FrameSpecificationInvalidException"/> class.
        /// </summary>
        /// <param name="message">The error message.</param>
        public FrameSpecificationInvalidException(string message)
            : base(message)
        {
        }
    }
}
