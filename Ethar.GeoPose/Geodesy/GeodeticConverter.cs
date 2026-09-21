// <copyright file="GeodeticConverter.cs" company="Ethar">
// Copyright (c) Ethar. All rights reserved.
// </copyright>

namespace Ethar.GeoPose.Geodesy
{
    using System;
    using Ethar.GeoPose.Conventions;
    using Ethar.GeoPose.DataTypes;

    /// <summary>
    /// Double precision conversions between WGS 84 geodetic positions, ECEF positions and Local Tangent Plane ENU displacements.
    /// </summary>
    /// <remarks>
    /// <para>
    /// All angles are decimal degrees and all lengths are meters, matching the GeoPose data types. Every calculation is carried out in
    /// double precision; callers that need single precision (for example a game engine vector) should convert only the final local
    /// displacement, never the geodetic or ECEF values. ECEF coordinates are of the order of 6.4 million meters and lose about half a
    /// meter of resolution in single precision.
    /// </para>
    /// <para>
    /// Heights are ellipsoidal (above WGS 84), as required by the GeoPose standard. Device altitudes reported above mean sea level must be
    /// corrected by the local geoid separation before use.
    /// </para>
    /// </remarks>
    public static class GeodeticConverter
    {
        private const int MaxGeodeticIterations = 12;
        private const double GeodeticConvergenceRadians = 1e-13;

        /// <summary>
        /// Converts a geodetic position to ECEF.
        /// </summary>
        /// <param name="position">The geodetic position (WGS 84, ellipsoidal height).</param>
        /// <returns>The ECEF position in meters.</returns>
        public static EcefPosition GeodeticToEcef(TangentPointPosition position)
        {
            var lat = Angles.DegreesToRadians(position.Latitude);
            var lon = Angles.DegreesToRadians(position.Longitude);
            var sinLat = Math.Sin(lat);
            var cosLat = Math.Cos(lat);
            var sinLon = Math.Sin(lon);
            var cosLon = Math.Cos(lon);
            var n = Wgs84.PrimeVerticalRadius(lat);
            var h = position.HeightInMeters;

            return new EcefPosition(
                (n + h) * cosLat * cosLon,
                (n + h) * cosLat * sinLon,
                (((1.0 - Wgs84.EccentricitySquared) * n) + h) * sinLat);
        }

        /// <summary>
        /// Converts an ECEF position to a geodetic position.
        /// </summary>
        /// <param name="position">The ECEF position in meters.</param>
        /// <returns>The geodetic position (WGS 84, ellipsoidal height).</returns>
        public static TangentPointPosition EcefToGeodetic(EcefPosition position)
        {
            var p = Math.Sqrt((position.X * position.X) + (position.Y * position.Y));
            var lon = Math.Atan2(position.Y, position.X);

            if (p < 1e-9)
            {
                // On the polar axis: latitude is ±90° and the height is measured from the pole.
                var polarLat = position.Z >= 0 ? 90.0 : -90.0;
                return new TangentPointPosition(polarLat, 0.0, Math.Abs(position.Z) - Wgs84.SemiMinorAxis);
            }

            // Iterative solution; converges to double precision in a handful of steps.
            var lat = Math.Atan2(position.Z, p * (1.0 - Wgs84.EccentricitySquared));
            var height = 0.0;
            for (var i = 0; i < MaxGeodeticIterations; i++)
            {
                var n = Wgs84.PrimeVerticalRadius(lat);
                height = (p / Math.Cos(lat)) - n;
                var nextLat = Math.Atan2(position.Z, p * (1.0 - (Wgs84.EccentricitySquared * n / (n + height))));
                var delta = Math.Abs(nextLat - lat);
                lat = nextLat;
                if (delta < GeodeticConvergenceRadians)
                {
                    break;
                }
            }

            var finalN = Wgs84.PrimeVerticalRadius(lat);
            height = (p / Math.Cos(lat)) - finalN;

            return new TangentPointPosition(Angles.RadiansToDegrees(lat), Angles.RadiansToDegrees(lon), height);
        }

        /// <summary>
        /// Expresses the displacement from an origin to an ECEF position in the origin's LTP-ENU frame.
        /// </summary>
        /// <param name="position">The ECEF position of the point.</param>
        /// <param name="origin">The geodetic position of the tangent point (origin of the ENU frame).</param>
        /// <returns>The East, North, Up displacement in meters.</returns>
        public static EnuVector EcefToEnu(EcefPosition position, TangentPointPosition origin)
        {
            var originEcef = GeodeticToEcef(origin);
            var dx = position.X - originEcef.X;
            var dy = position.Y - originEcef.Y;
            var dz = position.Z - originEcef.Z;

            var lat = Angles.DegreesToRadians(origin.Latitude);
            var lon = Angles.DegreesToRadians(origin.Longitude);
            var sinLat = Math.Sin(lat);
            var cosLat = Math.Cos(lat);
            var sinLon = Math.Sin(lon);
            var cosLon = Math.Cos(lon);

            var east = (-sinLon * dx) + (cosLon * dy);
            var north = (-sinLat * cosLon * dx) - (sinLat * sinLon * dy) + (cosLat * dz);
            var up = (cosLat * cosLon * dx) + (cosLat * sinLon * dy) + (sinLat * dz);

            return new EnuVector(east, north, up);
        }

