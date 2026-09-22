// <copyright file="H3Exception.cs" company="Ethar">
// Copyright (c) Ethar. All rights reserved.
// </copyright>

/* Ported from Uber H3 v4.5.0, src/h3lib/lib/h3Index.c (describeH3Error). Copyright 2016-2021, 2024, 2026 Uber Technologies, Inc. Licensed under the Apache License, Version 2.0. */

namespace Ethar.GeoPose.H3
{
    using System;

    /// <summary>
    /// Exception thrown when an H3 operation fails. Carries the H3 error code so callers can mirror the C library's behaviour.
    /// </summary>
    public class H3Exception : Exception
    {
        /// <summary>
        /// Descriptions of each error code, in enum order. C name <c>H3ErrorDescriptions</c>.
        /// </summary>
        private static readonly string[] Descriptions =
        {
            "Success",
            "The operation failed but a more specific error is not available",
            "Argument was outside of acceptable range",
            "Latitude or longitude arguments were outside of acceptable range",
            "Resolution argument was outside of acceptable range",
            "Cell argument was not valid",
            "Directed edge argument was not valid",
            "Undirected edge argument was not valid",
            "Vertex argument was not valid",
            "Pentagon distortion was encountered",
            "Duplicate input",
            "Cell arguments were not neighbors",
            "Cell arguments had incompatible resolutions",
            "Memory allocation failed",
            "Bounds of provided memory were insufficient",
            "Mode or flags argument was not valid",
            "Index argument was not valid",
            "Base cell number was outside of acceptable range",
            "Child digits invalid",
            "Deleted subsequence indicates invalid index",
        };

        /// <summary>
        /// Initializes a new instance of the <see cref="H3Exception"/> class with the standard description of the error code.
        /// </summary>
        /// <param name="errorCode">The H3 error code.</param>
        public H3Exception(H3ErrorCode errorCode)
            : this(errorCode, Describe(errorCode))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="H3Exception"/> class.
        /// </summary>
        /// <param name="errorCode">The H3 error code.</param>
        /// <param name="message">The error message.</param>
        public H3Exception(H3ErrorCode errorCode, string message)
            : base(message)
        {
            this.ErrorCode = errorCode;
        }

        /// <summary>
        /// Gets the H3 error code.
        /// </summary>
        public H3ErrorCode ErrorCode { get; }

        /// <summary>
        /// Returns the string describing the error code. C name <c>describeH3Error</c>.
        /// </summary>
        /// <param name="errorCode">The H3 error code.</param>
        /// <returns>The description, or "Invalid error code" for a value outside the enum.</returns>
        public static string Describe(H3ErrorCode errorCode)
        {
            var index = (int)errorCode;
            if (index >= 0 && index < Descriptions.Length)
            {
                return Descriptions[index];
            }

            return "Invalid error code";
        }
    }
}
