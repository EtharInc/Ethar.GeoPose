// <copyright file="Vec2d.cs" company="Ethar">
// Copyright (c) Ethar. All rights reserved.
// </copyright>

/* Ported from Uber H3 v4.5.0, src/h3lib/lib/vec2d.c and src/h3lib/include/vec2d.h. Copyright 2016-2017 Uber Technologies, Inc. Licensed under the Apache License, Version 2.0. */

namespace Ethar.GeoPose.H3
{
    using System;
    using Ethar.GeoPose.JsonConversion;

    /// <summary>
    /// 2D floating point vector. Ported from the C struct <c>Vec2d</c> and the functions of vec2d.c.
    /// </summary>
    internal struct Vec2d
    {
        /// <summary>
        /// The C <c>FLT_EPSILON</c>, 2 to the power -23. Not the same as <see cref="float.Epsilon"/>, which is the smallest positive float.
        /// </summary>
        private const double FltEpsilon = 1.1920928955078125e-07;

        /// <summary>
        /// Initializes a new instance of the <see cref="Vec2d"/> struct.
        /// </summary>
        /// <param name="x">The x component.</param>
        /// <param name="y">The y component.</param>
        public Vec2d(double x, double y)
        {
            this.X = x;
            this.Y = y;
        }

        /// <summary>
        /// Gets or sets the x component.
        /// </summary>
        public double X { get; set; }

        /// <summary>
        /// Gets or sets the y component.
        /// </summary>
        public double Y { get; set; }

        /// <summary>
        /// Calculates the magnitude of a 2D cartesian vector. C name <c>_v2dMag</c>.
        /// </summary>
        /// <param name="v">The 2D cartesian vector.</param>
        /// <returns>The magnitude of the vector.</returns>
        public static double V2dMag(Vec2d v)
        {
            return Math.Sqrt((v.X * v.X) + (v.Y * v.Y));
        }

        /// <summary>
        /// Finds the intersection between two lines. Assumes that the lines intersect and that the intersection is not at an endpoint of either line. C name <c>_v2dIntersect</c>.
        /// </summary>
        /// <param name="p0">The first endpoint of the first line.</param>
        /// <param name="p1">The second endpoint of the first line.</param>
        /// <param name="p2">The first endpoint of the second line.</param>
        /// <param name="p3">The second endpoint of the second line.</param>
        /// <returns>The intersection point.</returns>
        public static Vec2d V2dIntersect(Vec2d p0, Vec2d p1, Vec2d p2, Vec2d p3)
        {
            var s1 = new Vec2d(p1.X - p0.X, p1.Y - p0.Y);
            var s2 = new Vec2d(p3.X - p2.X, p3.Y - p2.Y);

            var t = ((s2.X * (p0.Y - p2.Y)) - (s2.Y * (p0.X - p2.X))) / ((-s2.X * s1.Y) + (s1.X * s2.Y));

            return new Vec2d(p0.X + (t * s1.X), p0.Y + (t * s1.Y));
        }

        /// <summary>
        /// Whether two 2D vectors are almost equal, within the C float epsilon. C name <c>_v2dAlmostEquals</c>.
        /// </summary>
        /// <param name="v1">First vector to compare.</param>
        /// <param name="v2">Second vector to compare.</param>
        /// <returns>Whether the vectors are almost equal.</returns>
        public static bool V2dAlmostEquals(Vec2d v1, Vec2d v2)
        {
            return Math.Abs(v1.X - v2.X) < FltEpsilon && Math.Abs(v1.Y - v2.Y) < FltEpsilon;
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return "(" + InvariantNumber.Format(this.X) + ", " + InvariantNumber.Format(this.Y) + ")";
        }
    }
}
