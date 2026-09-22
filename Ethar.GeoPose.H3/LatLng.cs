// <copyright file="LatLng.cs" company="Ethar">
// Copyright (c) Ethar. All rights reserved.
// </copyright>

/* Ported from Uber H3 v4.5.0, src/h3lib/lib/latLng.c, src/h3lib/include/latLng.h and src/h3lib/include/h3api.h.in. Copyright 2016-2023, 2026 Uber Technologies, Inc. Licensed under the Apache License, Version 2.0. */

namespace Ethar.GeoPose.H3
{
    using System;
    using Ethar.GeoPose.H3.Tables;
    using Ethar.GeoPose.JsonConversion;

    /// <summary>
    /// Latitude and longitude in radians, exactly as the H3 C API. Ported from the C struct <c>LatLng</c>.
    /// </summary>
    /// <remarks>
    /// Use the extension methods in <c>Ethar.GeoPose.Extensions</c> to move between this type and a GeoPose <c>TangentPointPosition</c> in degrees.
    /// </remarks>
    public readonly struct LatLng : IEquatable<LatLng>
    {
        /// <summary>
        /// Epsilon of about 0.1 mm in degrees. C name <c>EPSILON_DEG</c>.
        /// </summary>
        internal const double EpsilonDeg = .000000001;

        /// <summary>
        /// Epsilon of about 0.1 mm in radians. C name <c>EPSILON_RAD</c>.
        /// </summary>
        internal const double EpsilonRad = EpsilonDeg * Constants.M_PI_180;

        /// <summary>
        /// Initializes a new instance of the <see cref="LatLng"/> struct.
        /// </summary>
        /// <param name="lat">The latitude in radians.</param>
        /// <param name="lng">The longitude in radians.</param>
        public LatLng(double lat, double lng)
        {
            this.Lat = lat;
            this.Lng = lng;
        }

        /// <summary>
        /// Gets the latitude in radians.
        /// </summary>
        public double Lat { get; }

        /// <summary>
        /// Gets the longitude in radians.
        /// </summary>
        public double Lng { get; }

        /// <summary>
        /// Returns true if both coordinates are exactly equal.
        /// </summary>
        /// <param name="left">The first coordinate.</param>
        /// <param name="right">The second coordinate.</param>
        /// <returns>True if equal.</returns>
        public static bool operator ==(LatLng left, LatLng right) => left.Equals(right);

        /// <summary>
        /// Returns true if the coordinates differ.
        /// </summary>
        /// <param name="left">The first coordinate.</param>
        /// <param name="right">The second coordinate.</param>
        /// <returns>True if not equal.</returns>
        public static bool operator !=(LatLng left, LatLng right) => !left.Equals(right);

        /// <summary>
        /// Creates spherical coordinates from decimal degrees. C name <c>setGeoDegs</c>.
        /// </summary>
        /// <param name="latDegs">The latitude in decimal degrees.</param>
        /// <param name="lngDegs">The longitude in decimal degrees.</param>
        /// <returns>The coordinates in radians.</returns>
        public static LatLng FromDegrees(double latDegs, double lngDegs)
        {
            return new LatLng(H3.DegsToRads(latDegs), H3.DegsToRads(lngDegs));
        }

        /// <inheritdoc/>
        public bool Equals(LatLng other) => this.Lat.Equals(other.Lat) && this.Lng.Equals(other.Lng);

        /// <inheritdoc/>
        public override bool Equals(object obj) => obj is LatLng other && this.Equals(other);

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            unchecked
            {
                return (this.Lat.GetHashCode() * 397) ^ this.Lng.GetHashCode();
            }
        }

        /// <summary>
        /// Returns the coordinates in radians, culture invariant.
        /// </summary>
        /// <returns>The text, for example <c>Lat:0.71, Lng:-1.29</c>.</returns>
        public override string ToString()
        {
            return "Lat:" + InvariantNumber.Format(this.Lat) + ", Lng:" + InvariantNumber.Format(this.Lng);
        }

        /// <summary>
        /// Normalizes radians to a value between 0.0 and two PI. C name <c>_posAngleRads</c>.
        /// </summary>
        /// <param name="rads">The input radians value.</param>
        /// <returns>The normalized radians value.</returns>
        internal static double PosAngleRads(double rads)
        {
            var tmp = rads < 0.0 ? rads + Constants.M_2PI : rads;
            if (rads >= Constants.M_2PI)
            {
                tmp -= Constants.M_2PI;
            }

            return tmp;
        }

        /// <summary>
        /// Determines if the components of two spherical coordinates are within some threshold distance of each other. C name <c>geoAlmostEqualThreshold</c>.
        /// </summary>
        /// <param name="p1">The first spherical coordinates.</param>
        /// <param name="p2">The second spherical coordinates.</param>
        /// <param name="threshold">The threshold distance.</param>
        /// <returns>Whether or not the two coordinates are within the threshold distance of each other.</returns>
        internal static bool GeoAlmostEqualThreshold(LatLng p1, LatLng p2, double threshold)
        {
            return Math.Abs(p1.Lat - p2.Lat) < threshold && Math.Abs(p1.Lng - p2.Lng) < threshold;
        }

        /// <summary>
        /// Determines if the components of two spherical coordinates are within the standard epsilon distance of each other. C name <c>geoAlmostEqual</c>.
        /// </summary>
        /// <param name="p1">The first spherical coordinates.</param>
        /// <param name="p2">The second spherical coordinates.</param>
        /// <returns>Whether or not the two coordinates are within the epsilon distance of each other.</returns>
        internal static bool GeoAlmostEqual(LatLng p1, LatLng p2)
        {
            return GeoAlmostEqualThreshold(p1, p2, EpsilonRad);
        }

        /// <summary>
        /// Makes sure latitudes are in the proper bounds. C name <c>constrainLat</c>.
        /// </summary>
        /// <param name="lat">The original latitude value.</param>
        /// <returns>The corrected latitude value.</returns>
        internal static double ConstrainLat(double lat)
        {
            while (lat > Constants.M_PI_2)
            {
                lat = lat - Constants.M_PI;
            }

            return lat;
        }

        /// <summary>
        /// Makes sure longitudes are in the proper bounds. C name <c>constrainLng</c>.
        /// </summary>
        /// <param name="lng">The original longitude value.</param>
        /// <returns>The corrected longitude value.</returns>
        internal static double ConstrainLng(double lng)
        {
            while (lng > Constants.M_PI)
            {
                lng = lng - (2 * Constants.M_PI);
            }

            while (lng < -Constants.M_PI)
            {
                lng = lng + (2 * Constants.M_PI);
            }

            return lng;
        }
    }
}
