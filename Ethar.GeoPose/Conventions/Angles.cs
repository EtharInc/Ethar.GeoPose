// <copyright file="Angles.cs" company="Ethar">
// Copyright (c) Ethar. All rights reserved.
// </copyright>

namespace Ethar.GeoPose.Conventions
{
    using System;

    /// <summary>
    /// Angle unit conversion and normalisation helpers. GeoPose angles are decimal degrees.
    /// </summary>
    public static class Angles
    {
        private const double DegreesPerRadian = 180.0 / Math.PI;

        /// <summary>
        /// Converts degrees to radians.
        /// </summary>
        /// <param name="degrees">The angle in degrees.</param>
        /// <returns>The angle in radians.</returns>
        public static double DegreesToRadians(double degrees) => degrees / DegreesPerRadian;

        /// <summary>
        /// Converts radians to degrees.
        /// </summary>
        /// <param name="radians">The angle in radians.</param>
        /// <returns>The angle in degrees.</returns>
        public static double RadiansToDegrees(double radians) => radians * DegreesPerRadian;

        /// <summary>
        /// Normalises an angle to the range [0, 360).
        /// </summary>
        /// <param name="degrees">The angle in degrees.</param>
        /// <returns>The equivalent angle in the range [0, 360).</returns>
        public static double Normalize360(double degrees)
        {
            var result = degrees % 360.0;
            if (result < 0.0)
            {
                result += 360.0;
            }

            // Guard against -0.0 and rounding that lands exactly on 360.
            return result >= 360.0 ? 0.0 : Math.Abs(result);
        }

        /// <summary>
        /// Normalises an angle to the range [-180, 180).
        /// </summary>
        /// <param name="degrees">The angle in degrees.</param>
        /// <returns>The equivalent angle in the range [-180, 180).</returns>
        public static double Normalize180(double degrees)
        {
            return Normalize360(degrees + 180.0) - 180.0;
        }
    }
}
