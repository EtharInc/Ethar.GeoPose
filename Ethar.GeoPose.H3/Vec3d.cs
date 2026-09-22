// <copyright file="Vec3d.cs" company="Ethar">
// Copyright (c) Ethar. All rights reserved.
// </copyright>

/* Ported from Uber H3 v4.5.0, src/h3lib/include/vec3d.h. Copyright 2018, 2020-2021, 2026 Uber Technologies, Inc. Licensed under the Apache License, Version 2.0. */

namespace Ethar.GeoPose.H3
{
    using System;
    using Ethar.GeoPose.JsonConversion;

    /// <summary>
    /// 3D floating point vector. For geodesic calculations it represents a point on the surface of the Earth as a unit vector in 3D Cartesian space. Ported from the C struct <c>Vec3d</c> and the inline functions of vec3d.h.
    /// </summary>
    /// <remarks>
    /// The struct is mutable so that the ported maths can update it in place, as the C code does through a pointer.
    /// </remarks>
    internal struct Vec3d
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Vec3d"/> struct.
        /// </summary>
        /// <param name="x">The x component, towards 0 degrees latitude, 0 degrees longitude.</param>
        /// <param name="y">The y component, towards 0 degrees latitude, 90 degrees longitude.</param>
        /// <param name="z">The z component, towards the north pole.</param>
        public Vec3d(double x, double y, double z)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
        }

        /// <summary>
        /// Gets or sets the x component, towards 0 degrees latitude, 0 degrees longitude.
        /// </summary>
        public double X { get; set; }

        /// <summary>
        /// Gets or sets the y component, towards 0 degrees latitude, 90 degrees longitude.
        /// </summary>
        public double Y { get; set; }

        /// <summary>
        /// Gets or sets the z component, towards the north pole.
        /// </summary>
        public double Z { get; set; }

        /// <summary>
        /// Convert latitude and longitude to a unit vector on the sphere. C name <c>latLngToVec3</c>.
        /// </summary>
        /// <param name="geo">The spherical coordinates in radians.</param>
        /// <returns>The unit vector.</returns>
        public static Vec3d LatLngToVec3(LatLng geo)
        {
            var r = Math.Cos(geo.Lat);
            return new Vec3d(Math.Cos(geo.Lng) * r, Math.Sin(geo.Lng) * r, Math.Sin(geo.Lat));
        }

        /// <summary>
        /// Convert a unit vector on the sphere to latitude and longitude. C name <c>vec3ToLatLng</c>.
        /// </summary>
        /// <param name="v">The unit vector.</param>
        /// <returns>The spherical coordinates in radians.</returns>
        public static LatLng Vec3ToLatLng(Vec3d v)
        {
            return new LatLng(Math.Asin(v.Z), Math.Atan2(v.Y, v.X));
        }

        /// <summary>
        /// Linear combination a * v1 + b * v2. C name <c>vec3LinComb</c>.
        /// </summary>
        /// <param name="a">The first scalar.</param>
        /// <param name="v1">The first vector.</param>
        /// <param name="b">The second scalar.</param>
        /// <param name="v2">The second vector.</param>
        /// <returns>The combination.</returns>
        public static Vec3d Vec3LinComb(double a, Vec3d v1, double b, Vec3d v2)
        {
            return new Vec3d((a * v1.X) + (b * v2.X), (a * v1.Y) + (b * v2.Y), (a * v1.Z) + (b * v2.Z));
        }

        /// <summary>
        /// Cross product. C name <c>vec3Cross</c>.
        /// </summary>
        /// <param name="v1">The first vector.</param>
        /// <param name="v2">The second vector.</param>
        /// <returns>The cross product v1 x v2.</returns>
        public static Vec3d Vec3Cross(Vec3d v1, Vec3d v2)
        {
            return new Vec3d((v1.Y * v2.Z) - (v1.Z * v2.Y), (v1.Z * v2.X) - (v1.X * v2.Z), (v1.X * v2.Y) - (v1.Y * v2.X));
        }

        /// <summary>
        /// Dot product. C name <c>vec3Dot</c>.
        /// </summary>
        /// <param name="v1">The first vector.</param>
        /// <param name="v2">The second vector.</param>
        /// <returns>The dot product.</returns>
        public static double Vec3Dot(Vec3d v1, Vec3d v2)
        {
            return (v1.X * v2.X) + (v1.Y * v2.Y) + (v1.Z * v2.Z);
        }

        /// <summary>
        /// Squared norm. C name <c>vec3NormSq</c>.
        /// </summary>
        /// <param name="v">The vector.</param>
        /// <returns>The squared length.</returns>
        public static double Vec3NormSq(Vec3d v)
        {
            return Vec3Dot(v, v);
        }

        /// <summary>
        /// Norm. C name <c>vec3Norm</c>.
        /// </summary>
        /// <param name="v">The vector.</param>
        /// <returns>The length.</returns>
        public static double Vec3Norm(Vec3d v)
        {
            return Math.Sqrt(Vec3NormSq(v));
        }

        /// <summary>
        /// Normalizes the vector in place. A zero vector, or one whose squared norm underflows to zero, becomes exactly zero. C name <c>vec3Normalize</c>.
        /// </summary>
        /// <param name="v">The vector to normalize.</param>
        public static void Vec3Normalize(ref Vec3d v)
        {
            var norm = Vec3Norm(v);

            // Norm can be zero either from true zero vector, or from squaring underflowing to zero.
            // If the norm is nonzero, we normalize v using it. If the norm is zero, we set the vector to be exactly zero.
            var s = 0.0;
            if (norm > 0.0)
            {
                s = 1.0 / norm;
            }

            v.X *= s;
            v.Y *= s;
            v.Z *= s;
        }

        /// <summary>
        /// Squared euclidean distance between two vectors. C name <c>vec3DistSq</c>.
        /// </summary>
        /// <param name="v1">The first vector.</param>
        /// <param name="v2">The second vector.</param>
        /// <returns>The squared distance.</returns>
        public static double Vec3DistSq(Vec3d v1, Vec3d v2)
        {
            var d = Vec3LinComb(1.0, v1, -1.0, v2);
            return Vec3NormSq(d);
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return "(" + InvariantNumber.Format(this.X) + ", " + InvariantNumber.Format(this.Y) + ", " + InvariantNumber.Format(this.Z) + ")";
        }
    }
}
