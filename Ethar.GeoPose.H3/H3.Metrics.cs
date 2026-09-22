// <copyright file="H3.Metrics.cs" company="Ethar">
// Copyright (c) Ethar. All rights reserved.
// </copyright>

/* Ported from Uber H3 v4.5.0, src/h3lib/lib/latLng.c. Copyright 2016-2023, 2026 Uber Technologies, Inc. Licensed under the Apache License, Version 2.0. */

namespace Ethar.GeoPose.H3
{
    using System;
    using Ethar.GeoPose.H3.Tables;

    /// <summary>
    /// Angle conversion, great circle distance and the per-resolution metrics from latLng.c.
    /// </summary>
    public static partial class H3
    {
        /// <summary>
        /// Average hexagon area in square kilometres by resolution. C name <c>areas</c> in <c>getHexagonAreaAvgKm2</c>.
        /// </summary>
        private static readonly double[] HexagonAreaAvgKm2 =
        {
            4.357449416078383e+06, 6.097884417941332e+05, 8.680178039899720e+04,
            1.239343465508816e+04, 1.770347654491307e+03, 2.529038581819449e+02,
            3.612906216441245e+01, 5.161293359717191e+00, 7.373275975944177e-01,
            1.053325134272067e-01, 1.504750190766435e-02, 2.149643129451879e-03,
            3.070918756316060e-04, 4.387026794728296e-05, 6.267181135324313e-06,
            8.953115907605790e-07,
        };

        /// <summary>
        /// Average hexagon area in square metres by resolution. C name <c>areas</c> in <c>getHexagonAreaAvgM2</c>.
        /// </summary>
        private static readonly double[] HexagonAreaAvgM2 =
        {
            4.357449416078390e+12, 6.097884417941339e+11, 8.680178039899731e+10,
            1.239343465508818e+10, 1.770347654491309e+09, 2.529038581819452e+08,
            3.612906216441250e+07, 5.161293359717198e+06, 7.373275975944188e+05,
            1.053325134272069e+05, 1.504750190766437e+04, 2.149643129451882e+03,
            3.070918756316063e+02, 4.387026794728301e+01, 6.267181135324322e+00,
            8.953115907605802e-01,
        };

        /// <summary>
        /// Average hexagon edge length in kilometres by resolution. C name <c>lens</c> in <c>getHexagonEdgeLengthAvgKm</c>.
        /// </summary>
        private static readonly double[] HexagonEdgeLengthAvgKm =
        {
            1281.256011, 483.0568391, 182.5129565, 68.97922179,
            26.07175968, 9.854090990, 3.724532667, 1.406475763,
            0.531414010, 0.200786148, 0.075863783, 0.028663897,
            0.010830188, 0.004092010, 0.001546100, 0.000584169,
        };

        /// <summary>
        /// Average hexagon edge length in metres by resolution. C name <c>lens</c> in <c>getHexagonEdgeLengthAvgM</c>.
        /// </summary>
        private static readonly double[] HexagonEdgeLengthAvgM =
        {
            1281256.011, 483056.8391, 182512.9565, 68979.22179,
            26071.75968, 9854.090990, 3724.532667, 1406.475763,
            531.4140101, 200.7861476, 75.86378287, 28.66389748,
            10.83018784, 4.092010473, 1.546099657, 0.584168630,
        };

        /// <summary>
        /// Convert from decimal degrees to radians. C name <c>degsToRads</c>.
        /// </summary>
        /// <param name="degrees">The decimal degrees.</param>
        /// <returns>The corresponding radians.</returns>
        public static double DegsToRads(double degrees)
        {
            return degrees * Constants.M_PI_180;
        }

        /// <summary>
        /// Convert from radians to decimal degrees. C name <c>radsToDegs</c>.
        /// </summary>
        /// <param name="radians">The radians.</param>
        /// <returns>The corresponding decimal degrees.</returns>
        public static double RadsToDegs(double radians)
        {
            return radians * Constants.M_180_PI;
        }

        /// <summary>
        /// The great circle distance in radians between two spherical coordinates, by the Haversine formula. C name <c>greatCircleDistanceRads</c>.
        /// </summary>
        /// <param name="a">The first coordinates in radians.</param>
        /// <param name="b">The second coordinates in radians.</param>
        /// <returns>The great circle distance in radians between a and b.</returns>
        public static double GreatCircleDistanceRads(LatLng a, LatLng b)
        {
            var sinLat = Math.Sin((b.Lat - a.Lat) * 0.5);
            var sinLng = Math.Sin((b.Lng - a.Lng) * 0.5);

            var h = (sinLat * sinLat) + (Math.Cos(a.Lat) * Math.Cos(b.Lat) * sinLng * sinLng);

            return 2 * Math.Atan2(Math.Sqrt(h), Math.Sqrt(1 - h));
        }

