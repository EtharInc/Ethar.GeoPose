// <copyright file="TangentPointPositionGeodesyExtensions.cs" company="Ethar">
// Copyright (c) Ethar. All rights reserved.
// </copyright>

namespace Ethar.GeoPose.Extensions
{
    using Ethar.GeoPose.DataTypes;
    using Ethar.GeoPose.Geodesy;

    /// <summary>
    /// Geodesy conveniences on <see cref="TangentPointPosition"/>; thin wrappers over <see cref="GeodeticConverter"/>.
    /// </summary>
    public static class TangentPointPositionGeodesyExtensions
    {
        /// <summary>
        /// Converts the position to ECEF.
        /// </summary>
        /// <param name="position">The geodetic position.</param>
        /// <returns>The ECEF position in meters.</returns>
        public static EcefPosition ToEcef(this TangentPointPosition position) => GeodeticConverter.GeodeticToEcef(position);

        /// <summary>
        /// Expresses this position as an East, North, Up displacement from an origin.
        /// </summary>
        /// <param name="position">The geodetic position.</param>
        /// <param name="origin">The tangent point that is the origin of the ENU frame.</param>
        /// <returns>The displacement in meters.</returns>
        public static EnuVector EnuOffsetFrom(this TangentPointPosition position, TangentPointPosition origin) => GeodeticConverter.GeodeticToEnu(position, origin);

        /// <summary>
        /// Returns the geodetic position reached by moving this position by an East, North, Up displacement.
        /// </summary>
        /// <param name="origin">The starting position, which is the tangent point of the displacement.</param>
        /// <param name="displacement">The displacement in meters.</param>
        /// <returns>The displaced geodetic position.</returns>
        public static TangentPointPosition OffsetBy(this TangentPointPosition origin, EnuVector displacement) => GeodeticConverter.EnuToGeodetic(displacement, origin);

        /// <summary>
        /// Great circle distance to another position, ignoring height.
        /// </summary>
        /// <param name="from">This position.</param>
        /// <param name="to">The other position.</param>
        /// <returns>The distance in meters.</returns>
        public static double DistanceTo(this TangentPointPosition from, TangentPointPosition to) => GeodeticConverter.GreatCircleDistance(from, to);

        /// <summary>
        /// Initial compass bearing to another position.
        /// </summary>
        /// <param name="from">This position.</param>
        /// <param name="to">The other position.</param>
        /// <returns>The bearing in degrees, clockwise from true North, in the range [0, 360).</returns>
        public static double BearingTo(this TangentPointPosition from, TangentPointPosition to) => GeodeticConverter.InitialBearing(from, to);
    }
}
