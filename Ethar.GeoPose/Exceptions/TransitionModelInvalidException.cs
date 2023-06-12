// <copyright file="TransitionModelInvalidException.cs" company="Ethar">
// Copyright (c) Ethar. All rights reserved.
// </copyright>

namespace Ethar.GeoPose.Exceptions
{
    using System;
    using Ethar.GeoPose.TransitionModels;

    /// <summary>
    /// Exception thrown when a <see cref="TransitionModel"/> has invalid parameters.
    /// </summary>
    public class TransitionModelInvalidException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TransitionModelInvalidException"/> class.
        /// </summary>
        /// <param name="message">The error message.</param>
        public TransitionModelInvalidException(string message)
            : base(message)
        {
        }
    }
}
