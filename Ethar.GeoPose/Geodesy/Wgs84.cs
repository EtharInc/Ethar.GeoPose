// <copyright file="Wgs84.cs" company="Ethar">
// Copyright (c) Ethar. All rights reserved.
// </copyright>

namespace Ethar.GeoPose.Geodesy
{
    using System;

    /// <summary>
    /// Defining constants of the WGS 84 reference ellipsoid (EPSG:4979 when used as a three dimensional geographic CRS).
    /// </summary>
    /// <remarks>
    /// The GeoPose Basic targets use WGS 84 as their outer frame, and <see cref="Ethar.GeoPose.DataTypes.TangentPointPosition.HeightInMeters"/>
    /// is a height above this ellipsoid (OGC GeoPose 1.0, requirement /req/tangent-point/height). It is not a height above sea level.
    /// </remarks>
    public static class Wgs84
    {
        /// <summary>
        /// Semi-major axis (equatorial radius) in meters.
        /// </summary>
        public const double SemiMajorAxis = 6378137.0;

        /// <summary>
        /// Inverse flattening 1/f.
        /// </summary>
        public const double InverseFlattening = 298.257223563;

        /// <summary>
        /// Flattening f = (a - b) / a.
        /// </summary>
        public const double Flattening = 1.0 / InverseFlattening;

        /// <summary>
        /// Semi-minor axis (polar radius) in meters, b = a (1 - f).
        /// </summary>
        public const double SemiMinorAxis = SemiMajorAxis * (1.0 - Flattening);

        /// <summary>
        /// First eccentricity squared, e² = f (2 - f).
        /// </summary>
        public const double EccentricitySquared = Flattening * (2.0 - Flattening);

        /// <summary>
        /// Mean radius in meters, (2a + b) / 3, used for great circle distances.
        /// </summary>
        public const double MeanRadius = ((2.0 * SemiMajorAxis) + SemiMinorAxis) / 3.0;

        /// <summary>
        /// Radius of curvature in the prime vertical, N(φ) = a / sqrt(1 - e² sin²φ).
        /// </summary>
        /// <param name="latitudeRadians">Geodetic latitude in radians.</param>
        /// <returns>The prime vertical radius of curvature in meters.</returns>
        public static double PrimeVerticalRadius(double latitudeRadians)
        {
            var sinLat = Math.Sin(latitudeRadians);
            return SemiMajorAxis / Math.Sqrt(1.0 - (EccentricitySquared * sinLat * sinLat));
        }
    }
}