        /// <summary>
        /// Converts a displacement in an origin's LTP-ENU frame back to an ECEF position.
        /// </summary>
        /// <param name="displacement">The East, North, Up displacement in meters.</param>
        /// <param name="origin">The geodetic position of the tangent point (origin of the ENU frame).</param>
        /// <returns>The ECEF position of the displaced point.</returns>
        public static EcefPosition EnuToEcef(EnuVector displacement, TangentPointPosition origin)
        {
            var originEcef = GeodeticToEcef(origin);
            var lat = Angles.DegreesToRadians(origin.Latitude);
            var lon = Angles.DegreesToRadians(origin.Longitude);
            var sinLat = Math.Sin(lat);
            var cosLat = Math.Cos(lat);
            var sinLon = Math.Sin(lon);
            var cosLon = Math.Cos(lon);

            var e = displacement.East;
            var n = displacement.North;
            var u = displacement.Up;

            var dx = (-sinLon * e) - (sinLat * cosLon * n) + (cosLat * cosLon * u);
            var dy = (cosLon * e) - (sinLat * sinLon * n) + (cosLat * sinLon * u);
            var dz = (cosLat * n) + (sinLat * u);

            return new EcefPosition(originEcef.X + dx, originEcef.Y + dy, originEcef.Z + dz);
        }

        /// <summary>
        /// Expresses the displacement from an origin to another geodetic position in the origin's LTP-ENU frame.
        /// </summary>
        /// <param name="position">The geodetic position of the point.</param>
        /// <param name="origin">The geodetic position of the tangent point (origin of the ENU frame).</param>
        /// <returns>The East, North, Up displacement in meters.</returns>
        public static EnuVector GeodeticToEnu(TangentPointPosition position, TangentPointPosition origin)
        {
            return EcefToEnu(GeodeticToEcef(position), origin);
        }

        /// <summary>
        /// Converts a displacement in an origin's LTP-ENU frame to a geodetic position.
        /// </summary>
        /// <param name="displacement">The East, North, Up displacement in meters.</param>
        /// <param name="origin">The geodetic position of the tangent point (origin of the ENU frame).</param>
        /// <returns>The geodetic position of the displaced point.</returns>
        public static TangentPointPosition EnuToGeodetic(EnuVector displacement, TangentPointPosition origin)
        {
            return EcefToGeodetic(EnuToEcef(displacement, origin));
        }

        /// <summary>
        /// Great circle distance between two positions over the WGS 84 mean sphere, using the haversine formula. Heights are ignored.
        /// </summary>
        /// <param name="from">The first position.</param>
        /// <param name="to">The second position.</param>
        /// <returns>The distance in meters.</returns>
        public static double GreatCircleDistance(TangentPointPosition from, TangentPointPosition to)
        {
            var lat1 = Angles.DegreesToRadians(from.Latitude);
            var lat2 = Angles.DegreesToRadians(to.Latitude);
            var dLat = lat2 - lat1;
            var dLon = Angles.DegreesToRadians(to.Longitude - from.Longitude);

            var sinHalfLat = Math.Sin(dLat / 2.0);
            var sinHalfLon = Math.Sin(dLon / 2.0);
            var a = (sinHalfLat * sinHalfLat) + (Math.Cos(lat1) * Math.Cos(lat2) * sinHalfLon * sinHalfLon);
            var c = 2.0 * Math.Asin(Math.Min(1.0, Math.Sqrt(a)));

            return Wgs84.MeanRadius * c;
        }

        /// <summary>
        /// Initial compass bearing of the great circle from one position to another.
        /// </summary>
        /// <param name="from">The starting position.</param>
        /// <param name="to">The destination position.</param>
        /// <returns>The bearing in decimal degrees, clockwise from true North, in the range [0, 360).</returns>
        public static double InitialBearing(TangentPointPosition from, TangentPointPosition to)
        {
            var lat1 = Angles.DegreesToRadians(from.Latitude);
            var lat2 = Angles.DegreesToRadians(to.Latitude);
            var dLon = Angles.DegreesToRadians(to.Longitude - from.Longitude);

            var y = Math.Sin(dLon) * Math.Cos(lat2);
            var x = (Math.Cos(lat1) * Math.Sin(lat2)) - (Math.Sin(lat1) * Math.Cos(lat2) * Math.Cos(dLon));

            return Angles.Normalize360(Angles.RadiansToDegrees(Math.Atan2(y, x)));
        }
    }
}
