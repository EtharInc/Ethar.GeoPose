// <copyright file="AuthorityNotSupportedException.cs" company="Ethar">
// Copyright (c) Ethar. All rights reserved.
// </copyright>

namespace Ethar.GeoPose.Exceptions
{
    using System;
    using Ethar.GeoPose.Authority;

    /// <summary>
    /// Exception that is thrown when a user attempts to get information about an <see cref="IAuthority"/> that is not supported.
    /// </summary>
    public class AuthorityNotSupportedException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AuthorityNotSupportedException"/> class.
        /// </summary>
        /// <param name="message">The error message.</param>
        public AuthorityNotSupportedException(string message)
            : base(message)
        {
        }
    }
}