        /// <summary>
        /// The great circle distance in kilometres between two spherical coordinates, on the WGS84 authalic sphere H3 uses. C name <c>greatCircleDistanceKm</c>.
        /// </summary>
        /// <param name="a">The first coordinates in radians.</param>
        /// <param name="b">The second coordinates in radians.</param>
        /// <returns>The great circle distance in kilometres between a and b.</returns>
        public static double GreatCircleDistanceKm(LatLng a, LatLng b)
        {
            return GreatCircleDistanceRads(a, b) * Constants.EARTH_RADIUS_KM;
        }

        /// <summary>
        /// The great circle distance in metres between two spherical coordinates, on the WGS84 authalic sphere H3 uses. C name <c>greatCircleDistanceM</c>.
        /// </summary>
        /// <param name="a">The first coordinates in radians.</param>
        /// <param name="b">The second coordinates in radians.</param>
        /// <returns>The great circle distance in metres between a and b.</returns>
        public static double GreatCircleDistanceM(LatLng a, LatLng b)
        {
            return GreatCircleDistanceKm(a, b) * 1000;
        }

        /// <summary>
        /// Average hexagon area in square kilometres at the given resolution. C name <c>getHexagonAreaAvgKm2</c>.
        /// </summary>
        /// <param name="res">The resolution, 0 to 15.</param>
        /// <param name="output">The area.</param>
        /// <returns><see cref="H3ErrorCode.Success"/>, or <see cref="H3ErrorCode.ResDomain"/> for a resolution outside 0 to 15.</returns>
        public static H3ErrorCode GetHexagonAreaAvgKm2(int res, out double output)
        {
            return Lookup(HexagonAreaAvgKm2, res, out output);
        }

        /// <summary>
        /// Average hexagon area in square metres at the given resolution. C name <c>getHexagonAreaAvgM2</c>.
        /// </summary>
        /// <param name="res">The resolution, 0 to 15.</param>
        /// <param name="output">The area.</param>
        /// <returns><see cref="H3ErrorCode.Success"/>, or <see cref="H3ErrorCode.ResDomain"/> for a resolution outside 0 to 15.</returns>
        public static H3ErrorCode GetHexagonAreaAvgM2(int res, out double output)
        {
            return Lookup(HexagonAreaAvgM2, res, out output);
        }

        /// <summary>
        /// Average hexagon edge length in kilometres at the given resolution. C name <c>getHexagonEdgeLengthAvgKm</c>.
        /// </summary>
        /// <param name="res">The resolution, 0 to 15.</param>
        /// <param name="output">The edge length.</param>
        /// <returns><see cref="H3ErrorCode.Success"/>, or <see cref="H3ErrorCode.ResDomain"/> for a resolution outside 0 to 15.</returns>
        public static H3ErrorCode GetHexagonEdgeLengthAvgKm(int res, out double output)
        {
            return Lookup(HexagonEdgeLengthAvgKm, res, out output);
        }

        /// <summary>
        /// Average hexagon edge length in metres at the given resolution. C name <c>getHexagonEdgeLengthAvgM</c>.
        /// </summary>
        /// <param name="res">The resolution, 0 to 15.</param>
        /// <param name="output">The edge length.</param>
        /// <returns><see cref="H3ErrorCode.Success"/>, or <see cref="H3ErrorCode.ResDomain"/> for a resolution outside 0 to 15.</returns>
        public static H3ErrorCode GetHexagonEdgeLengthAvgM(int res, out double output)
        {
            return Lookup(HexagonEdgeLengthAvgM, res, out output);
        }

        /// <summary>
        /// Number of cells at the given resolution. C name <c>getNumCells</c>.
        /// </summary>
        /// <param name="res">The resolution, 0 to 15.</param>
        /// <param name="output">The number of cells, 2 + 120 * 7^res.</param>
        /// <returns><see cref="H3ErrorCode.Success"/>, or <see cref="H3ErrorCode.ResDomain"/> for a resolution outside 0 to 15.</returns>
        public static H3ErrorCode GetNumCells(int res, out long output)
        {
            if (res < 0 || res > Constants.MAX_H3_RES)
            {
                output = 0;
                return H3ErrorCode.ResDomain;
            }

            output = 2 + (120 * MathExtensions.Ipow(7, res));
            return H3ErrorCode.Success;
        }

        private static H3ErrorCode Lookup(double[] table, int res, out double output)
        {
            if (res < 0 || res > Constants.MAX_H3_RES)
            {
                output = 0;
                return H3ErrorCode.ResDomain;
            }

            output = table[res];
            return H3ErrorCode.Success;
        }
    }
}
